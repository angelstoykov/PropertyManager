using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitPriceComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IncludeInCommission = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitPriceComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    PriceComponentId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitPriceComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitPriceComponents_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitPriceComponents_PriceComponents_PriceComponentId",
                        column: x => x.PriceComponentId,
                        principalTable: "PriceComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitPriceComponents_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceComponents_Name",
                table: "PriceComponents",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitPriceComponents_CurrencyId",
                table: "UnitPriceComponents",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitPriceComponents_PriceComponentId",
                table: "UnitPriceComponents",
                column: "PriceComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitPriceComponents_UnitId_PriceComponentId",
                table: "UnitPriceComponents",
                columns: new[] { "UnitId", "PriceComponentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitPriceComponents");

            migrationBuilder.DropTable(
                name: "PriceComponents");
        }
    }
}
