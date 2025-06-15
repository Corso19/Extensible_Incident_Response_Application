using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentResponseAPI.Migrations
{
    /// <inheritdoc />
    public partial class IncidentsModelNavigationToRecomm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recommendations_IncidentId",
                table: "Recommendations");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_IncidentId",
                table: "Recommendations",
                column: "IncidentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recommendations_IncidentId",
                table: "Recommendations");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_IncidentId",
                table: "Recommendations",
                column: "IncidentId");
        }
    }
}
