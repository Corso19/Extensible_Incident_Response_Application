using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentResponseAPI.Migrations
{
    /// <inheritdoc />
    public partial class LinkedIncidentsToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_EventId",
                table: "Incidents",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Events_EventId",
                table: "Incidents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Events_EventId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_EventId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Incidents");
        }
    }
}
