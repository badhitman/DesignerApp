using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    /// <inheritdoc />
    public partial class CommerceContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseUnit = table.Column<int>(type: "integer", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NewName = table.Column<string>(type: "text", nullable: true),
                    NewLegalAddress = table.Column<string>(type: "text", nullable: true),
                    NewINN = table.Column<string>(type: "text", nullable: true),
                    NewKPP = table.Column<string>(type: "text", nullable: true),
                    NewOGRN = table.Column<string>(type: "text", nullable: true),
                    NewCurrentAccount = table.Column<string>(type: "text", nullable: true),
                    NewCorrespondentAccount = table.Column<string>(type: "text", nullable: true),
                    NewBankName = table.Column<string>(type: "text", nullable: true),
                    NewBankBIC = table.Column<string>(type: "text", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    LegalAddress = table.Column<string>(type: "text", nullable: false),
                    INN = table.Column<string>(type: "text", nullable: false),
                    KPP = table.Column<string>(type: "text", nullable: false),
                    OGRN = table.Column<string>(type: "text", nullable: false),
                    CurrentAccount = table.Column<string>(type: "text", nullable: false),
                    CorrespondentAccount = table.Column<string>(type: "text", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    BankBIC = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffersGoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    OfferUnit = table.Column<int>(type: "integer", nullable: false),
                    Multiplicity = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffersGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffersGoods_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AddressesOrganizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Contacts = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressesOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressesOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdersDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorIdentityUserId = table.Column<string>(type: "text", nullable: false),
                    ExternalDocumentId = table.Column<string>(type: "text", nullable: true),
                    HelpdeskId = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersDocuments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationsUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    UserPersonIdentityId = table.Column<string>(type: "text", nullable: false),
                    LastAtUpdatedUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationsUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationsUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentsForOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OrderDocumentId = table.Column<int>(type: "integer", nullable: false),
                    FilePoint = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUTC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentsForOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentsForOrders_OrdersDocuments_OrderDocumentId",
                        column: x => x.OrderDocumentId,
                        principalTable: "OrdersDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentsDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ExternalDocumentId = table.Column<string>(type: "text", nullable: false),
                    OrderDocumentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentsDocuments_OrdersDocuments_OrderDocumentId",
                        column: x => x.OrderDocumentId,
                        principalTable: "OrdersDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabsAddressesForOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderDocumentId = table.Column<int>(type: "integer", nullable: false),
                    AddressOrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabsAddressesForOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabsAddressesForOrders_AddressesOrganizations_AddressOrgani~",
                        column: x => x.AddressOrganizationId,
                        principalTable: "AddressesOrganizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TabsAddressesForOrders_OrdersDocuments_OrderDocumentId",
                        column: x => x.OrderDocumentId,
                        principalTable: "OrdersDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RowsOfOrdersDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressForOrderTabId = table.Column<int>(type: "integer", nullable: false),
                    OrderDocumentId = table.Column<int>(type: "integer", nullable: true),
                    OfferId = table.Column<int>(type: "integer", nullable: false),
                    GoodsId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RowsOfOrdersDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersDocuments_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersDocuments_OffersGoods_OfferId",
                        column: x => x.OfferId,
                        principalTable: "OffersGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersDocuments_OrdersDocuments_OrderDocumentId",
                        column: x => x.OrderDocumentId,
                        principalTable: "OrdersDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RowsOfOrdersDocuments_TabsAddressesForOrders_AddressForOrde~",
                        column: x => x.AddressForOrderTabId,
                        principalTable: "TabsAddressesForOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressesOrganizations_Name",
                table: "AddressesOrganizations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AddressesOrganizations_OrganizationId",
                table: "AddressesOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentsForOrders_Name",
                table: "AttachmentsForOrders",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentsForOrders_OrderDocumentId",
                table: "AttachmentsForOrders",
                column: "OrderDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_IsDisabled",
                table: "Goods",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Name",
                table: "Goods",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OffersGoods_GoodsId",
                table: "OffersGoods",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersGoods_IsDisabled",
                table: "OffersGoods",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_OffersGoods_Name",
                table: "OffersGoods",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersDocuments_IsDisabled",
                table: "OrdersDocuments",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersDocuments_Name",
                table: "OrdersDocuments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersDocuments_OrganizationId",
                table: "OrdersDocuments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_BankBIC_CorrespondentAccount_CurrentAccount",
                table: "Organizations",
                columns: new[] { "BankBIC", "CorrespondentAccount", "CurrentAccount" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_INN",
                table: "Organizations",
                column: "INN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_IsDisabled",
                table: "Organizations",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OGRN",
                table: "Organizations",
                column: "OGRN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsUsers_OrganizationId_UserPersonIdentityId",
                table: "OrganizationsUsers",
                columns: new[] { "OrganizationId", "UserPersonIdentityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsUsers_UserPersonIdentityId",
                table: "OrganizationsUsers",
                column: "UserPersonIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentsDocuments_Name",
                table: "PaymentsDocuments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentsDocuments_OrderDocumentId",
                table: "PaymentsDocuments",
                column: "OrderDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_AddressForOrderTabId",
                table: "RowsOfOrdersDocuments",
                column: "AddressForOrderTabId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_GoodsId",
                table: "RowsOfOrdersDocuments",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_OfferId",
                table: "RowsOfOrdersDocuments",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RowsOfOrdersDocuments_OrderDocumentId",
                table: "RowsOfOrdersDocuments",
                column: "OrderDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TabsAddressesForOrders_AddressOrganizationId",
                table: "TabsAddressesForOrders",
                column: "AddressOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TabsAddressesForOrders_OrderDocumentId",
                table: "TabsAddressesForOrders",
                column: "OrderDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentsForOrders");

            migrationBuilder.DropTable(
                name: "OrganizationsUsers");

            migrationBuilder.DropTable(
                name: "PaymentsDocuments");

            migrationBuilder.DropTable(
                name: "RowsOfOrdersDocuments");

            migrationBuilder.DropTable(
                name: "OffersGoods");

            migrationBuilder.DropTable(
                name: "TabsAddressesForOrders");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "AddressesOrganizations");

            migrationBuilder.DropTable(
                name: "OrdersDocuments");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
