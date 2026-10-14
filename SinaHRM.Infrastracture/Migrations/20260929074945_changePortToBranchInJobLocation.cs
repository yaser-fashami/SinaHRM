using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class changePortToBranchInJobLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Port",
                schema: "Basic",
                table: "JobLocations",
                newName: "Branch");

            migrationBuilder.AlterColumn<string>(
                name: "JobName",
                schema: "Basic",
                table: "Professions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Branch",
                schema: "Basic",
                table: "JobLocations",
                newName: "Port");

            migrationBuilder.AlterColumn<string>(
                name: "JobName",
                schema: "Basic",
                table: "Professions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

        }
    }
}
