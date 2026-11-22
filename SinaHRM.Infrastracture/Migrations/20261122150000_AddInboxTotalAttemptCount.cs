using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SinaHRM.Infrastracture;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations;

[DbContext(typeof(SinaHRMDbContext))]
[Migration("20261122150000_AddInboxTotalAttemptCount")]
public partial class AddInboxTotalAttemptCount : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "TotalAttemptCount",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TotalAttemptCount",
            schema: "Integration",
            table: "WorkflowInboxMessages");
    }
}
