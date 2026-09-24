using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAisPositionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrentAisPositions",
                columns: table => new
                {
                    Mmsi = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    SourceMessageId = table.Column<string>(type: "text", nullable: true),
                    License = table.Column<int>(type: "integer", nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Sog = table.Column<double>(type: "double precision", nullable: true),
                    Cog = table.Column<double>(type: "double precision", nullable: true),
                    Heading = table.Column<int>(type: "integer", nullable: true),
                    NavigationStatus = table.Column<int>(type: "integer", nullable: true),
                    RateOfTurn = table.Column<double>(type: "double precision", nullable: true),
                    PositionAccuracy = table.Column<bool>(type: "boolean", nullable: true),
                    RepeatIndicator = table.Column<int>(type: "integer", nullable: true),
                    Valid = table.Column<bool>(type: "boolean", nullable: true),
                    SpecialManoeuvreIndicator = table.Column<int>(type: "integer", nullable: true),
                    Spare = table.Column<int>(type: "integer", nullable: true),
                    Raim = table.Column<bool>(type: "boolean", nullable: true),
                    CommunicationState = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentAisPositions", x => x.Mmsi);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalAisPositions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    SourceMessageId = table.Column<string>(type: "text", nullable: true),
                    License = table.Column<int>(type: "integer", nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    Mmsi = table.Column<int>(type: "integer", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Sog = table.Column<double>(type: "double precision", nullable: true),
                    Cog = table.Column<double>(type: "double precision", nullable: true),
                    Heading = table.Column<int>(type: "integer", nullable: true),
                    NavigationStatus = table.Column<int>(type: "integer", nullable: true),
                    RateOfTurn = table.Column<double>(type: "double precision", nullable: true),
                    PositionAccuracy = table.Column<bool>(type: "boolean", nullable: true),
                    RepeatIndicator = table.Column<int>(type: "integer", nullable: true),
                    Valid = table.Column<bool>(type: "boolean", nullable: true),
                    SpecialManoeuvreIndicator = table.Column<int>(type: "integer", nullable: true),
                    Spare = table.Column<int>(type: "integer", nullable: true),
                    Raim = table.Column<bool>(type: "boolean", nullable: true),
                    CommunicationState = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalAisPositions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentAisPositions_EventTimestamp",
                table: "CurrentAisPositions",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalAisPositions_Mmsi_EventTimestamp",
                table: "HistoricalAisPositions",
                columns: new[] { "Mmsi", "EventTimestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentAisPositions");

            migrationBuilder.DropTable(
                name: "HistoricalAisPositions");
        }
    }
}
