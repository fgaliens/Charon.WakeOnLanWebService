using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Charon.WakeOnLanWebService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    MacAddress = table.Column<byte[]>(type: "BLOB", maxLength: 6, nullable: false),
                    IpAddress = table.Column<byte[]>(type: "BLOB", maxLength: 16, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.MacAddress);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
