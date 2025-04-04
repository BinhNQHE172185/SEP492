using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Learning_Material_Type",
                table: "Learning_Material_Changes_History");

            migrationBuilder.RenameColumn(
                name: "MaterialType",
                table: "Learning_Materials",
                newName: "Material_Type");

            migrationBuilder.AlterColumn<string>(
                name: "Material_Type",
                table: "Learning_Materials",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Material_Type",
                table: "Learning_Materials",
                newName: "MaterialType");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialType",
                table: "Learning_Materials",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Learning_Material_Type",
                table: "Learning_Material_Changes_History",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
