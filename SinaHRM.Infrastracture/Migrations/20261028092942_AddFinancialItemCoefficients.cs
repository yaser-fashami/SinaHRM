using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialItemCoefficients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialItemCoefficients",
                schema: "Contract",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiscalYear = table.Column<string>(type: "char(4)", unicode: false, fixedLength: true, maxLength: 4, nullable: false),
                    Version = table.Column<short>(type: "smallint", nullable: false),
                    FinancialItemDefinitionId = table.Column<byte>(type: "tinyint", nullable: false),
                    BaseAmount = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    IncreaseFactor = table.Column<decimal>(type: "decimal(8,6)", precision: 8, scale: 6, nullable: false),
                    ConstantIncrease = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialItemCoefficients", x => x.Id);
                    table.CheckConstraint("CK_FinancialItemCoefficients_IncreaseFactor", "[IncreaseFactor] > 0");
                    table.ForeignKey(
                        name: "FK_FinancialItemCoefficients_FinancialItemDefinitions_FinancialItemDefinitionId",
                        column: x => x.FinancialItemDefinitionId,
                        principalSchema: "Contract",
                        principalTable: "FinancialItemDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialItemCoefficients_FinancialItemDefinitionId_FiscalYear_Version",
                schema: "Contract",
                table: "FinancialItemCoefficients",
                columns: new[] { "FinancialItemDefinitionId", "FiscalYear", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialItemCoefficients",
                schema: "Contract");
        }
    }
}
