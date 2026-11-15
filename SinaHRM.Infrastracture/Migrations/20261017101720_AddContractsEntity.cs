using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddContractsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Contract");

            migrationBuilder.CreateTable(
                name: "EmploymentContractTemplates",
                schema: "Contract",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentContractTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialItemDefinitions",
                schema: "Contract",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AllowManualOverride = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialItemDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentContractTemplateVersions",
                schema: "Contract",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionNumber = table.Column<short>(type: "smallint", nullable: false),
                    TemplateText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    EmploymentContractTemplateId = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentContractTemplateVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentContractTemplateVersions_EmploymentContractTemplates_EmploymentContractTemplateId",
                        column: x => x.EmploymentContractTemplateId,
                        principalSchema: "Contract",
                        principalTable: "EmploymentContractTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentContracts",
                schema: "Contract",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmploymentContractTemplateVersionId = table.Column<short>(type: "smallint", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TemplateTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkflowInstanceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SignatureCompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActivatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancelledAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    UpdatedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentContracts_Emploies_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Basic",
                        principalTable: "Emploies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmploymentContracts_EmploymentContractTemplateVersions_EmploymentContractTemplateVersionId",
                        column: x => x.EmploymentContractTemplateVersionId,
                        principalSchema: "Contract",
                        principalTable: "EmploymentContractTemplateVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentContractFinancialItems",
                schema: "Contract",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmploymentContractId = table.Column<int>(type: "int", nullable: false),
                    FinancialItemDefinitionId = table.Column<byte>(type: "tinyint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CalculatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsManuallyOverridden = table.Column<bool>(type: "bit", nullable: false),
                    OverrideReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    UpdatedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentContractFinancialItems", x => x.Id);
                    table.CheckConstraint("CK_EmploymentContractFinancialItems_CalculatedAmount_NonNegative", "[CalculatedAmount] >= 0");
                    table.CheckConstraint("CK_EmploymentContractFinancialItems_FinalAmount_NonNegative", "[FinalAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_EmploymentContractFinancialItems_EmploymentContracts_EmploymentContractId",
                        column: x => x.EmploymentContractId,
                        principalSchema: "Contract",
                        principalTable: "EmploymentContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmploymentContractFinancialItems_FinancialItemDefinitions_FinancialItemDefinitionId",
                        column: x => x.FinancialItemDefinitionId,
                        principalSchema: "Contract",
                        principalTable: "FinancialItemDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContractFinancialItems_EmploymentContractId_DisplayOrder",
                schema: "Contract",
                table: "EmploymentContractFinancialItems",
                columns: new[] { "EmploymentContractId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContractFinancialItems_FinancialItemDefinitionId",
                schema: "Contract",
                table: "EmploymentContractFinancialItems",
                column: "FinancialItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_ContractNumber",
                schema: "Contract",
                table: "EmploymentContracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_EmployeeId_StartDate",
                schema: "Contract",
                table: "EmploymentContracts",
                columns: new[] { "EmployeeId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_EmployeeId_Status",
                schema: "Contract",
                table: "EmploymentContracts",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_EmploymentContractTemplateVersionId",
                schema: "Contract",
                table: "EmploymentContracts",
                column: "EmploymentContractTemplateVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContracts_WorkflowInstanceId",
                schema: "Contract",
                table: "EmploymentContracts",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContractTemplates_Code",
                schema: "Contract",
                table: "EmploymentContractTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentContractTemplateVersions_EmploymentContractTemplateId_VersionNumber",
                schema: "Contract",
                table: "EmploymentContractTemplateVersions",
                columns: new[] { "EmploymentContractTemplateId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialItemDefinitions_Code",
                schema: "Contract",
                table: "FinancialItemDefinitions",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmploymentContractFinancialItems",
                schema: "Contract");

            migrationBuilder.DropTable(
                name: "EmploymentContracts",
                schema: "Contract");

            migrationBuilder.DropTable(
                name: "FinancialItemDefinitions",
                schema: "Contract");

            migrationBuilder.DropTable(
                name: "EmploymentContractTemplateVersions",
                schema: "Contract");

            migrationBuilder.DropTable(
                name: "EmploymentContractTemplates",
                schema: "Contract");
        }
    }
}
