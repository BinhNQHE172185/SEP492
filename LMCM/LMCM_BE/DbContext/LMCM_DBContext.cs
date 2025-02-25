using System;
using System.Collections.Generic;
using BusinessObject.Models;
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

        public virtual DbSet<AcceptanceRecord> AcceptanceRecords { get; set; } = null!;
        public virtual DbSet<BudgetProposal> BudgetProposals { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Curriculum> Curriculums { get; set; } = null!;
        public virtual DbSet<CurriculumsSubject> CurriculumsSubjects { get; set; } = null!;
        public virtual DbSet<DocumentTemplate> DocumentTemplates { get; set; } = null!;
        public virtual DbSet<LearningMaterial> LearningMaterials { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectsSyllabus> SubjectsSyllabi { get; set; } = null!;
        public virtual DbSet<Syllabus> Syllabi { get; set; } = null!;
        public virtual DbSet<SyllabusLearningMaterial> SyllabusLearningMaterials { get; set; } = null!;
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
                entity.ToTable(tb => tb.IsTemporal(ttb =>
    {
        ttb.UseHistoryTable("Curriculums_History", "dbo");
        ttb
            .HasPeriodStart("CreatedAt")
            .HasColumnName("CreatedAt");
        ttb
            .HasPeriodEnd("UpdatedAt")
            .HasColumnName("UpdatedAt");
    }
));

                entity.Property(e => e.CurriculumId)
                    .HasColumnName("Curriculum_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CurriculumName)
                    .HasMaxLength(255)
                    .HasColumnName("Curriculum_Name");

                entity.Property(e => e.Status).HasMaxLength(255);
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

                entity.HasIndex(e => e.SyllabusCode, "UQ__Syllabus__DAF08DF2B8572EA1")
                    .IsUnique();

                entity.Property(e => e.SyllabusId)
                    .HasColumnName("Syllabus_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.SyllabusCode)
                    .HasMaxLength(255)
                    .HasColumnName("Syllabus_Code");

                entity.Property(e => e.SyllabusName)
                    .HasMaxLength(255)
                    .HasColumnName("Syllabus_Name");
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
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
