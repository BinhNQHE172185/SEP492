using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Syllabus__1AE5B24D3BC7ACD5",
                table: "Syllabus");

            migrationBuilder.DropIndex(
                name: "UQ__Subjects__4A7C5769891B3439",
                table: "Subjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UQ__Syllabus__1AE5B24D3BC7ACD5",
                table: "Syllabus",
                column: "Course_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Subjects__4A7C5769891B3439",
                table: "Subjects",
                column: "Subject_Code",
                unique: true);
        }
    }
}
