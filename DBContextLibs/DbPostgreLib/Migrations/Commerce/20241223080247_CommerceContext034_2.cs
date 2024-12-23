using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext034_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LockerOffersAvailability",
                table: "LockerOffersAvailability");

            migrationBuilder.RenameTable(
                name: "LockerOffersAvailability",
                newName: "LockersTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_LockerOffersAvailability_LockerId_LockerName_RubricId",
                table: "LockersTransactions",
                newName: "IX_LockersTransactions_LockerId_LockerName_RubricId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LockersTransactions",
                table: "LockersTransactions",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LockersTransactions",
                table: "LockersTransactions");

            migrationBuilder.RenameTable(
                name: "LockersTransactions",
                newName: "LockerOffersAvailability");

            migrationBuilder.RenameIndex(
                name: "IX_LockersTransactions_LockerId_LockerName_RubricId",
                table: "LockerOffersAvailability",
                newName: "IX_LockerOffersAvailability_LockerId_LockerName_RubricId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LockerOffersAvailability",
                table: "LockerOffersAvailability",
                column: "Id");
        }
    }
}
