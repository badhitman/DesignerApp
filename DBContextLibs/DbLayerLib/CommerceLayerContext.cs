////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        TimeSpanToTicksConverter converter = new();
        DateOnlyToStringConverter converter2 = new();

        modelBuilder
           .Entity<WorkScheduleCalendarModelDB>()
           .Property(e => e.DateScheduleCalendar)
           .HasConversion(converter2);

        modelBuilder
            .Entity<WorkScheduleBaseModelDB>()
            .Property(e => e.StartPart)
            .HasConversion(converter);

        modelBuilder
           .Entity<WorkScheduleBaseModelDB>()
           .Property(e => e.EndPart)
           .HasConversion(converter);
    }

    /// <summary>
    /// WorksSchedules
    /// </summary>
    public DbSet<WorkScheduleModelDB> WorksSchedules { get; set; } = default!;

    /// <summary>
    /// WorksSchedulesCalendar
    /// </summary>
    public DbSet<WorkScheduleCalendarModelDB> WorksSchedulesCalendar { get; set; } = default!;

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
    public DbSet<NomenclatureModelDB> Nomenclatures { get; set; } = default!;

    /// <summary>
    /// Offers
    /// </summary>
    public DbSet<OfferModelDB> Offers { get; set; } = default!;

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
    /// OrdersAttendances
    /// </summary>
    public DbSet<OrderAttendanceModelDB> OrdersAttendances { get; set; } = default!;

    /// <summary>
    /// RowsOfOrdersAttendances
    /// </summary>
    public DbSet<RowOfAttendanceModelDB> RowsOfOrdersAttendances { get; set; } = default!;

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