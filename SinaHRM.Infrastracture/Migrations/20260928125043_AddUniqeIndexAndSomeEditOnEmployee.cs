using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqeIndexAndSomeEditOnEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                schema: "Basic",
                table: "Emploies",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "MaritalStatus",
                schema: "Basic",
                table: "Emploies",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "ManagementLayer",
                schema: "Basic",
                table: "Emploies",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Basic",
                table: "Emploies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_Email",
                schema: "Basic",
                table: "Emploies",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_NationalCode",
                schema: "Basic",
                table: "Emploies",
                column: "NationalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_PersonnelCode",
                schema: "Basic",
                table: "Emploies",
                column: "PersonnelCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Emploies_Email",
                schema: "Basic",
                table: "Emploies");

            migrationBuilder.DropIndex(
                name: "IX_Emploies_NationalCode",
                schema: "Basic",
                table: "Emploies");

            migrationBuilder.DropIndex(
                name: "IX_Emploies_PersonnelCode",
                schema: "Basic",
                table: "Emploies");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Basic",
                table: "Emploies");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                schema: "Basic",
                table: "Emploies",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaritalStatus",
                schema: "Basic",
                table: "Emploies",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "ManagementLayer",
                schema: "Basic",
                table: "Emploies",
                type: "int",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);
        }
    }
}
