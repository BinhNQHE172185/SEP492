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
using LMCM_BE.Services.CLOService;
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
using LMCM_BE.Services.ScheduleService;
using LMCM_BE.AutoMapper.ScheduleProfiles;
using LMCM_BE.AutoMapper.GradingStructureProfiles;
using LMCM_BE.Repositories.GradingStructureRepository;
using LMCM_BE.Services.GradingStructureService;
using LMCM_BE.AutoMapper.ConstructivistQuestionProfiles;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.Services.ConstructivistQuestionService;
using LMCM_BE.AutoMapper.LearningMaterialChangesHistoryProfiles;
using LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository;
using LMCM_BE.Services.LearningMaterialChangesHistoryService;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LMCM_DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F-JaNdRfUserjd89#5*6Xn2r5usErw8x/A?D(G+KbPeShV")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<LMCM_DBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
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

//DI
builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<ICurriculumService, CurriculumService>();
builder.Services.AddScoped<IPloRepository, PloRepository>();
builder.Services.AddScoped<IPloSubjectRepository, PloSubjectRepository>();
builder.Services.AddScoped<ICurriculumsSubjectRepository, CurriculumsSubjectRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISyllabusRepository, SyllabusRepository>();
builder.Services.AddScoped<ISyllabusService, SyllabusService>();
builder.Services.AddScoped<ICLORepository, CLOReposiroty>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICLOService, CLOService>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IGradingStructureRepository, GradingStructureRepository>();
builder.Services.AddScoped<IGradingStructureService, GradingStructureService>();
builder.Services.AddScoped<IConstructivistQuestionRepository, ConstructivistQuestionRepository>();
builder.Services.AddScoped<IConstructivistQuestionService, ConstructivistQuestionService>();
builder.Services.AddScoped<ILearningMaterialChangesHistoryRepository, LearningMaterialChangesHistoryRepository>();
builder.Services.AddScoped<ILearningMaterialChangesHistorySerivce, LearningMaterialChangesHistorySerivce>();

builder.Services.AddAuthorization();

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

app.MapControllers();

app.Run();
