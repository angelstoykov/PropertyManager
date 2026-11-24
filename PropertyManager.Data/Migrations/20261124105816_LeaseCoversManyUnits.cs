using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class LeaseCoversManyUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaseUnits",
                columns: table => new
                {
                    LeaseId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseUnits", x => new { x.LeaseId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_LeaseUnits_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaseUnits_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaseUnits_UnitId",
                table: "LeaseUnits",
                column: "UnitId");

            migrationBuilder.Sql("""
                INSERT INTO LeaseUnits (LeaseId, UnitId)
                SELECT Id, UnitId FROM Leases;
                """);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Leases",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE l
                SET ClientId = (
                    SELECT MIN(cu.ClientId)
                    FROM ClientUnit cu
                    INNER JOIN LeaseUnits lu ON lu.UnitId = cu.UnitId
                    WHERE lu.LeaseId = l.Id)
                FROM Leases l
                WHERE l.ClientId IS NULL;

                DELETE FROM RentPayments
                WHERE LeaseId IN (SELECT Id FROM Leases WHERE ClientId IS NULL);

                DELETE FROM Leases WHERE ClientId IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Leases",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Units_UnitId",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_UnitId",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Leases");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Leases",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_ClientId",
                table: "Leases",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Clients_ClientId",
                table: "Leases",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                DECLARE @ClientId int, @UnitId int, @LeaseId int;
                DECLARE unit_cursor CURSOR LOCAL FAST_FORWARD FOR
                    SELECT cu.ClientId, cu.UnitId
                    FROM ClientUnit cu
                    WHERE NOT EXISTS (
                        SELECT 1 FROM LeaseUnits lu WHERE lu.UnitId = cu.UnitId);

                OPEN unit_cursor;
                FETCH NEXT FROM unit_cursor INTO @ClientId, @UnitId;
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    INSERT INTO Leases (ClientId, StartDate, MonthlyRent, DepositAmount, Status)
                    VALUES (@ClientId, CAST(GETDATE() AS date), 0, 0, 10);

                    SET @LeaseId = CAST(SCOPE_IDENTITY() AS int);

                    INSERT INTO LeaseUnits (LeaseId, UnitId)
                    VALUES (@LeaseId, @UnitId);

                    FETCH NEXT FROM unit_cursor INTO @ClientId, @UnitId;
                END
                CLOSE unit_cursor;
                DEALLOCATE unit_cursor;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Clients_ClientId",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_ClientId",
                table: "Leases");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Leases",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE l
                SET UnitId = (
                    SELECT MIN(lu.UnitId)
                    FROM LeaseUnits lu
                    WHERE lu.LeaseId = l.Id)
                FROM Leases l;

                DELETE FROM RentPayments
                WHERE LeaseId IN (SELECT Id FROM Leases WHERE UnitId IS NULL);

                DELETE FROM Leases WHERE UnitId IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Leases",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Leases");

            migrationBuilder.DropTable(
                name: "LeaseUnits");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Leases",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_UnitId",
                table: "Leases",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Units_UnitId",
                table: "Leases",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
