using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinaHRM.Infrastracture.Data.Sql.Migrations;

[DbContext(typeof(SinaHRMDbContext))]
[Migration("20261117130000_AddWorkflowInbox")]
public partial class AddWorkflowInbox : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "Integration");

        migrationBuilder.CreateTable(
            name: "WorkflowInboxMessages",
            schema: "Integration",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ReceivedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkflowInboxMessages", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_WorkflowInboxMessages_EventId",
            schema: "Integration",
            table: "WorkflowInboxMessages",
            column: "EventId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WorkflowInboxMessages",
            schema: "Integration");
    }
}
