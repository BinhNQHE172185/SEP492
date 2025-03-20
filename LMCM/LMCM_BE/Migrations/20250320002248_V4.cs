using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Learning_Material_Changes_History_New_Material_ID",
                table: "Learning_Material_Changes_History",
                column: "New_Material_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Learning_Material_Changes_History_Old_Material_ID",
                table: "Learning_Material_Changes_History",
                column: "Old_Material_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningMaterialChangesHistory_NewMaterial",
                table: "Learning_Material_Changes_History",
                column: "New_Material_ID",
                principalTable: "Learning_Materials",
                principalColumn: "Material_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningMaterialChangesHistory_OldMaterial",
                table: "Learning_Material_Changes_History",
                column: "Old_Material_ID",
                principalTable: "Learning_Materials",
                principalColumn: "Material_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningMaterialChangesHistory_NewMaterial",
                table: "Learning_Material_Changes_History");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningMaterialChangesHistory_OldMaterial",
                table: "Learning_Material_Changes_History");

            migrationBuilder.DropIndex(
                name: "IX_Learning_Material_Changes_History_New_Material_ID",
                table: "Learning_Material_Changes_History");

            migrationBuilder.DropIndex(
                name: "IX_Learning_Material_Changes_History_Old_Material_ID",
                table: "Learning_Material_Changes_History");
        }
    }
}
