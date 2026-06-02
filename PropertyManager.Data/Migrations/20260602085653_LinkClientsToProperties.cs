using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkClientsToProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientProperty",
                columns: table => new
                {
                    RentedPropertiesId = table.Column<int>(type: "int", nullable: false),
                    RentingClientsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProperty", x => new { x.RentedPropertiesId, x.RentingClientsId });
                    table.ForeignKey(
                        name: "FK_ClientProperty_Clients_RentingClientsId",
                        column: x => x.RentingClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientProperty_Properties_RentedPropertiesId",
                        column: x => x.RentedPropertiesId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProperty_RentingClientsId",
                table: "ClientProperty",
                column: "RentingClientsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientProperty");
        }
    }
}
