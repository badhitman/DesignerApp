using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext015_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName_RubricId",
                table: "LockerOffersAvailability");

            migrationBuilder.CreateIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName",
                table: "LockerOffersAvailability",
                columns: new[] { "LockerId", "LockerName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName",
                table: "LockerOffersAvailability");

            migrationBuilder.CreateIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName_RubricId",
                table: "LockerOffersAvailability",
                columns: new[] { "LockerId", "LockerName", "RubricId" },
                unique: true);
        }
    }
}
