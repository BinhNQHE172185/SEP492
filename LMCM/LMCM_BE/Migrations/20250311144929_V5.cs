using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Learning_Materials",
                newName: "MaterialType");

            migrationBuilder.AddColumn<string>(
                name: "LearningType",
                table: "Learning_Materials",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearningType",
                table: "Learning_Materials");

            migrationBuilder.RenameColumn(
                name: "MaterialType",
                table: "Learning_Materials",
                newName: "Type");
        }
    }
}
