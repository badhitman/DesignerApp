using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext014 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName",
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

            migrationBuilder.AddColumn<int>(
                name: "RubricId",
                table: "OffersAvailability",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RubricId",
                table: "LockerOffersAvailability",
                type: "integer",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "RubricId",
                table: "RowsOfWarehouseDocuments");

            migrationBuilder.DropColumn(
                name: "RubricId",
                table: "RowsOfOrdersDocuments");

            migrationBuilder.DropColumn(
                name: "RubricId",
                table: "OffersAvailability");

            migrationBuilder.DropColumn(
                name: "RubricId",
                table: "LockerOffersAvailability");

            migrationBuilder.CreateIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName",
                table: "LockerOffersAvailability",
                columns: new[] { "LockerId", "LockerName" },
                unique: true);
        }
    }
}
