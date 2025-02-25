using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Curriculums",
                columns: table => new
                {
                    Curriculum_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Curriculum_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Curriculum_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Curriculum_Name_English = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Curriculum_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Vocational_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Vocational_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    English_Vocational_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Decision_No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Approved_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Curricul__2F88E2C21CA8612E", x => x.Curriculum_ID);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Curriculums_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Learning_Materials",
                columns: table => new
                {
                    Material_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Material_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Material_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Author = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Publisher = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Published_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edition = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Learning__3A09B0FD884B7FA8", x => x.Material_ID);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LearningMaterials_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Item_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Subject_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subject_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subject_Name_English = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    isConstructivist = table.Column<bool>(type: "bit", nullable: true),
                    Method = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    Reality = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Subject_ID);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Subjects_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Syllabus",
                columns: table => new
                {
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Program_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Decision_No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Course_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Course_Name_English = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Course_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Learning_Teaching_Method = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    No_of_Credits = table.Column<int>(type: "int", nullable: false),
                    Degree_Level = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Time_Allocation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Pre_requisite = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Student_Task = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Tools = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Min_GPA_to_Pass = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    Scoring_Scale = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    Approved_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllabus", x => x.Syllabus_ID);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Syllabus_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Curriculums_Subjects",
                columns: table => new
                {
                    Curriculum_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Term_No = table.Column<int>(type: "int", nullable: false),
                    Credit = table.Column<int>(type: "int", nullable: false),
                    Options = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curriculums_Subjects", x => new { x.Curriculum_ID, x.Subject_ID });
                    table.ForeignKey(
                        name: "FK__Curriculu__Curri__534D60F1",
                        column: x => x.Curriculum_ID,
                        principalTable: "Curriculums",
                        principalColumn: "Curriculum_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Curriculu__Subje__5441852A",
                        column: x => x.Subject_ID,
                        principalTable: "Subjects",
                        principalColumn: "Subject_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Curriculums_Subjects_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "PLO",
                columns: table => new
                {
                    PLO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Curriculum_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PLO_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PLO_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PLO__3BEF007EC7EFD1E5", x => x.PLO_ID);
                    table.ForeignKey(
                        name: "FK__PLO__Curriculum___59FA5E80",
                        column: x => x.Curriculum_ID,
                        principalTable: "Curriculums",
                        principalColumn: "Curriculum_ID");
                    table.ForeignKey(
                        name: "FK__PLO__Subject_ID__5AEE82B9",
                        column: x => x.Subject_ID,
                        principalTable: "Subjects",
                        principalColumn: "Subject_ID");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PLO_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "CLO",
                columns: table => new
                {
                    CLO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CLO_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CLO_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CLO__C3755EAD5A32F777", x => x.CLO_ID);
                    table.ForeignKey(
                        name: "FK__CLO__Syllabus_ID__0F624AF8",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CLO_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Constructivist_Question",
                columns: table => new
                {
                    Question_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Session_No = table.Column<int>(type: "int", nullable: false),
                    Question_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Question_Detail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Construc__B0B2E4C62AEA54F9", x => x.Question_ID);
                    table.ForeignKey(
                        name: "FK__Construct__Sylla__08B54D69",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Constructivist_Question_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Grading_Structure",
                columns: table => new
                {
                    Structure_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Structure_No = table.Column<int>(type: "int", nullable: false),
                    Assessment_Component = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Assessment_Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    Part = table.Column<int>(type: "int", nullable: true),
                    Min_Value = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CLO = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Question_Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Question_No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    How = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Session_No = table.Column<int>(type: "int", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grading___71D721C6B560AAF0", x => x.Structure_ID);
                    table.ForeignKey(
                        name: "FK__Grading_S__Sylla__02084FDA",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Grading_Structure_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Schedule_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Schedule_No = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CLO = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ITU = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Student_Material = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Student_Task = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Student_Material_Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Lecturer_Material = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Lecturer_Task = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Lecturer_Material_Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__8C4D3BBB0610810D", x => x.Schedule_ID);
                    table.ForeignKey(
                        name: "FK__Schedule__Syllab__7B5B524B",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Schedule_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Subjects_Syllabus",
                columns: table => new
                {
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects_Syllabus", x => new { x.Subject_ID, x.Syllabus_ID });
                    table.ForeignKey(
                        name: "FK__Subjects___Subje__60A75C0F",
                        column: x => x.Subject_ID,
                        principalTable: "Subjects",
                        principalColumn: "Subject_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Subjects___Sylla__619B8048",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Subjects_Syllabus_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "Syllabus_Learning_Materials",
                columns: table => new
                {
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Material_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllabus_Learning_Materials", x => new { x.Syllabus_ID, x.Material_ID });
                    table.ForeignKey(
                        name: "FK__Syllabus___Mater__6E01572D",
                        column: x => x.Material_ID,
                        principalTable: "Learning_Materials",
                        principalColumn: "Material_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Syllabus___Sylla__6D0D32F4",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Syllabus_Learning_Materials_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Budget_Proposals",
                columns: table => new
                {
                    Proposal_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Author_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Proposal_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Budget_P__C9D0461F9781A89B", x => x.Proposal_ID);
                    table.ForeignKey(
                        name: "FK__Budget_Pr__Autho__7F2BE32F",
                        column: x => x.Author_ID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Contract_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Contract_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Contractor_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contract_Value = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Contract_ID);
                    table.ForeignKey(
                        name: "FK__Contracts__Contr__797309D9",
                        column: x => x.Contractor_ID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Document_Templates",
                columns: table => new
                {
                    Template_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Template_Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Template_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Author_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Document__E7FB8F012F99C312", x => x.Template_ID);
                    table.ForeignKey(
                        name: "FK__Document___Updat__0A9D95DB",
                        column: x => x.Author_ID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Notification_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    User_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Notification_ID);
                    table.ForeignKey(
                        name: "FK__Notificat__User___73BA3083",
                        column: x => x.User_ID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Acceptance_Record",
                columns: table => new
                {
                    Acceptance_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Contract_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acceptance_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Final_Price = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Acceptan__FC008882EB6B5A19", x => x.Acceptance_ID);
                    table.ForeignKey(
                        name: "FK__Acceptanc__Contr__04E4BC85",
                        column: x => x.Contract_ID,
                        principalTable: "Contracts",
                        principalColumn: "Contract_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acceptance_Record_Contract_ID",
                table: "Acceptance_Record",
                column: "Contract_ID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_Proposals_Author_ID",
                table: "Budget_Proposals",
                column: "Author_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CLO_Syllabus_ID",
                table: "CLO",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Constructivist_Question_Syllabus_ID",
                table: "Constructivist_Question",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Contractor_ID",
                table: "Contracts",
                column: "Contractor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculums_Subjects_Subject_ID",
                table: "Curriculums_Subjects",
                column: "Subject_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Document_Templates_Author_ID",
                table: "Document_Templates",
                column: "Author_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Grading_Structure_Syllabus_ID",
                table: "Grading_Structure",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_User_ID",
                table: "Notification",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PLO_Curriculum_ID",
                table: "PLO",
                column: "Curriculum_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PLO_Subject_ID",
                table: "PLO",
                column: "Subject_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Syllabus_ID",
                table: "Schedule",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "UQ__Subjects__4A7C5769EA153DC8",
                table: "Subjects",
                column: "Subject_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Syllabus_Syllabus_ID",
                table: "Subjects_Syllabus",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "UQ__Syllabus__1AE5B24D9C85777D",
                table: "Syllabus",
                column: "Course_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_Learning_Materials_Material_ID",
                table: "Syllabus_Learning_Materials",
                column: "Material_ID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acceptance_Record");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Budget_Proposals");

            migrationBuilder.DropTable(
                name: "CLO")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CLO_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Constructivist_Question")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Constructivist_Question_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Curriculums_Subjects")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Curriculums_Subjects_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Document_Templates");

            migrationBuilder.DropTable(
                name: "Grading_Structure")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Grading_Structure_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "PLO")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PLO_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Schedule")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Schedule_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Subjects_Syllabus")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Subjects_Syllabus_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Syllabus_Learning_Materials")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Syllabus_Learning_Materials_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Curriculums")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Curriculums_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Subjects")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Subjects_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Learning_Materials")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LearningMaterials_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "Syllabus")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Syllabus_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "UpdatedAt")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "CreatedAt");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
