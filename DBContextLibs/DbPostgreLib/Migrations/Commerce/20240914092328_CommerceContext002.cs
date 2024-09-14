using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricesRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    QuantityRule = table.Column<long>(type: "bigint", nullable: false),
                    PriceRule = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricesRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricesRules_OffersGoods_OfferId",
                        column: x => x.OfferId,
                        principalTable: "OffersGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricesRules_IsDisabled",
                table: "PricesRules",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_PricesRules_Name",
                table: "PricesRules",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PricesRules_OfferId_QuantityRule",
                table: "PricesRules",
                columns: new[] { "OfferId", "QuantityRule" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricesRules");
        }
    }
}
