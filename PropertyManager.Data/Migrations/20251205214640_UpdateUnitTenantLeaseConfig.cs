using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUnitTenantLeaseConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lease_Tenant_TenantId",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_Units_UnitId",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_RentPayment_Lease_LeaseId",
                table: "RentPayment");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentPayment",
                table: "RentPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lease",
                table: "Lease");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Properties");

            migrationBuilder.RenameTable(
                name: "RentPayment",
                newName: "RentPayments");

            migrationBuilder.RenameTable(
                name: "Lease",
                newName: "Leases");

            migrationBuilder.RenameIndex(
                name: "IX_RentPayment_LeaseId",
                table: "RentPayments",
                newName: "IX_RentPayments_LeaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_UnitId",
                table: "Leases",
                newName: "IX_Leases_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_TenantId",
                table: "Leases",
                newName: "IX_Leases_TenantId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Properties",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentPayments",
                table: "RentPayments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leases",
                table: "Leases",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsCompany = table.Column<bool>(type: "bit", nullable: false),
                    CompanyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Tenants_TenantId",
                table: "Leases",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Units_UnitId",
                table: "Leases",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentPayments_Leases_LeaseId",
                table: "RentPayments",
                column: "LeaseId",
                principalTable: "Leases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Tenants_TenantId",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Units_UnitId",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_RentPayments_Leases_LeaseId",
                table: "RentPayments");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentPayments",
                table: "RentPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leases",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Properties");

            migrationBuilder.RenameTable(
                name: "RentPayments",
                newName: "RentPayment");

            migrationBuilder.RenameTable(
                name: "Leases",
                newName: "Lease");

            migrationBuilder.RenameIndex(
                name: "IX_RentPayments_LeaseId",
                table: "RentPayment",
                newName: "IX_RentPayment_LeaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_UnitId",
                table: "Lease",
                newName: "IX_Lease_UnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_TenantId",
                table: "Lease",
                newName: "IX_Lease_TenantId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentPayment",
                table: "RentPayment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lease",
                table: "Lease",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsCompany = table.Column<bool>(type: "bit", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_Tenant_TenantId",
                table: "Lease",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_Units_UnitId",
                table: "Lease",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentPayment_Lease_LeaseId",
                table: "RentPayment",
                column: "LeaseId",
                principalTable: "Lease",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
