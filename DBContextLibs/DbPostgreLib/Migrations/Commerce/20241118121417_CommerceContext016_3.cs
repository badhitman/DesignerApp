﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext016_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TabsAddressesForOrders_WarehouseId",
                table: "TabsAddressesForOrders",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TabsAddressesForOrders_WarehouseId",
                table: "TabsAddressesForOrders");
        }
    }
}
