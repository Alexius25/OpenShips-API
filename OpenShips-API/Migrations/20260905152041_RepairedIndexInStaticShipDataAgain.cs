using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class RepairedIndexInStaticShipDataAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData");

            migrationBuilder.CreateIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData",
                column: "ImoNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData");

            migrationBuilder.CreateIndex(
                name: "IX_StaticShipData_ImoNumber",
                table: "StaticShipData",
                column: "ImoNumber",
                unique: true);
        }
    }
}
