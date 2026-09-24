using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexShipDestinations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShipDestinations_Mmsi_LastSeen",
                table: "ShipDestinations",
                columns: new[] { "Mmsi", "LastSeen" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShipDestinations_Mmsi_LastSeen",
                table: "ShipDestinations");
        }
    }
}
