using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceFinancialRecordCurrencyWithCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "UnitFinancialRecords",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE r
SET r.CurrencyId = c.Id
FROM UnitFinancialRecords r
INNER JOIN Currencies c ON c.Code = UPPER(LTRIM(RTRIM(r.Currency)));
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Currencies WHERE Code = N'EUR')
INSERT INTO Currencies (Code, Name, Symbol, DecimalPlaces, IsActive)
VALUES (N'EUR', N'Euro', N'€', 2, 1);
");

            migrationBuilder.Sql(@"
UPDATE UnitFinancialRecords
SET CurrencyId = (SELECT TOP 1 Id FROM Currencies WHERE Code = N'EUR')
WHERE CurrencyId IS NULL;
");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyId",
                table: "UnitFinancialRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "UnitFinancialRecords");

            migrationBuilder.CreateIndex(
                name: "IX_UnitFinancialRecords_CurrencyId",
                table: "UnitFinancialRecords",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitFinancialRecords_Currencies_CurrencyId",
                table: "UnitFinancialRecords",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitFinancialRecords_Currencies_CurrencyId",
                table: "UnitFinancialRecords");

            migrationBuilder.DropIndex(
                name: "IX_UnitFinancialRecords_CurrencyId",
                table: "UnitFinancialRecords");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "UnitFinancialRecords",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.Sql(@"
UPDATE r
SET r.Currency = LEFT(c.Code, 3)
FROM UnitFinancialRecords r
INNER JOIN Currencies c ON c.Id = r.CurrencyId;
");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Code",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "UnitFinancialRecords");
        }
    }
}
