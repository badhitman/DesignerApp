﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class CommerceLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public CommerceLayerContext(DbContextOptions options)
        : base(options)
    {
//#if DEBUG
//        Database.EnsureCreated();
//#else
        Database.Migrate();
//#endif
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(Console.WriteLine);
#endif
    }

    /// <summary>
    /// Organizations
    /// </summary>
    public DbSet<OrganizationModelDB> Organizations { get; set; }

    /// <summary>
    /// AddressesOrganizations
    /// </summary>
    public DbSet<AddressOrganizationModelDB> AddressesOrganizations { get; set; }

    /// <summary>
    /// OrganizationsUsers
    /// </summary>
    public DbSet<UserOrganizationModelDB> OrganizationsUsers { get; set; }


    /// <summary>
    /// Товары
    /// </summary>
    public DbSet<GoodsModelDB> Goods { get; set; }

    /// <summary>
    /// ProductsOffers
    /// </summary>
    public DbSet<OfferGoodModelDB> OffersGoods { get; set; }

    /// <summary>
    /// Правила формирования цены
    /// </summary>
    public DbSet<PriceRuleForOfferModelDB> PricesRules { get; set; }



    /// <summary>
    /// Orders
    /// </summary>
    public DbSet<OrderDocumentModelDB> OrdersDocuments { get; set; }

    /// <summary>
    /// Адреса организаций в заказе
    /// </summary>
    public DbSet<TabAddressForOrderModelDb> TabsAddressesForOrders { get; set; }

    /// <summary>
    /// Строки заказов
    /// </summary>
    public DbSet<RowOfOrderDocumentModelDB> RowsOfOrdersDocuments { get; set; }


    /// <summary>
    /// PaymentsDocuments
    /// </summary>
    public DbSet<PaymentDocumentModelDb> PaymentsDocuments { get; set; }

    /// <summary>
    /// AttachmentsForOrders
    /// </summary>
    public DbSet<AttachmentForOrderModelDB> AttachmentsForOrders { get; set; }
}