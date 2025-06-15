using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentResponseAPI.Migrations
{
    /// <inheritdoc />
    public partial class SensorTableChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConfigurationJson",
                table: "Sensors",
                newName: "TenantId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationId",
                table: "Sensors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Sensors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Sensors");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Sensors",
                newName: "ConfigurationJson");
        }
    }
}
