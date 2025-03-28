using System;
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
            migrationBuilder.CreateTable(
                name: "Contract_Value_Item",
                columns: table => new
                {
                    Value_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Measurement_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Standard_Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quality_Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value_Ratio_For_Update = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Contract_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "NULL")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ContractValueItem", x => x.Value_ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contract_Value_Item");
        }
    }
}
