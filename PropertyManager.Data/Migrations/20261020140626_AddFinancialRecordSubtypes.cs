using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialRecordSubtypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialRecordSubtypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialRecordSubtypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialRecordSubtypes_Type_Name",
                table: "FinancialRecordSubtypes",
                columns: new[] { "Type", "Name" },
                unique: true);

            migrationBuilder.Sql(@"
INSERT INTO FinancialRecordSubtypes (Type, Name, IsActive, SortOrder)
VALUES
    (10, N'Rent', 1, 1),
    (10, N'Fee', 1, 2),
    (10, N'Penalty', 1, 3),
    (10, N'Repair charged to tenant', 1, 4),
    (10, N'Other', 1, 5),
    (20, N'Repair', 1, 1),
    (20, N'Key duplicates', 1, 2),
    (20, N'Cleaning', 1, 3),
    (20, N'Additional service', 1, 4),
    (20, N'Other', 1, 5);
");

            migrationBuilder.AddColumn<int>(
                name: "SubtypeId",
                table: "UnitFinancialRecords",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE r
SET r.SubtypeId = s.Id
FROM UnitFinancialRecords r
INNER JOIN FinancialRecordSubtypes s ON s.Type = r.Type AND s.Name = N'Other';
");

            migrationBuilder.AlterColumn<int>(
                name: "SubtypeId",
                table: "UnitFinancialRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitFinancialRecords_SubtypeId",
                table: "UnitFinancialRecords",
                column: "SubtypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitFinancialRecords_FinancialRecordSubtypes_SubtypeId",
                table: "UnitFinancialRecords",
                column: "SubtypeId",
                principalTable: "FinancialRecordSubtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitFinancialRecords_FinancialRecordSubtypes_SubtypeId",
                table: "UnitFinancialRecords");

            migrationBuilder.DropTable(
                name: "FinancialRecordSubtypes");

            migrationBuilder.DropIndex(
                name: "IX_UnitFinancialRecords_SubtypeId",
                table: "UnitFinancialRecords");

            migrationBuilder.DropColumn(
                name: "SubtypeId",
                table: "UnitFinancialRecords");
        }
    }
}
