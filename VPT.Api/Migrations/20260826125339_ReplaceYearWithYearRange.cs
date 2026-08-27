using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPT.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceYearWithYearRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Year",
                table: "VehicleSearches",
                newName: "MinYear");

            migrationBuilder.AddColumn<int>(
                name: "MaxYear",
                table: "VehicleSearches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxYear",
                table: "VehicleSearches");

            migrationBuilder.RenameColumn(
                name: "MinYear",
                table: "VehicleSearches",
                newName: "Year");
        }
    }
}
