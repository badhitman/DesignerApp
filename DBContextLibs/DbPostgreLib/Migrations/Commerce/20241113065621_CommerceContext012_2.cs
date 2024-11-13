using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext012_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LockerOffersAvailability_OffersAvailability_OfferAvailabili~",
                table: "LockerOffersAvailability");

            migrationBuilder.DropIndex(
                name: "IX_LockerOffersAvailability_OfferAvailabilityId",
                table: "LockerOffersAvailability");

            migrationBuilder.RenameColumn(
                name: "OfferAvailabilityId",
                table: "LockerOffersAvailability",
                newName: "LockerId");

            migrationBuilder.AddColumn<string>(
                name: "LockerName",
                table: "LockerOffersAvailability",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.DropColumn(
                name: "LockerName",
                table: "LockerOffersAvailability");

            migrationBuilder.RenameColumn(
                name: "LockerId",
                table: "LockerOffersAvailability",
                newName: "OfferAvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LockerOffersAvailability_OfferAvailabilityId",
                table: "LockerOffersAvailability",
                column: "OfferAvailabilityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LockerOffersAvailability_OffersAvailability_OfferAvailabili~",
                table: "LockerOffersAvailability",
                column: "OfferAvailabilityId",
                principalTable: "OffersAvailability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
