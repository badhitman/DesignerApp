////////////////////////////////////////////////
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
#if DEBUG
        Database.Migrate();
#else
        Database.EnsureCreated();
#endif
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
#if DEBUG
        options.LogTo(Console.WriteLine);
#endif
    }

    /// <summary>
    /// AttachmentsForOrders
    /// </summary>
    public DbSet<AttachmentForOrderModelDB> AttachmentsForOrders { get; set; }

    /// <summary>
    /// PaymentsDocuments
    /// </summary>
    public DbSet<PaymentDocumentModelDb> PaymentsDocuments { get; set; }

    /// <summary>
    /// Доставка
    /// </summary>
    public DbSet<DeliveryModelDb> Deliveries { get; set; }

    /// <summary>
    /// Строки заказов
    /// </summary>
    public DbSet<RowOfOrderDocumentModelDB> RowsOfOrdersDocuments { get; set; }

    /// <summary>
    /// Orders
    /// </summary>
    public DbSet<OrderDocumentModelDB> OrdersDocuments { get; set; }

    /// <summary>
    /// ProductsOffers
    /// </summary>
    public DbSet<OfferGoodModelDB> OffersGoods { get; set; }

    /// <summary>
    /// Товары
    /// </summary>
    public DbSet<GoodsModelDB> Goods { get; set; }

    /// <summary>
    /// Organizations
    /// </summary>
    public DbSet<OrganizationModelDB> Organizations { get; set; }

    /// <summary>
    /// OrganizationsUsers
    /// </summary>
    public DbSet<UserOrganizationModelDB> OrganizationsUsers { get; set; }

    /// <summary>
    /// AddressesOrganizations
    /// </summary>
    public DbSet<AddressOrganizationModelDB> AddressesOrganizations { get; set; }
}