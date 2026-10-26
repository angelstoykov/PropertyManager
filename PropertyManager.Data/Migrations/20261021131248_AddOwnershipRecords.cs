using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnershipRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: true),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SharePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnershipRecords", x => x.Id);
                    table.CheckConstraint("CK_OwnershipRecords_Target", "([PropertyId] IS NOT NULL AND [UnitId] IS NULL) OR ([PropertyId] IS NULL AND [UnitId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OwnershipRecords_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnershipRecords_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnershipRecords_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipRecords_ClientId",
                table: "OwnershipRecords",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipRecords_PropertyId_StartDate",
                table: "OwnershipRecords",
                columns: new[] { "PropertyId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipRecords_UnitId_StartDate",
                table: "OwnershipRecords",
                columns: new[] { "UnitId", "StartDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnershipRecords");
        }
    }
}
