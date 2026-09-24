using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDestinationPortReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromPortId",
                table: "ShipDestinations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchedBy",
                table: "ShipDestinations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToPortId",
                table: "ShipDestinations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipDestinations_FromPortId",
                table: "ShipDestinations",
                column: "FromPortId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipDestinations_ToPortId",
                table: "ShipDestinations",
                column: "ToPortId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipDestinations_Ports_FromPortId",
                table: "ShipDestinations",
                column: "FromPortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipDestinations_Ports_ToPortId",
                table: "ShipDestinations",
                column: "ToPortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipDestinations_Ports_FromPortId",
                table: "ShipDestinations");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipDestinations_Ports_ToPortId",
                table: "ShipDestinations");

            migrationBuilder.DropIndex(
                name: "IX_ShipDestinations_FromPortId",
                table: "ShipDestinations");

            migrationBuilder.DropIndex(
                name: "IX_ShipDestinations_ToPortId",
                table: "ShipDestinations");

            migrationBuilder.DropColumn(
                name: "FromPortId",
                table: "ShipDestinations");

            migrationBuilder.DropColumn(
                name: "MatchedBy",
                table: "ShipDestinations");

            migrationBuilder.DropColumn(
                name: "ToPortId",
                table: "ShipDestinations");
        }
    }
}
