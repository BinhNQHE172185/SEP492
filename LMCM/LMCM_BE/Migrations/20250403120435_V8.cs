using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Learning___Mater__66603565",
                table: "Learning_Materials");

            migrationBuilder.DropForeignKey(
                name: "FK__Syllabus__Previo__5AEE82B9",
                table: "Syllabus");

            migrationBuilder.DropTable(
                name: "Learning_Material_Detail");

            migrationBuilder.DropIndex(
                name: "IX_Learning_Materials_Material_Detail_ID",
                table: "Learning_Materials");

            migrationBuilder.DropColumn(
                name: "Material_Detail_ID",
                table: "Learning_Materials");

            migrationBuilder.DropColumn(
                name: "Material_No",
                table: "Learning_Materials");

            migrationBuilder.RenameColumn(
                name: "Previous_Version_ID",
                table: "Syllabus",
                newName: "SyllabusId1");

            migrationBuilder.RenameIndex(
                name: "IX_Syllabus_Previous_Version_ID",
                table: "Syllabus",
                newName: "IX_Syllabus_SyllabusId1");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Learning_Materials",
                newName: "URL");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Learning_Materials",
                newName: "Updated_At");

            migrationBuilder.RenameColumn(
                name: "LearningType",
                table: "Learning_Materials",
                newName: "Learning_Type");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Learning_Materials",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "Material_Quantity",
                table: "Learning_Materials",
                newName: "Publisher");

            migrationBuilder.RenameColumn(
                name: "MaterialType",
                table: "Learning_Materials",
                newName: "ISBN");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Learning_Materials",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Edition",
                table: "Learning_Materials",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Published_Date",
                table: "Learning_Materials",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabus_Syllabus_SyllabusId1",
                table: "Syllabus",
                column: "SyllabusId1",
                principalTable: "Syllabus",
                principalColumn: "Syllabus_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabus_Syllabus_SyllabusId1",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Learning_Materials");

            migrationBuilder.DropColumn(
                name: "Edition",
                table: "Learning_Materials");

            migrationBuilder.DropColumn(
                name: "Published_Date",
                table: "Learning_Materials");

            migrationBuilder.RenameColumn(
                name: "SyllabusId1",
                table: "Syllabus",
                newName: "Previous_Version_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Syllabus_SyllabusId1",
                table: "Syllabus",
                newName: "IX_Syllabus_Previous_Version_ID");

            migrationBuilder.RenameColumn(
                name: "URL",
                table: "Learning_Materials",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Updated_At",
                table: "Learning_Materials",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Learning_Type",
                table: "Learning_Materials",
                newName: "LearningType");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "Learning_Materials",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Publisher",
                table: "Learning_Materials",
                newName: "Material_Quantity");

            migrationBuilder.RenameColumn(
                name: "ISBN",
                table: "Learning_Materials",
                newName: "MaterialType");

            migrationBuilder.AddColumn<Guid>(
                name: "Material_Detail_ID",
                table: "Learning_Materials",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Material_No",
                table: "Learning_Materials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Learning_Material_Detail",
                columns: table => new
                {
                    Material_Detail_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Author = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    Edition = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Material_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Material_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Published_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Publisher = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysutcdatetime())"),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Learning__700CBEE1C557F788", x => x.Material_Detail_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Materials_Material_Detail_ID",
                table: "Learning_Materials",
                column: "Material_Detail_ID");

            migrationBuilder.AddForeignKey(
                name: "FK__Learning___Mater__66603565",
                table: "Learning_Materials",
                column: "Material_Detail_ID",
                principalTable: "Learning_Material_Detail",
                principalColumn: "Material_Detail_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK__Syllabus__Previo__5AEE82B9",
                table: "Syllabus",
                column: "Previous_Version_ID",
                principalTable: "Syllabus",
                principalColumn: "Syllabus_ID");
        }
    }
}
