using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext034 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContractorsOrganizations_OrganizationId",
                table: "ContractorsOrganizations");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorsOrganizations_OrganizationId_OfferId",
                table: "ContractorsOrganizations",
                columns: new[] { "OrganizationId", "OfferId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContractorsOrganizations_OrganizationId_OfferId",
                table: "ContractorsOrganizations");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorsOrganizations_OrganizationId",
                table: "ContractorsOrganizations",
                column: "OrganizationId");
        }
    }
}
