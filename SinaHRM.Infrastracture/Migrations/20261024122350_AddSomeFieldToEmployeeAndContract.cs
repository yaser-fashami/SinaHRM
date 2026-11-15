using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeFieldToEmployeeAndContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CEOMobile",
                schema: "Basic",
                table: "Memberships",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthCertificateNumber",
                schema: "Basic",
                table: "Emploies",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CEOMobile",
                schema: "Basic",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "BirthCertificateNumber",
                schema: "Basic",
                table: "Emploies");
        }
    }
}
