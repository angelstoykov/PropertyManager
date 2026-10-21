using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PersonalId",
                table: "Clients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Clients",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Clients",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Clients",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Clients",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Clients
                SET
                    PersonalId = NULLIF(LTRIM(RTRIM(PersonalId)), N''),
                    CompanyNumber = NULLIF(LTRIM(RTRIM(CompanyNumber)), N''),
                    ClientType = CASE
                        WHEN NULLIF(LTRIM(RTRIM(ISNULL(FirstName, N''))), N'') IS NULL
                         AND NULLIF(LTRIM(RTRIM(ISNULL(LastName, N''))), N'') IS NULL
                         AND (
                                NULLIF(LTRIM(RTRIM(ISNULL(CompanyName, N''))), N'') IS NOT NULL
                                OR NULLIF(LTRIM(RTRIM(ISNULL(CompanyNumber, N''))), N'') IS NOT NULL
                             )
                        THEN 20
                        ELSE 10
                    END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyNumber",
                table: "Clients",
                column: "CompanyNumber",
                unique: true,
                filter: "[CompanyNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_PersonalId",
                table: "Clients",
                column: "PersonalId",
                unique: true,
                filter: "[PersonalId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_CompanyNumber",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_PersonalId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "PersonalId",
                table: "Clients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
