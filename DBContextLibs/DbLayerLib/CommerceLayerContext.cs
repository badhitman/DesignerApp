////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        TimeSpanToTicksConverter converter = new();
        DateOnlyToStringConverter converter2 = new();

        modelBuilder
           .Entity<CalendarScheduleModelDB>()
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
    /// Расписание (по дням недели)
    /// </summary>
    public DbSet<WeeklyScheduleModelDB> WeeklySchedules { get; set; } = default!;

    /// <summary>
    /// Расписание на определённую дату (приоритетное)
    /// </summary>
    public DbSet<CalendarScheduleModelDB> WorksSchedulesCalendars { get; set; } = default!;



    /// <summary>
    /// Организации
    /// </summary>
    public DbSet<OrganizationModelDB> Organizations { get; set; } = default!;

    /// <summary>
    /// Адреса организации (филиалы/офисы)
    /// </summary>
    public DbSet<AddressOrganizationModelDB> AddressesOrganizations { get; set; } = default!;

    /// <summary>
    /// Сотрудники компаний
    /// </summary>
    public DbSet<UserOrganizationModelDB> OrganizationsUsers { get; set; } = default!;

    /// <summary>
    /// Подрядчики
    /// </summary>
    /// <remarks>
    /// Связь организации с офером
    /// </remarks>
    public DbSet<OrganizationContractorModel> ContractorsOrganizations { get; set; } = default!;


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
    /// Регистры учёта остатков оферов в разрезе складов (топиков)
    /// </summary>
    public DbSet<OfferAvailabilityModelDB> OffersAvailability { get; set; } = default!;

    /// <summary>
    /// Locker offers availability
    /// </summary>
    public DbSet<LockTransactionModelDB> LockersTransactions { get; set; } = default!;



    /// <summary>
    /// Заказы на услуги (бронь/запись)
    /// </summary>
    public DbSet<OrderAttendanceModelDB> OrdersAttendances { get; set; } = default!;


    /// <summary>
    /// Заказы товаров со складов
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