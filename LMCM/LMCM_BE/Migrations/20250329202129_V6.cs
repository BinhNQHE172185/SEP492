using System;
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
            migrationBuilder.DropColumn(
                name: "Updated_At",
                table: "Contract_Value_Item");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated_At",
                table: "Contract_Value_Item",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "NULL");
        }
    }
}
