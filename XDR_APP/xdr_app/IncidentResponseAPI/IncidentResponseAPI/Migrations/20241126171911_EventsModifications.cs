using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentResponseAPI.Migrations
{
    /// <inheritdoc />
    public partial class EventsModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDataJson",
                table: "Events",
                newName: "TypeName");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "Events",
                newName: "EventDataJson");
        }
    }
}
