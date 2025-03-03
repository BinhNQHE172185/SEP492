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
                name: "Contractor",
                columns: table => new
                {
                    Contractor_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Contractor_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Tax_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Employee_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ID_Card_Number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ID_Issued_Place = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Bank_Account_Number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Bank_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contract__61C4678DDD41C6C5", x => x.Contractor_ID);
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Curricul__2F88E2C279D7E077", x => x.Curriculum_ID);
                });

            migrationBuilder.CreateTable(
                name: "Referenced_Learning_Materials",
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Referenc__3A09B0FD6A28AF49", x => x.Material_ID);
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subjects__D98F54D68388CEB8", x => x.Subject_ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                name: "PLO",
                columns: table => new
                {
                    PLO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Curriculum_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PLO_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PLO_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PLO__3BEF007E3D145C77", x => x.PLO_ID);
                    table.ForeignKey(
                        name: "FK__PLO__Curriculum___5165187F",
                        column: x => x.Curriculum_ID,
                        principalTable: "Curriculums",
                        principalColumn: "Curriculum_ID");
                });

            migrationBuilder.CreateTable(
                name: "Curriculums_Subjects",
                columns: table => new
                {
                    Curriculum_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Term_No = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<int>(type: "int", nullable: true),
                    Options = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Curricul__39754AB27598164C", x => new { x.Curriculum_ID, x.Subject_ID, x.CreatedAt });
                    table.ForeignKey(
                        name: "FK__Curriculu__Curri__4AB81AF0",
                        column: x => x.Curriculum_ID,
                        principalTable: "Curriculums",
                        principalColumn: "Curriculum_ID");
                    table.ForeignKey(
                        name: "FK__Curriculu__Subje__4BAC3F29",
                        column: x => x.Subject_ID,
                        principalTable: "Subjects",
                        principalColumn: "Subject_ID");
                });

            migrationBuilder.CreateTable(
                name: "Syllabus",
                columns: table => new
                {
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Previous_Version_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Program_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Decision_No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Course_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Course_Name_English = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Course_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Learning_Teaching_Method = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    No_of_Credits = table.Column<int>(type: "int", nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Syllabus__2F9B495028311941", x => x.Syllabus_ID);
                    table.ForeignKey(
                        name: "FK__Syllabus__Previo__5EBF139D",
                        column: x => x.Previous_Version_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                    table.ForeignKey(
                        name: "FK__Syllabus__Subjec__5DCAEF64",
                        column: x => x.Subject_ID,
                        principalTable: "Subjects",
                        principalColumn: "Subject_ID");
                });

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
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Proposal_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Budget_P__C9D0461F515BE8A9", x => x.Proposal_ID);
                    table.ForeignKey(
                        name: "FK__Budget_Pr__Autho__151B244E",
                        column: x => x.Author_ID,
                        principalTable: "User",
                        principalColumn: "Id");
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Document__E7FB8F01F3346854", x => x.Template_ID);
                    table.ForeignKey(
                        name: "FK__Document___Updat__29221CFB",
                        column: x => x.Author_ID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Notification_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    User_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__8C1160B52FF047B0", x => x.Notification_ID);
                    table.ForeignKey(
                        name: "FK__Notificat__User___0A9D95DB",
                        column: x => x.User_ID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    User_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Item_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permissi__3214EC079928F72B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Permissio__User___3B75D760",
                        column: x => x.User_ID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PLO_Subjects",
                columns: table => new
                {
                    PLO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PLO_Subj__2D12A80EB49AFFDC", x => new { x.PLO_ID, x.Subject_ID, x.CreatedAt });
                    table.ForeignKey(
                        name: "FK__PLO_Subje__PLO_I__5629CD9C",
                        column: x => x.PLO_ID,
                        principalTable: "PLO",
                        principalColumn: "PLO_ID");
                    table.ForeignKey(
                        name: "FK__PLO_Subje__Subje__571DF1D5",
                        column: x => x.Subject_ID,
                        principalTable: "Subjects",
                        principalColumn: "Subject_ID");
                });

            migrationBuilder.CreateTable(
                name: "CLO",
                columns: table => new
                {
                    CLO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CLO_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CLO_Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CLO__C3755EAD29554F84", x => x.CLO_ID);
                    table.ForeignKey(
                        name: "FK__CLO__Syllabus_ID__00200768",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                });

            migrationBuilder.CreateTable(
                name: "Constructivist_Question",
                columns: table => new
                {
                    Question_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Session_No = table.Column<int>(type: "int", nullable: false),
                    Question_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Question_Detail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Construc__B0B2E4C6488BFAB9", x => x.Question_ID);
                    table.ForeignKey(
                        name: "FK__Construct__Sylla__7A672E12",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                });

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
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Grading___71D721C6DB6130A4", x => x.Structure_ID);
                    table.ForeignKey(
                        name: "FK__Grading_S__Sylla__74AE54BC",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                });

            migrationBuilder.CreateTable(
                name: "Imported_Learning_Materials",
                columns: table => new
                {
                    Material_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Material_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Material_Quantity = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Material_No = table.Column<int>(type: "int", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Imported__3A09B0FD89507A0F", x => x.Material_ID);
                    table.ForeignKey(
                        name: "FK__Imported___Sylla__05D8E0BE",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                });

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
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__8C4D3BBBF298C27A", x => x.Schedule_ID);
                    table.ForeignKey(
                        name: "FK__Schedule__Syllab__6EF57B66",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                });

            migrationBuilder.CreateTable(
                name: "Syllabus_Referenced_Learning_Materials",
                columns: table => new
                {
                    Syllabus_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Material_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Syllabus__875E8F626D76336E", x => new { x.Syllabus_ID, x.Material_ID, x.CreatedAt });
                    table.ForeignKey(
                        name: "FK__Syllabus___Mater__693CA210",
                        column: x => x.Material_ID,
                        principalTable: "Referenced_Learning_Materials",
                        principalColumn: "Material_ID");
                    table.ForeignKey(
                        name: "FK__Syllabus___Sylla__68487DD7",
                        column: x => x.Syllabus_ID,
                        principalTable: "Syllabus",
                        principalColumn: "Syllabus_ID");
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Contract_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Proposal_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Author_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contractor_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Contract_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Contract_Value = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Contract__5E2E73DAEDA2E168", x => x.Contract_ID);
                    table.ForeignKey(
                        name: "FK__Contracts__Autho__1BC821DD",
                        column: x => x.Author_ID,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Contracts__Contr__1CBC4616",
                        column: x => x.Contractor_ID,
                        principalTable: "Contractor",
                        principalColumn: "Contractor_ID");
                    table.ForeignKey(
                        name: "FK__Contracts__Propo__1AD3FDA4",
                        column: x => x.Proposal_ID,
                        principalTable: "Budget_Proposals",
                        principalColumn: "Proposal_ID");
                });

            migrationBuilder.CreateTable(
                name: "Acceptance_Record",
                columns: table => new
                {
                    Acceptance_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Author_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contract_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Acceptance_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Final_Price = table.Column<decimal>(type: "decimal(19,2)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Acceptan__FC008882157AA898", x => x.Acceptance_ID);
                    table.ForeignKey(
                        name: "FK__Acceptanc__Autho__22751F6C",
                        column: x => x.Author_ID,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Acceptanc__Contr__236943A5",
                        column: x => x.Contract_ID,
                        principalTable: "Contracts",
                        principalColumn: "Contract_ID");
                });

            migrationBuilder.CreateTable(
                name: "Learning_Material_Changes_History",
                columns: table => new
                {
                    History_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    User_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contract_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    New_Material_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Old_Material_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Learning_Material_Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Change_Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Change_Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Completion_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Start_Term = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Course_Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Learning__A6BABA377EF62FFA", x => x.History_ID);
                    table.ForeignKey(
                        name: "FK__Learning___Contr__2DE6D218",
                        column: x => x.Contract_ID,
                        principalTable: "Contracts",
                        principalColumn: "Contract_ID");
                    table.ForeignKey(
                        name: "FK__Learning___User___2CF2ADDF",
                        column: x => x.User_ID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acceptance_Record_Author_ID",
                table: "Acceptance_Record",
                column: "Author_ID");

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
                name: "IX_Contracts_Author_ID",
                table: "Contracts",
                column: "Author_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Contractor_ID",
                table: "Contracts",
                column: "Contractor_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Proposal_ID",
                table: "Contracts",
                column: "Proposal_ID");

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
                name: "IX_Imported_Learning_Materials_Syllabus_ID",
                table: "Imported_Learning_Materials",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Material_Changes_History_Contract_ID",
                table: "Learning_Material_Changes_History",
                column: "Contract_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Material_Changes_History_User_ID",
                table: "Learning_Material_Changes_History",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_User_ID",
                table: "Notification",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_User_ID",
                table: "Permissions",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PLO_Curriculum_ID",
                table: "PLO",
                column: "Curriculum_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PLO_Subjects_Subject_ID",
                table: "PLO_Subjects",
                column: "Subject_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_Syllabus_ID",
                table: "Schedule",
                column: "Syllabus_ID");

            migrationBuilder.CreateIndex(
                name: "UQ__Subjects__4A7C5769222B0F9A",
                table: "Subjects",
                column: "Subject_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_Previous_Version_ID",
                table: "Syllabus",
                column: "Previous_Version_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_Subject_ID",
                table: "Syllabus",
                column: "Subject_ID");

            migrationBuilder.CreateIndex(
                name: "UQ__Syllabus__1AE5B24D4FFC6ACA",
                table: "Syllabus",
                column: "Course_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_Referenced_Learning_Materials_Material_ID",
                table: "Syllabus_Referenced_Learning_Materials",
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
                name: "CLO");

            migrationBuilder.DropTable(
                name: "Constructivist_Question");

            migrationBuilder.DropTable(
                name: "Curriculums_Subjects");

            migrationBuilder.DropTable(
                name: "Document_Templates");

            migrationBuilder.DropTable(
                name: "Grading_Structure");

            migrationBuilder.DropTable(
                name: "Imported_Learning_Materials");

            migrationBuilder.DropTable(
                name: "Learning_Material_Changes_History");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "PLO_Subjects");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Syllabus_Referenced_Learning_Materials");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "PLO");

            migrationBuilder.DropTable(
                name: "Referenced_Learning_Materials");

            migrationBuilder.DropTable(
                name: "Syllabus");

            migrationBuilder.DropTable(
                name: "Contractor");

            migrationBuilder.DropTable(
                name: "Budget_Proposals");

            migrationBuilder.DropTable(
                name: "Curriculums");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
