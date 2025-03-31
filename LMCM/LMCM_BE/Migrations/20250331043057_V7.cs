using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMCM_BE.Migrations
{
    /// <inheritdoc />
    public partial class V7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningMaterialChangesHistory_NewMaterial",
                table: "Learning_Material_Changes_History");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningMaterialChangesHistory_OldMaterial",
                table: "Learning_Material_Changes_History");

            migrationBuilder.DropIndex(
                name: "IX_Learning_Material_Changes_History_Old_Material_ID",
                table: "Learning_Material_Changes_History");

            migrationBuilder.DropColumn(
                name: "Old_Material_ID",
                table: "Learning_Material_Changes_History");

            migrationBuilder.RenameColumn(
                name: "New_Material_ID",
                table: "Learning_Material_Changes_History",
                newName: "Syllabus_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Learning_Material_Changes_History_New_Material_ID",
                table: "Learning_Material_Changes_History",
                newName: "IX_Learning_Material_Changes_History_Syllabus_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningMaterialChangesHistory_Syllabus",
                table: "Learning_Material_Changes_History",
                column: "Syllabus_ID",
                principalTable: "Syllabus",
                principalColumn: "Syllabus_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningMaterialChangesHistory_Syllabus",
                table: "Learning_Material_Changes_History");

            migrationBuilder.RenameColumn(
                name: "Syllabus_ID",
                table: "Learning_Material_Changes_History",
                newName: "New_Material_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Learning_Material_Changes_History_Syllabus_ID",
                table: "Learning_Material_Changes_History",
                newName: "IX_Learning_Material_Changes_History_New_Material_ID");

            migrationBuilder.AddColumn<Guid>(
                name: "Old_Material_ID",
                table: "Learning_Material_Changes_History",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
