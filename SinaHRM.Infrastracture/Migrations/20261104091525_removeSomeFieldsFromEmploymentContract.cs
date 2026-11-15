using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class removeSomeFieldsFromEmploymentContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByEmployeeId",
                schema: "Contract",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "TemplateTextSnapshot",
                schema: "Contract",
                table: "EmploymentContracts");

            migrationBuilder.DropColumn(
                name: "UpdatedByEmployeeId",
                schema: "Contract",
                table: "EmploymentContracts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByEmployeeId",
                schema: "Contract",
                table: "EmploymentContracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateTextSnapshot",
                schema: "Contract",
                table: "EmploymentContracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByEmployeeId",
                schema: "Contract",
                table: "EmploymentContracts",
                type: "int",
                nullable: true);
        }
    }
}
