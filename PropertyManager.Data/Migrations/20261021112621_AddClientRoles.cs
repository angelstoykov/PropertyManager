using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractorClientId",
                table: "UnitFinancialRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientRoles_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitFinancialRecords_ContractorClientId",
                table: "UnitFinancialRecords",
                column: "ContractorClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRoles_ClientId_Role",
                table: "ClientRoles",
                columns: new[] { "ClientId", "Role" },
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO [ClientRoles] ([ClientId], [Role], [StartDate], [IsActive])
                SELECT [Id], 20, CAST(GETDATE() AS date), CAST(1 AS bit)
                FROM [Clients];
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitFinancialRecords_Clients_ContractorClientId",
                table: "UnitFinancialRecords",
                column: "ContractorClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitFinancialRecords_Clients_ContractorClientId",
                table: "UnitFinancialRecords");

            migrationBuilder.DropTable(
                name: "ClientRoles");

            migrationBuilder.DropIndex(
                name: "IX_UnitFinancialRecords_ContractorClientId",
                table: "UnitFinancialRecords");

            migrationBuilder.DropColumn(
                name: "ContractorClientId",
                table: "UnitFinancialRecords");
        }
    }
}
