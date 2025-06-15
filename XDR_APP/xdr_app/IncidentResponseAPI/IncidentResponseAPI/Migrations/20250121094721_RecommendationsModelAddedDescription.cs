using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentResponseAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecommendationsModelAddedDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Recommendation",
                table: "Recommendations",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Recommendations",
                newName: "Recommendation");
        }
    }
}
