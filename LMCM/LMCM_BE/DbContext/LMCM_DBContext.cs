using System;
using System.Collections.Generic;
using LMCM_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BusinessObject.Context
{
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

        public virtual DbSet<Curriculum> Curriculums { get; set; }

        public virtual DbSet<CurriculumsSubject> CurriculumsSubjects { get; set; }

        public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; }

        public virtual DbSet<GradingStructure> GradingStructures { get; set; }

        public virtual DbSet<LearningMaterial> LearningMaterials { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<Plo> Plos { get; set; }

        public virtual DbSet<Schedule> Schedules { get; set; }

        public virtual DbSet<Subject> Subjects { get; set; }

        public virtual DbSet<SubjectsSyllabus> SubjectsSyllabi { get; set; }

        public virtual DbSet<Syllabus> Syllabi { get; set; }

        public virtual DbSet<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; }
        public virtual DbSet<User> Users { get; set; } = null!;

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
                entity.HasKey(e => e.AcceptanceId)
                    .HasName("PK__Acceptan__FC008882EB6B5A19");

                entity.ToTable("Acceptance_Record");

                entity.Property(e => e.AcceptanceId)
                    .HasColumnName("Acceptance_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AcceptanceDate).HasColumnName("Acceptance_Date");

                entity.Property(e => e.ContractId).HasColumnName("Contract_ID");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.FinalPrice)
                    .HasColumnType("decimal(19, 2)")
                    .HasColumnName("Final_Price");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Url).HasMaxLength(255);

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.AcceptanceRecords)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK__Acceptanc__Contr__04E4BC85");
            });

            modelBuilder.Entity<BudgetProposal>(entity =>
            {
                entity.HasKey(e => e.ProposalId)
                    .HasName("PK__Budget_P__C9D0461F9781A89B");

                entity.ToTable("Budget_Proposals");

                entity.Property(e => e.ProposalId)
                    .HasColumnName("Proposal_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AuthorId).HasColumnName("Author_ID");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.ProposalDate).HasColumnName("Proposal_Date");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Url).HasMaxLength(255);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.BudgetProposals)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK__Budget_Pr__Autho__7F2BE32F");
            });

            modelBuilder.Entity<Clo>(entity =>
            {
                entity.HasKey(e => e.CloId).HasName("PK__CLO__C3755EAD5A32F777");

                entity
                    .ToTable("CLO")
                    .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("CLO_History", "dbo");
                        ttb
                        .HasPeriodStart("CreatedAt")
                        .HasColumnName("CreatedAt");
                        ttb
                        .HasPeriodEnd("UpdatedAt")
                        .HasColumnName("UpdatedAt");
                    }));

                entity.Property(e => e.CloId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("CLO_ID");
                entity.Property(e => e.CloDescription)
                    .HasMaxLength(255)
                    .HasColumnName("CLO_Description");
                entity.Property(e => e.CloName)
                    .HasMaxLength(255)
                    .HasColumnName("CLO_Name");
                entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");

                entity.HasOne(d => d.Syllabus).WithMany(p => p.Clos)
                    .HasForeignKey(d => d.SyllabusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CLO__Syllabus_ID__0F624AF8");
            });
            modelBuilder.Entity<ConstructivistQuestion>(entity =>
            {
                entity.HasKey(e => e.QuestionId).HasName("PK__Construc__B0B2E4C62AEA54F9");

                entity
                    .ToTable("Constructivist_Question")
                    .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Constructivist_Question_History", "dbo");
                        ttb
                        .HasPeriodStart("CreatedAt")
                        .HasColumnName("CreatedAt");
                        ttb
                        .HasPeriodEnd("UpdatedAt")
                        .HasColumnName("UpdatedAt");
                    }));

                entity.Property(e => e.QuestionId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("Question_ID");
                entity.Property(e => e.QuestionDetail)
                    .HasMaxLength(255)
                    .HasColumnName("Question_Detail");
                entity.Property(e => e.QuestionName)
                    .HasMaxLength(255)
                    .HasColumnName("Question_Name");
                entity.Property(e => e.SessionNo).HasColumnName("Session_No");
                entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");

                entity.HasOne(d => d.Syllabus).WithMany(p => p.ConstructivistQuestions)
                    .HasForeignKey(d => d.SyllabusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Construct__Sylla__08B54D69");
            });
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ContractId)
                    .HasColumnName("Contract_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ContractDate).HasColumnName("Contract_Date");

                entity.Property(e => e.ContractValue)
                    .HasColumnType("decimal(19, 2)")
                    .HasColumnName("Contract_Value");

                entity.Property(e => e.ContractorId).HasColumnName("Contractor_ID");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Url).HasMaxLength(255);

                entity.HasOne(d => d.Contractor)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.ContractorId)
                    .HasConstraintName("FK__Contracts__Contr__797309D9");
            });

            modelBuilder.Entity<Curriculum>(entity =>
            {
                entity.HasKey(e => e.CurriculumId).HasName("PK__Curricul__2F88E2C21CA8612E");

                entity.ToTable(tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("Curriculums_History", "dbo");
                    ttb
                        .HasPeriodStart("CreatedAt")
                        .HasColumnName("CreatedAt");
                    ttb
                        .HasPeriodEnd("UpdatedAt")
                        .HasColumnName("UpdatedAt");
                }));

                entity.Property(e => e.CurriculumId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("Curriculum_ID");
                entity.Property(e => e.ApprovedDate).HasColumnName("Approved_Date");
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
                entity.Property(e => e.VocationalCode)
                    .HasMaxLength(255)
                    .HasColumnName("Vocational_Code");
                entity.Property(e => e.VocationalName)
                    .HasMaxLength(255)
                    .HasColumnName("Vocational_Name");
            });

            modelBuilder.Entity<CurriculumsSubject>(entity =>
            {
                modelBuilder.Entity<CurriculumsSubject>()
        .HasKey(cs => new { cs.CurriculumId, cs.SubjectId });

                entity.ToTable("Curriculums_Subjects");

                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("Curriculums_Subjects_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.Property(e => e.CurriculumId).HasColumnName("Curriculum_ID");

                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.Property(e => e.TermNo).HasColumnName("Term_No");


                entity.HasOne(d => d.Curriculum)
                    .WithMany()
                    .HasForeignKey(d => d.CurriculumId)
                    .HasConstraintName("FK__Curriculu__Curri__534D60F1");

                entity.HasOne(d => d.Subject)
                    .WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("FK__Curriculu__Subje__5441852A");
            });

            modelBuilder.Entity<DocumentTemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateId)
                    .HasName("PK__Document__E7FB8F012F99C312");

                entity.ToTable("Document_Templates");

                entity.Property(e => e.TemplateId)
                    .HasColumnName("Template_ID")
                    .HasDefaultValueSql("(newid())");

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

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.DocumentTemplates)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK__Document___Updat__0A9D95DB");
            });
            modelBuilder.Entity<GradingStructure>(entity =>
            {
                entity.HasKey(e => e.StructureId).HasName("PK__Grading___71D721C6B560AAF0");

                entity
                    .ToTable("Grading_Structure")
                    .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Grading_Structure_History", "dbo");
                        ttb
                        .HasPeriodStart("CreatedAt")
                        .HasColumnName("CreatedAt");
                        ttb
                        .HasPeriodEnd("UpdatedAt")
                        .HasColumnName("UpdatedAt");
                    }));

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
                entity.Property(e => e.StructureNo).HasColumnName("Structure_No");
                entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");
                entity.Property(e => e.Weight).HasColumnType("decimal(19, 2)");

                entity.HasOne(d => d.Syllabus).WithMany(p => p.GradingStructures)
                    .HasForeignKey(d => d.SyllabusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Grading_S__Sylla__02084FDA");
            });

            modelBuilder.Entity<LearningMaterial>(entity =>
            {
                entity.HasKey(e => e.MaterialId)
                    .HasName("PK__Learning__3A09B0FD884B7FA8");

                entity.ToTable("Learning_Materials");

                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("LearningMaterials_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.Property(e => e.MaterialId)
                    .HasColumnName("Material_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Author).HasMaxLength(255);

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

                entity.Property(e => e.Url).HasMaxLength(255);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId)
                    .HasColumnName("Notification_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.Property(e => e.Message).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("User_ID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Notificat__User___73BA3083");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ItemId).HasColumnName("Item_ID");

                entity.Property(e => e.Type).HasMaxLength(255);
            });
            modelBuilder.Entity<Plo>(entity =>
            {
                entity.HasKey(e => e.PloId).HasName("PK__PLO__3BEF007EC7EFD1E5");

                entity
                    .ToTable("PLO")
                    .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("PLO_History", "dbo");
                        ttb
                        .HasPeriodStart("CreatedAt")
                        .HasColumnName("CreatedAt");
                        ttb
                        .HasPeriodEnd("UpdatedAt")
                        .HasColumnName("UpdatedAt");
                    }));

                entity.Property(e => e.PloId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("PLO_ID");
                entity.Property(e => e.CurriculumId).HasColumnName("Curriculum_ID");
                entity.Property(e => e.PloDescription)
                    .HasMaxLength(255)
                    .HasColumnName("PLO_Description");
                entity.Property(e => e.PloName)
                    .HasMaxLength(255)
                    .HasColumnName("PLO_Name");
                entity.Property(e => e.Status).HasMaxLength(255);
                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.HasOne(d => d.Curriculum).WithMany(p => p.Plos)
                    .HasForeignKey(d => d.CurriculumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PLO__Curriculum___59FA5E80");

                entity.HasOne(d => d.Subject).WithMany(p => p.Plos)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PLO__Subject_ID__5AEE82B9");
            });
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__8C4D3BBB0610810D");

                entity
                    .ToTable("Schedule")
                    .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("Schedule_History", "dbo");
                        ttb
                        .HasPeriodStart("CreatedAt")
                        .HasColumnName("CreatedAt");
                        ttb
                        .HasPeriodEnd("UpdatedAt")
                        .HasColumnName("UpdatedAt");
                    }));

                entity.Property(e => e.ScheduleId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("Schedule_ID");
                entity.Property(e => e.Clo)
                    .HasMaxLength(255)
                    .HasColumnName("CLO");
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

                entity.HasOne(d => d.Syllabus).WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.SyllabusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Schedule__Syllab__7B5B524B");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("Subjects_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.HasIndex(e => e.SubjectCode, "UQ__Subjects__4A7C5769EA153DC8")
                    .IsUnique();

                entity.Property(e => e.SubjectId)
                    .HasColumnName("Subject_ID")
                    .HasDefaultValueSql("(newid())");

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
            });

            modelBuilder.Entity<SubjectsSyllabus>(entity =>
            {
                entity.HasKey(e => new { e.SubjectId, e.SyllabusId });


                entity.ToTable("Subjects_Syllabus");

                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("Subjects_Syllabus_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.Property(e => e.SubjectId).HasColumnName("Subject_ID");

                entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");

                entity.HasOne(d => d.Subject)
                    .WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("FK__Subjects___Subje__60A75C0F");

                entity.HasOne(d => d.Syllabus)
                    .WithMany()
                    .HasForeignKey(d => d.SyllabusId)
                    .HasConstraintName("FK__Subjects___Sylla__619B8048");
            });

            modelBuilder.Entity<Syllabus>(entity =>
            {
                entity.ToTable("Syllabus");

                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("Syllabus_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.HasIndex(e => e.CourseCode, "UQ__Syllabus__1AE5B24D9C85777D").IsUnique();

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
            });

            modelBuilder.Entity<SyllabusLearningMaterial>(entity =>
            {
                entity.HasKey(e => new { e.SyllabusId, e.MaterialId });

                entity.ToTable("Syllabus_Learning_Materials");

                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("Syllabus_Learning_Materials_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.Property(e => e.MaterialId).HasColumnName("Material_ID");

                entity.Property(e => e.SyllabusId).HasColumnName("Syllabus_ID");

                entity.HasOne(d => d.Material)
                    .WithMany()
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("FK__Syllabus___Mater__6E01572D");

                entity.HasOne(d => d.Syllabus)
                    .WithMany()
                    .HasForeignKey(d => d.SyllabusId)
                    .HasConstraintName("FK__Syllabus___Sylla__6D0D32F4");
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
}
