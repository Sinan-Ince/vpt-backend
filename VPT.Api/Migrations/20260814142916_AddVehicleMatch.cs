using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VPT.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "VehicleTrackings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VehicleTrackings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckedAt",
                table: "VehicleTrackings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextCheckAt",
                table: "VehicleTrackings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VehicleMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleSearchId = table.Column<int>(type: "integer", nullable: false),
                    ListingId = table.Column<int>(type: "integer", nullable: false),
                    MatchScore = table.Column<decimal>(type: "numeric", nullable: false),
                    MatchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleMatches_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleMatches_VehicleSearches_VehicleSearchId",
                        column: x => x.VehicleSearchId,
                        principalTable: "VehicleSearches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleMatches_ListingId",
                table: "VehicleMatches",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleMatches_VehicleSearchId",
                table: "VehicleMatches",
                column: "VehicleSearchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleMatches");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VehicleTrackings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VehicleTrackings");

            migrationBuilder.DropColumn(
                name: "LastCheckedAt",
                table: "VehicleTrackings");

            migrationBuilder.DropColumn(
                name: "NextCheckAt",
                table: "VehicleTrackings");
        }
    }
}
