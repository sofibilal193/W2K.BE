using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFI.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EventLog_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "events");

            migrationBuilder.CreateTable(
                name: "EventLogs",
                schema: "events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                                    .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OfficeId = table.Column<int>(type: "int", nullable: true),
                    RecordId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_OfficeId",
                schema: "events",
                table: "EventLogs",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_RecordId",
                schema: "events",
                table: "EventLogs",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_Type",
                schema: "events",
                table: "EventLogs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_UserId",
                schema: "events",
                table: "EventLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLogs",
                schema: "events");
        }
    }
}
