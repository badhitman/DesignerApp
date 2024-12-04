////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;
using System;

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
    public DbSet<OrganizationModelDB> Organizations { get; set; } = default!;

    /// <summary>
    /// AddressesOrganizations
    /// </summary>
    public DbSet<AddressOrganizationModelDB> AddressesOrganizations { get; set; } = default!;

    /// <summary>
    /// OrganizationsUsers
    /// </summary>
    public DbSet<UserOrganizationModelDB> OrganizationsUsers { get; set; } = default!;


    /// <summary>
    /// Номенклатура
    /// </summary>
    public DbSet<NomenclatureModelDB> Goods { get; set; } = default!;

    /// <summary>
    /// ProductsOffers
    /// </summary>
    public DbSet<OfferGoodModelDB> OffersGoods { get; set; } = default!;

    /// <summary>
    /// Правила формирования цены
    /// </summary>
    public DbSet<PriceRuleForOfferModelDB> PricesRules { get; set; } = default!;


    /// <summary>
    /// Документы поступления
    /// </summary>
    public DbSet<WarehouseDocumentModelDB> WarehouseDocuments { get; set; } = default!;

    /// <summary>
    /// Rows of warehouse documents
    /// </summary>
    public DbSet<RowOfWarehouseDocumentModelDB> RowsOfWarehouseDocuments { get; set; } = default!;

    /// <summary>
    /// Offers availability
    /// </summary>
    public DbSet<OfferAvailabilityModelDB> OffersAvailability { get; set; } = default!;

    /// <summary>
    /// Locker offers availability
    /// </summary>
    public DbSet<LockOffersAvailabilityModelDB> LockerOffersAvailability { get; set; } = default!;


    /// <summary>
    /// Orders
    /// </summary>
    public DbSet<OrderDocumentModelDB> OrdersDocuments { get; set; } = default!;

    /// <summary>
    /// Адреса организаций в заказе
    /// </summary>
    public DbSet<TabAddressForOrderModelDb> TabsAddressesForOrders { get; set; } = default!;

    /// <summary>
    /// Строки заказов
    /// </summary>
    public DbSet<RowOfOrderDocumentModelDB> RowsOfOrdersDocuments { get; set; } = default!;


    /// <summary>
    /// Payments documents
    /// </summary>
    public DbSet<PaymentDocumentModelDb> PaymentsDocuments { get; set; } = default!;
}