using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Services.SubjectService;
using LMCM_BE.AutoMapper.SubjectProfile;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Services.SyllabusService;
using LMCM_BE.AutoMapper.SyllabusProfiles;
using LMCM_BE.Repositories.CLORepository;
using LMCM_BE.Repositories.CurriculumRepository;
using LMCM_BE.Services.CurriculumService;
using LMCM_BE.Repositories.PloRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE;
using LMCM_BE.AutoMapper.CLOProfiles;
using LMCM_BE.AutoMapper.UserProfiles;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.UserService;
using LMCM_BE.Repositories.ScheduleRepository;
using LMCM_BE.AutoMapper.ScheduleProfiles;
using LMCM_BE.AutoMapper.GradingStructureProfiles;
using LMCM_BE.Repositories.GradingStructureRepository;
using LMCM_BE.AutoMapper.ConstructivistQuestionProfiles;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.AutoMapper.LearningMaterialProfiles;
using LMCM_BE.Repositories.LearningMaterialRepository;
using LMCM_BE.Services.LearningMaterialService;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.ContractService;
using LMCM_BE.AutoMapper.ContractProfiles;
using LMCM_BE.Services.PloService;
using LMCM_BE.AutoMapper.BudgetProposalProfile;
using LMCM_BE.Repositories.BudgetPropasalRepository;
using LMCM_BE.Services.BudgetPropasalService;
using LMCM_BE.AutoMapper.PloProfiles;
using LMCM_BE.Services.ContractorService;
using LMCM_BE.Repositories.ContractorRepository;
using LMCM_BE.AutoMapper.AcceptanceRecordProfile;
using LMCM_BE.Services.AcceptanceRecordService;
using LMCM_BE.Repositories.AcceptanceRecordRepository;
using LMCM_BE.Utilities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using LMCM_BE.AutoMapper.TemplateProfiles;
using LMCM_BE.Repositories.DocumentTemplateRepository;
using LMCM_BE.Services.DocumentTemplateService;
using LMCM_BE.Repositories.ContractValueItemRepository;
using LMCM_BE.Services.ContractValueItemService;
using LMCM_BE.AutoMapper.LearningMaterialChangesHistoryProfile;
using LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository;
using LMCM_BE.Services.LearningMaterialChangesHistoryService;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Services.CurriculumsSubjectService;
using LMCM_BE.Services.PloSubjectService;
using LMCM_BE.Services.DashboardService;
using LMCM_BE.AutoMapper.ContractValueItemProfiles;
using LMCM_BE.Services.OpenAIService;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LMCM_DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
// Add Swagger services
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme =JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter jwt token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition("Bearer",jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {jwtSecurityScheme,Array.Empty<string>() }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Secret"])),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        RequireExpirationTime = true,
        ValidateLifetime = true,
    };
});
builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.None; // Allow HTTP (For develovment)
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<LMCM_DBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddSingleton<DriveService>(provider =>
{
    GoogleCredential credential;
    using (var stream = new FileStream("google-drive-credentials.json", FileMode.Open, FileAccess.Read))
    {
        credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.Scope.Drive);
    }

    return new DriveService(new BaseClientService.Initializer
    {
        HttpClientInitializer = credential,
        ApplicationName = "LMCM"
    });
});

//AutoMapper
builder.Services.AddAutoMapper(typeof(SubjectProfile));
builder.Services.AddAutoMapper(typeof(SyllabusProfile));
builder.Services.AddAutoMapper(typeof(CLOProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(ScheduleIProfile));
builder.Services.AddAutoMapper(typeof(GradingStructureProfile));
builder.Services.AddAutoMapper(typeof(ConstructivistQuestionProfile));
builder.Services.AddAutoMapper(typeof(LearningMaterialChangesHistoryProfile));
builder.Services.AddAutoMapper(typeof(LearningMaterialProfile));
builder.Services.AddAutoMapper(typeof(ContractProfile));
builder.Services.AddAutoMapper(typeof(BudgetPropasalProfile));
builder.Services.AddAutoMapper(typeof(PloProfile));
builder.Services.AddAutoMapper(typeof(ContractProfile));
builder.Services.AddAutoMapper(typeof(AcceptanceRecordProfile));
builder.Services.AddAutoMapper(typeof(DocumentTemplateProfile));
builder.Services.AddAutoMapper(typeof(ContractValueItemProfile));

//DI
builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<ICurriculumService, CurriculumService>();
builder.Services.AddScoped<IPloRepository, PloRepository>();
builder.Services.AddScoped<IPloService, PloService>();
builder.Services.AddScoped<IPloSubjectRepository, PloSubjectRepository>();
builder.Services.AddScoped<IPloSubjectService, PloSubjectService>();
builder.Services.AddScoped<ICurriculumsSubjectRepository, CurriculumsSubjectRepository>();
builder.Services.AddScoped<ICurriculumsSubjectService, CurriculumsSubjectService>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISyllabusRepository, SyllabusRepository>();
builder.Services.AddScoped<ISyllabusService, SyllabusService>();
builder.Services.AddScoped<ICLORepository, CLOReposiroty>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IGradingStructureRepository, GradingStructureRepository>();
builder.Services.AddScoped<IConstructivistQuestionRepository, ConstructivistQuestionRepository>();
builder.Services.AddScoped<ILearningMaterialChangesHistoryRepository, LearningMaterialChangesHistoryRepository>();
builder.Services.AddScoped<ILearningMaterialChangesHistorySerivce, LearningMaterialChangesHistorySerivce>();
builder.Services.AddScoped<ILearningMaterialRepository, LearningMaterialRepository>();
builder.Services.AddScoped<ILearningMaterialService, LearningMaterialService>();
builder.Services.AddScoped<GoogleDriveService>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IBudgetProposalRepository, BudgetProposalRepository>();
builder.Services.AddScoped<IBudgetProposalService, BudgetProposalService>();
builder.Services.AddScoped<IContractorRepository, ContractorRepository>();
builder.Services.AddScoped<IContractorService, ContractorService>();
builder.Services.AddScoped<IContractValueItemRepository, ContractValueItemRepository>();
builder.Services.AddScoped<IContractValueItemService, ContractValueItemService>();
builder.Services.AddScoped<IAcceptanceRecordRepository, AcceptanceRecordRepository>();
builder.Services.AddScoped<IAcceptanceRecordService, AcceptanceRecordService>();
builder.Services.AddScoped<IFileHelper, FileHelper>();
builder.Services.AddScoped<IDocumentTemplateRepository, DocumentTemplateRepository>();
builder.Services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseCookiePolicy();

app.MapControllers();

app.Run();
