using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabus_Syllabus_SyllabusId1",
                table: "Syllabus");

            migrationBuilder.DropIndex(
                name: "IX_Syllabus_SyllabusId1",
                table: "Syllabus");

            migrationBuilder.DropColumn(
                name: "SyllabusId1",
                table: "Syllabus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SyllabusId1",
                table: "Syllabus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Syllabus_SyllabusId1",
                table: "Syllabus",
                column: "SyllabusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabus_Syllabus_SyllabusId1",
                table: "Syllabus",
                column: "SyllabusId1",
                principalTable: "Syllabus",
                principalColumn: "Syllabus_ID");
        }
    }
}
