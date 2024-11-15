using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext015_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RowsOfWarehouseDocuments_RubricId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RowsOfOrdersDocuments_RubricId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OffersAvailability_RubricId",
                table: "OffersAvailability");

            migrationBuilder.DropIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName",
                table: "LockerOffersAvailability");

            migrationBuilder.DropColumn(
                name: "RubricId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropColumn(
                name: "RubricId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.AlterColumn<int>(
                name: "RubricId",
                table: "OffersAvailability",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RubricId",
                table: "LockerOffersAvailability",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName_RubricId",
                table: "LockerOffersAvailability",
                columns: new[] { "LockerId", "LockerName", "RubricId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName_RubricId",
                table: "LockerOffersAvailability");

            migrationBuilder.AddColumn<int>(
                name: "RubricId",
                table: "RowsOfWarehouseDocuments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RubricId",
                table: "RowsOfOrdersDocuments",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RubricId",
                table: "OffersAvailability",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "RubricId",
                table: "LockerOffersAvailability",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfWarehouseDocuments_RubricId",
                table: "RowsOfWarehouseDocuments",
                column: "RubricId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_RubricId",
                table: "RowsOfOrdersDocuments",
                column: "RubricId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersAvailability_RubricId",
                table: "OffersAvailability",
                column: "RubricId");

            migrationBuilder.CreateIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName",
                table: "LockerOffersAvailability",
                columns: new[] { "LockerId", "LockerName" },
                unique: true);
        }
    }
}
