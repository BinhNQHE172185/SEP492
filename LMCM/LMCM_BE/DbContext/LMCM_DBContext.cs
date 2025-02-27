using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LMCM_BE.Models;

namespace LMCM_BE.DbContext;

public partial class LMCM_DBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public LMCM_DBContext()
    {
    }

    public LMCM_DBContext(DbContextOptions<LMCM_DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcceptanceRecord> AcceptanceRecords { get; set; }

    public virtual DbSet<BudgetProposal> BudgetProposals { get; set; }

    public virtual DbSet<Clo> Clos { get; set; }

    public virtual DbSet<ConstructivistQuestion> ConstructivistQuestions { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Contractor> Contractors { get; set; }

    public virtual DbSet<Curriculum> Curriculums { get; set; }

    public virtual DbSet<CurriculumsSubject> CurriculumsSubjects { get; set; }

    public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; }

    public virtual DbSet<GradingStructure> GradingStructures { get; set; }

    public virtual DbSet<HistoryOfChange> HistoryOfChanges { get; set; }

    public virtual DbSet<ImportedLearningMaterial> ImportedLearningMaterials { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Plo> Plos { get; set; }

    public virtual DbSet<PloSubject> PloSubjects { get; set; }

    public virtual DbSet<ReferencedLearningMaterial> ReferencedLearningMaterials { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectsSyllabus> SubjectsSyllabi { get; set; }

    public virtual DbSet<Syllabus> Syllabi { get; set; }

    public virtual DbSet<SyllabusReferencedLearningMaterial> SyllabusReferencedLearningMaterials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyDB"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AcceptanceRecord>(entity =>
        {
            entity.HasKey(e => e.AcceptanceId).HasName("PK__Acceptan__FC008882B335D583");

            entity.ToTable("Acceptance_Record");

            entity.Property(e => e.AcceptanceId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Acceptance_ID");
            entity.Property(e => e.AcceptanceDate).HasColumnName("Acceptance_Date");
            entity.Property(e => e.ContractId).HasColumnName("Contract_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("Final_Price");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Contract).WithMany(p => p.AcceptanceRecords)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Acceptanc__Contr__282DF8C2");
        });

        modelBuilder.Entity<BudgetProposal>(entity =>
        {
            entity.HasKey(e => e.ProposalId).HasName("PK__Budget_P__C9D0461F0E26CF3B");

            entity.ToTable("Budget_Proposals");

            entity.Property(e => e.ProposalId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Proposal_ID");
            entity.Property(e => e.AuthorId).HasColumnName("Author_ID");
            entity.Property(e => e.ContractId).HasColumnName("Contract_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ProposalDate).HasColumnName("Proposal_Date");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.BudgetProposals)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Budget_Pr__Autho__2180FB33");

            entity.HasOne(d => d.Contract).WithMany(p => p.BudgetProposals)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Budget_Pr__Contr__22751F6C");
        });

        modelBuilder.Entity<Clo>(entity =>
        {
            entity.HasKey(e => e.CloId).HasName("PK__CLO__C3755EAD98282D65");

            entity.ToTable("CLO");

            entity.Property(e => e.CloId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("CLO_ID");
            entity.Property(e => e.CloDescription)
                .HasMaxLength(255)
                .HasColumnName("CLO_Description");
            entity.Property(e => e.CloName)
                .HasMaxLength(255)
                .HasColumnName("CLO_Name");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.Clos)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CLO__Syllabus_ID__06CD04F7");
        });

        modelBuilder.Entity<ConstructivistQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Construc__B0B2E4C616BF6D17");

            entity.ToTable("Constructivist_Question");

            entity.Property(e => e.QuestionId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Question_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.QuestionDetail)
                .HasMaxLength(255)
                .HasColumnName("Question_Detail");
            entity.Property(e => e.QuestionName)
                .HasMaxLength(255)
                .HasColumnName("Question_Name");
            entity.Property(e => e.SessionNo).HasColumnName("Session_No");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.ConstructivistQuestions)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Construct__Sylla__01142BA1");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__5E2E73DAA7081C7B");

            entity.Property(e => e.ContractId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Contract_ID");
            entity.Property(e => e.ContractDate).HasColumnName("Contract_Date");
            entity.Property(e => e.ContractValue)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("Contract_Value");
            entity.Property(e => e.ContractorId).HasColumnName("Contractor_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Contractor).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ContractorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Contr__1BC821DD");
        });

        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.HasKey(e => e.ContractorId).HasName("PK__Contract__61C4678DC17AEF97");

            entity.ToTable("Contractor");

            entity.Property(e => e.ContractorId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Contractor_ID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(255)
                .HasColumnName("Bank_Account_Number");
            entity.Property(e => e.BankName)
                .HasMaxLength(255)
                .HasColumnName("Bank_Name");
            entity.Property(e => e.ContractorName)
                .HasMaxLength(255)
                .HasColumnName("Contractor_Name");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("Email");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(255)
                .HasColumnName("Employee_Code");
            entity.Property(e => e.IdCardNumber)
                .HasMaxLength(255)
                .HasColumnName("ID_Card_Number");
            entity.Property(e => e.IdIssuedPlace)
                .HasMaxLength(255)
                .HasColumnName("ID_Issued_Place");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("Phone_Number");
            entity.Property(e => e.Position).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TaxCode)
                .HasMaxLength(255)
                .HasColumnName("Tax_Code");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<Curriculum>(entity =>
        {
            entity.HasKey(e => e.CurriculumId).HasName("PK__Curricul__2F88E2C2833996B8");

            entity.Property(e => e.CurriculumId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Curriculum_ID");
            entity.Property(e => e.ApprovedDate).HasColumnName("Approved_Date");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CurriculumCode)
                .HasMaxLength(255)
                .HasColumnName("Curriculum_Code");
            entity.Property(e => e.CurriculumDescription)
                .HasMaxLength(255)
                .HasColumnName("Curriculum_Description");
            entity.Property(e => e.CurriculumName)
                .HasMaxLength(255)
                .HasColumnName("Curriculum_Name");
            entity.Property(e => e.CurriculumNameEnglish)
                .HasMaxLength(255)
                .HasColumnName("Curriculum_Name_English");
            entity.Property(e => e.DecisionNo)
                .HasMaxLength(255)
                .HasColumnName("Decision_No");
            entity.Property(e => e.EnglishVocationalName)
                .HasMaxLength(255)
                .HasColumnName("English_Vocational_Name");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.VocationalCode)
                .HasMaxLength(255)
                .HasColumnName("Vocational_Code");
            entity.Property(e => e.VocationalName)
                .HasMaxLength(255)
                .HasColumnName("Vocational_Name");
        });

        modelBuilder.Entity<CurriculumsSubject>(entity =>
        {
            entity.HasKey(e => new { e.CurriculumId, e.SubjectId, e.CreatedAt }).HasName("PK__Curricul__39754AB284BC4AC1");

            entity.ToTable("Curriculums_Subjects");

            entity.Property(e => e.CurriculumId).HasColumnName("Curriculum_ID");
            entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TermNo).HasColumnName("Term_No");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Curriculum).WithMany(p => p.CurriculumsSubjects)
                .HasForeignKey(d => d.CurriculumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Curriculu__Curri__4D94879B");

            entity.HasOne(d => d.Subject).WithMany(p => p.CurriculumsSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Curriculu__Subje__4E88ABD4");
        });

        modelBuilder.Entity<DocumentTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__Document__E7FB8F0169989A10");

            entity.ToTable("Document_Templates");

            entity.Property(e => e.TemplateId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Template_ID");
            entity.Property(e => e.AuthorId).HasColumnName("Author_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.TemplateName)
                .HasMaxLength(255)
                .HasColumnName("Template_Name");
            entity.Property(e => e.TemplateType)
                .HasMaxLength(255)
                .HasColumnName("Template_Type");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.DocumentTemplates)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Document___Updat__2DE6D218");
        });

        modelBuilder.Entity<GradingStructure>(entity =>
        {
            entity.HasKey(e => e.StructureId).HasName("PK__Grading___71D721C6E3213E63");

            entity.ToTable("Grading_Structure");

            entity.Property(e => e.StructureId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Structure_ID");
            entity.Property(e => e.AssessmentComponent)
                .HasMaxLength(255)
                .HasColumnName("Assessment_Component");
            entity.Property(e => e.AssessmentType)
                .HasMaxLength(255)
                .HasColumnName("Assessment_Type");
            entity.Property(e => e.Clo)
                .HasMaxLength(255)
                .HasColumnName("CLO");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Duration).HasMaxLength(255);
            entity.Property(e => e.How).HasMaxLength(255);
            entity.Property(e => e.MinValue)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("Min_Value");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.QuestionNo)
                .HasMaxLength(255)
                .HasColumnName("Question_No");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(255)
                .HasColumnName("Question_Type");
            entity.Property(e => e.Reference).HasMaxLength(255);
            entity.Property(e => e.Scope).HasMaxLength(255);
            entity.Property(e => e.SessionNo).HasColumnName("Session_No");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.StructureNo).HasColumnName("Structure_No");
            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Weight).HasColumnType("decimal(19, 2)");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.GradingStructures)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grading_S__Sylla__7B5B524B");
        });

        modelBuilder.Entity<HistoryOfChange>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__History___A6BABA37CB1AD5F5");

            entity.ToTable("History_Of_Changes");

            entity.Property(e => e.HistoryId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("History_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ItemIdNew).HasColumnName("Item_ID_New");
            entity.Property(e => e.ItemIdOld).HasColumnName("Item_ID_Old");
            entity.Property(e => e.ItemType)
                .HasMaxLength(255)
                .HasColumnName("Item_Type");
        });

        modelBuilder.Entity<ImportedLearningMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Imported__3A09B0FD5110BD93");

            entity.ToTable("Imported_Learning_Materials");

            entity.Property(e => e.MaterialId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Material_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(255)
                .HasColumnName("Material_Name");
            entity.Property(e => e.MaterialNo).HasColumnName("Material_No");
            entity.Property(e => e.MaterialQuantity)
                .HasMaxLength(255)
                .HasColumnName("Material_Quantity");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Purpose).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Syllabus).WithMany(p => p.ImportedLearningMaterials)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Imported___Sylla__0C85DE4D");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__8C1160B5B03230B9");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Notification_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__User___114A936A");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC072C2E5CB1");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ItemId).HasColumnName("Item_ID");
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<Plo>(entity =>
        {
            entity.HasKey(e => e.PloId).HasName("PK__PLO__3BEF007EFE658749");

            entity.ToTable("PLO");

            entity.Property(e => e.PloId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PLO_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CurriculumId).HasColumnName("Curriculum_ID");
            entity.Property(e => e.PloDescription)
                .HasMaxLength(255)
                .HasColumnName("PLO_Description");
            entity.Property(e => e.PloName)
                .HasMaxLength(255)
                .HasColumnName("PLO_Name");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Curriculum).WithMany(p => p.Plos)
                .HasForeignKey(d => d.CurriculumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PLO__Curriculum___5441852A");
        });

        modelBuilder.Entity<PloSubject>(entity =>
        {
            entity.HasKey(e => new { e.PloId, e.SubjectId, e.CreatedAt }).HasName("PK__PLO_Subj__2D12A80EB8A2C766");

            entity.ToTable("PLO_Subjects");

            entity.Property(e => e.PloId).HasColumnName("PLO_ID");
            entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Plo).WithMany(p => p.PloSubjects)
                .HasForeignKey(d => d.PloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PLO_Subje__PLO_I__59063A47");

            entity.HasOne(d => d.Subject).WithMany(p => p.PloSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PLO_Subje__Subje__59FA5E80");
        });

        modelBuilder.Entity<ReferencedLearningMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Referenc__3A09B0FD1BA50115");

            entity.ToTable("Referenced_Learning_Materials");

            entity.Property(e => e.MaterialId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Material_ID");
            entity.Property(e => e.Author).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Edition).HasMaxLength(255);
            entity.Property(e => e.Isbn)
                .HasMaxLength(255)
                .HasColumnName("ISBN");
            entity.Property(e => e.MaterialDescription)
                .HasMaxLength(255)
                .HasColumnName("Material_Description");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(255)
                .HasColumnName("Material_Name");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PublishedDate).HasColumnName("Published_Date");
            entity.Property(e => e.Publisher).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__8C4D3BBB5DCC3D6E");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Schedule_ID");
            entity.Property(e => e.Clo)
                .HasMaxLength(255)
                .HasColumnName("CLO");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Itu)
                .HasMaxLength(255)
                .HasColumnName("ITU");
            entity.Property(e => e.LecturerMaterial)
                .HasMaxLength(255)
                .HasColumnName("Lecturer_Material");
            entity.Property(e => e.LecturerMaterialUrl)
                .HasMaxLength(255)
                .HasColumnName("Lecturer_Material_Url");
            entity.Property(e => e.LecturerTask)
                .HasMaxLength(255)
                .HasColumnName("Lecturer_Task");
            entity.Property(e => e.Method).HasMaxLength(255);
            entity.Property(e => e.ScheduleNo).HasColumnName("Schedule_No");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.StudentMaterial)
                .HasMaxLength(255)
                .HasColumnName("Student_Material");
            entity.Property(e => e.StudentMaterialUrl)
                .HasMaxLength(255)
                .HasColumnName("Student_Material_Url");
            entity.Property(e => e.StudentTask)
                .HasMaxLength(255)
                .HasColumnName("Student_Task");
            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedule__Syllab__75A278F5");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__D98F54D65461C76D");

            entity.HasIndex(e => e.SubjectCode, "UQ__Subjects__4A7C5769A05FD9C7").IsUnique();

            entity.Property(e => e.SubjectId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Subject_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsConstructivist).HasColumnName("isConstructivist");
            entity.Property(e => e.Method).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(255)
                .HasColumnName("Subject_Code");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(255)
                .HasColumnName("Subject_Name");
            entity.Property(e => e.SubjectNameEnglish)
                .HasMaxLength(255)
                .HasColumnName("Subject_Name_English");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<SubjectsSyllabus>(entity =>
        {
            entity.HasKey(e => new { e.SubjectId, e.SyllabusId, e.CreatedAt }).HasName("PK__Subjects__A013BD7E2D6623F4");

            entity.ToTable("Subjects_Syllabus");

            entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");
            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectsSyllabi)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subjects___Subje__6477ECF3");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.SubjectsSyllabi)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subjects___Sylla__656C112C");
        });

        modelBuilder.Entity<Syllabus>(entity =>
        {
            entity.HasKey(e => e.SyllabusId).HasName("PK__Syllabus__2F9B4950E783D5C2");

            entity.ToTable("Syllabus");

            entity.HasIndex(e => e.CourseCode, "UQ__Syllabus__1AE5B24D8229B1C8").IsUnique();

            entity.Property(e => e.SyllabusId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Syllabus_ID");
            entity.Property(e => e.ApprovedDate).HasColumnName("Approved_Date");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(255)
                .HasColumnName("Course_Code");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("Course_Name");
            entity.Property(e => e.CourseNameEnglish)
                .HasMaxLength(255)
                .HasColumnName("Course_Name_English");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DecisionNo)
                .HasMaxLength(255)
                .HasColumnName("Decision_No");
            entity.Property(e => e.DegreeLevel)
                .HasMaxLength(255)
                .HasColumnName("Degree_Level");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.LearningTeachingMethod)
                .HasMaxLength(255)
                .HasColumnName("Learning_Teaching_Method");
            entity.Property(e => e.MinGpaToPass)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("Min_GPA_to_Pass");
            entity.Property(e => e.NoOfCredits).HasColumnName("No_of_Credits");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.PreRequisite)
                .HasMaxLength(255)
                .HasColumnName("Pre_requisite");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(255)
                .HasColumnName("Program_Name");
            entity.Property(e => e.ScoringScale)
                .HasColumnType("decimal(19, 2)")
                .HasColumnName("Scoring_Scale");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.StudentTask)
                .HasMaxLength(255)
                .HasColumnName("Student_Task");
            entity.Property(e => e.TimeAllocation)
                .HasMaxLength(255)
                .HasColumnName("Time_Allocation");
            entity.Property(e => e.Tools).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<SyllabusReferencedLearningMaterial>(entity =>
        {
            entity.HasKey(e => new { e.SyllabusId, e.MaterialId, e.CreatedAt }).HasName("PK__Syllabus__875E8F62998A0BCA");

            entity.ToTable("Syllabus_Referenced_Learning_Materials");

            entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
            entity.Property(e => e.MaterialId).HasColumnName("Material_ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Material).WithMany(p => p.SyllabusReferencedLearningMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Syllabus___Mater__6FE99F9F");

            entity.HasOne(d => d.Syllabus).WithMany(p => p.SyllabusReferencedLearningMaterials)
                .HasForeignKey(d => d.SyllabusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Syllabus___Sylla__6EF57B66");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Picture).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
