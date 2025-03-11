using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Learning___Mater__66603565",
                table: "Learning_Materials");

            migrationBuilder.AddForeignKey(
                name: "FK__Learning___Mater__66603565",
                table: "Learning_Materials",
                column: "Material_Detail_ID",
                principalTable: "Learning_Material_Detail",
                principalColumn: "Material_Detail_ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Learning___Mater__66603565",
                table: "Learning_Materials");

            migrationBuilder.AddForeignKey(
                name: "FK__Learning___Mater__66603565",
                table: "Learning_Materials",
                column: "Material_Detail_ID",
                principalTable: "Learning_Material_Detail",
                principalColumn: "Material_Detail_ID");
        }
    }
}
