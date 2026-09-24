using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class RepairedIndexInStaticShipData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData");

            migrationBuilder.DropIndex(
                name: "IX_StaticShipData_ShipType",
                table: "StaticShipData");

            migrationBuilder.CreateIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData",
                column: "ImoNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaticShipData_ShipType",
                table: "StaticShipData",
                column: "ShipType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData");

            migrationBuilder.DropIndex(
                name: "IX_StaticShipData_ShipType",
                table: "StaticShipData");

            migrationBuilder.CreateIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData",
                column: "ImoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_StaticShipData_ShipType",
                table: "StaticShipData",
                column: "ShipType",
                unique: true);
        }
    }
}
