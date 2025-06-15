using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentResponseAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSensorsModelForced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecommendationsModel_Incidents_IncidentId",
                table: "RecommendationsModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecommendationsModel",
                table: "RecommendationsModel");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Sensors");

            migrationBuilder.RenameTable(
                name: "RecommendationsModel",
                newName: "Recommendations");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Sensors",
                newName: "DummyColumn");

            migrationBuilder.RenameColumn(
                name: "CreatedAd",
                table: "Sensors",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "Sensors",
                newName: "Configuration");

            migrationBuilder.RenameIndex(
                name: "IX_RecommendationsModel_IncidentId",
                table: "Recommendations",
                newName: "IX_Recommendations_IncidentId");

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "Sensors",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastEventMarker",
                table: "Sensors",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRunAfter",
                table: "Sensors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recommendations",
                table: "Recommendations",
                column: "RecommendationId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Sensors_RetrievalInterval",
                table: "Sensors",
                sql: "[RetrievalInterval] BETWEEN 1 AND 1440");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_Incidents_IncidentId",
                table: "Recommendations",
                column: "IncidentId",
                principalTable: "Incidents",
                principalColumn: "IncidentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_Incidents_IncidentId",
                table: "Recommendations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Sensors_RetrievalInterval",
                table: "Sensors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recommendations",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "LastEventMarker",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "NextRunAfter",
                table: "Sensors");

            migrationBuilder.RenameTable(
                name: "Recommendations",
                newName: "RecommendationsModel");

            migrationBuilder.RenameColumn(
                name: "DummyColumn",
                table: "Sensors",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Sensors",
                newName: "CreatedAd");

            migrationBuilder.RenameColumn(
                name: "Configuration",
                table: "Sensors",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Recommendations_IncidentId",
                table: "RecommendationsModel",
                newName: "IX_RecommendationsModel_IncidentId");

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Sensors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecommendationsModel",
                table: "RecommendationsModel",
                column: "RecommendationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecommendationsModel_Incidents_IncidentId",
                table: "RecommendationsModel",
                column: "IncidentId",
                principalTable: "Incidents",
                principalColumn: "IncidentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
