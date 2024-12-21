using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext033_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractorsOrganizations_Offers_OfferId",
                table: "ContractorsOrganizations");

            migrationBuilder.DropColumn(
                name: "LastAtUpdatedUTC",
                table: "ContractorsOrganizations");

            migrationBuilder.AlterColumn<int>(
                name: "OfferId",
                table: "ContractorsOrganizations",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractorsOrganizations_Offers_OfferId",
                table: "ContractorsOrganizations",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractorsOrganizations_Offers_OfferId",
                table: "ContractorsOrganizations");

            migrationBuilder.AlterColumn<int>(
                name: "OfferId",
                table: "ContractorsOrganizations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAtUpdatedUTC",
                table: "ContractorsOrganizations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_ContractorsOrganizations_Offers_OfferId",
                table: "ContractorsOrganizations",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
