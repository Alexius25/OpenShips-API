using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticShipDataAis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Mmsi",
                table: "ShipDestinations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "StaticShipData",
                columns: table => new
                {
                    Mmsi = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    License = table.Column<int>(type: "integer", nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ShipName = table.Column<string>(type: "text", nullable: true),
                    ShipType = table.Column<int>(type: "integer", nullable: true),
                    ImoNumber = table.Column<int>(type: "integer", nullable: true),
                    CallSign = table.Column<string>(type: "text", nullable: true),
                    RawDestination = table.Column<string>(type: "text", nullable: true),
                    Draught = table.Column<float>(type: "real", nullable: true),
                    Eta = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Dim_A = table.Column<float>(type: "real", nullable: true),
                    Dim_B = table.Column<float>(type: "real", nullable: true),
                    Dim_C = table.Column<float>(type: "real", nullable: true),
                    Dim_D = table.Column<float>(type: "real", nullable: true),
                    Length = table.Column<float>(type: "real", nullable: true),
                    Beam = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticShipData", x => x.Mmsi);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaticShipData");

            migrationBuilder.AlterColumn<long>(
                name: "Mmsi",
                table: "ShipDestinations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
