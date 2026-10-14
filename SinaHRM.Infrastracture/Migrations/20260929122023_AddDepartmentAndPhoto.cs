using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAndPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "DepartmentId",
                schema: "Basic",
                table: "Emploies",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePhotos",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePhotos_Emploies_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "Basic",
                        principalTable: "Emploies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_DepartmentId",
                schema: "Basic",
                table: "Emploies",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePhotos_EmployeeId",
                schema: "Basic",
                table: "EmployeePhotos",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emploies_Departments_DepartmentId",
                schema: "Basic",
                table: "Emploies",
                column: "DepartmentId",
                principalSchema: "Basic",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emploies_Departments_DepartmentId",
                schema: "Basic",
                table: "Emploies");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "EmployeePhotos",
                schema: "Basic");

            migrationBuilder.DropIndex(
                name: "IX_Emploies_DepartmentId",
                schema: "Basic",
                table: "Emploies");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "Basic",
                table: "Emploies");
        }
    }
}
