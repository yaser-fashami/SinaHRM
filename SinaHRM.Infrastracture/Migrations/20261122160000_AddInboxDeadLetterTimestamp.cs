using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SinaHRM.Infrastracture;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations;

[DbContext(typeof(SinaHRMDbContext))]
[Migration("20261122160000_AddInboxDeadLetterTimestamp")]
public sealed class AddInboxDeadLetterTimestamp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "DeadLetterAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "datetimeoffset",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DeadLetterAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages");
    }
}
