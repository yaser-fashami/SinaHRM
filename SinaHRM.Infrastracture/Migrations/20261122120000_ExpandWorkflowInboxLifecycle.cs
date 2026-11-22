using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SinaHRM.Infrastracture;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations;

[DbContext(typeof(SinaHRMDbContext))]
[Migration("20261122120000_ExpandWorkflowInboxLifecycle")]
public partial class ExpandWorkflowInboxLifecycle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "AttemptCount",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "NextAttemptAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "datetimeoffset",
            nullable: false,
            defaultValueSql: "SYSUTCDATETIME()");

        migrationBuilder.AddColumn<Guid>(
            name: "LeaseToken",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "LeaseExpiresAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "LastAttemptAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LastError",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "nvarchar(4000)",
            maxLength: 4000,
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "ProcessedAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_WorkflowInboxMessages_Status_NextAttemptAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            columns: new[] { "Status", "NextAttemptAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_WorkflowInboxMessages_Status_LeaseExpiresAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            columns: new[] { "Status", "LeaseExpiresAtUtc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_WorkflowInboxMessages_Status_NextAttemptAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages");

        migrationBuilder.DropIndex(
            name: "IX_WorkflowInboxMessages_Status_LeaseExpiresAtUtc",
            schema: "Integration",
            table: "WorkflowInboxMessages");

        migrationBuilder.DropColumn(name: "AttemptCount", schema: "Integration", table: "WorkflowInboxMessages");
        migrationBuilder.DropColumn(name: "NextAttemptAtUtc", schema: "Integration", table: "WorkflowInboxMessages");
        migrationBuilder.DropColumn(name: "LeaseToken", schema: "Integration", table: "WorkflowInboxMessages");
        migrationBuilder.DropColumn(name: "LeaseExpiresAtUtc", schema: "Integration", table: "WorkflowInboxMessages");
        migrationBuilder.DropColumn(name: "LastAttemptAtUtc", schema: "Integration", table: "WorkflowInboxMessages");
        migrationBuilder.DropColumn(name: "LastError", schema: "Integration", table: "WorkflowInboxMessages");
        migrationBuilder.DropColumn(name: "ProcessedAtUtc", schema: "Integration", table: "WorkflowInboxMessages");
    }
}
