using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Basic");

            migrationBuilder.CreateTable(
                name: "JobLocations",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobLocationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Port = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApearanceCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TitleCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AcountCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CEO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professions",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emploies",
                schema: "Basic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Age = table.Column<byte>(type: "tinyint", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PlaceBirth = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlaceOfIssuanceOfBirthCertificate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gneder = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EducationalQualification = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    MembershipId = table.Column<byte>(type: "tinyint", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    JobLocationId = table.Column<byte>(type: "tinyint", nullable: false),
                    ManagementLayer = table.Column<int>(type: "int", nullable: true),
                    MaritalStatus = table.Column<int>(type: "int", nullable: false),
                    NumberOfChildren = table.Column<byte>(type: "tinyint", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ResponsibilitySalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YearsBase = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HousingBonous = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WorkerBonous = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MartialBonous = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChildrenBonous = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasDependents = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    createdById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emploies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emploies_JobLocations_JobLocationId",
                        column: x => x.JobLocationId,
                        principalSchema: "Basic",
                        principalTable: "JobLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emploies_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalSchema: "Basic",
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emploies_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalSchema: "Basic",
                        principalTable: "Professions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_JobLocationId",
                schema: "Basic",
                table: "Emploies",
                column: "JobLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_MembershipId",
                schema: "Basic",
                table: "Emploies",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Emploies_ProfessionId",
                schema: "Basic",
                table: "Emploies",
                column: "ProfessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emploies",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "JobLocations",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "Memberships",
                schema: "Basic");

            migrationBuilder.DropTable(
                name: "Professions",
                schema: "Basic");
        }
    }
}
