using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenShipsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPortsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Location = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameWoDiacritics = table.Column<string>(type: "text", nullable: true),
                    Subregion = table.Column<string>(type: "text", nullable: true),
                    Function = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ports_Country_Location",
                table: "Ports",
                columns: new[] { "Country", "Location" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ports_Latitude_Longitude",
                table: "Ports",
                columns: new[] { "Latitude", "Longitude" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ports");
        }
    }
}
