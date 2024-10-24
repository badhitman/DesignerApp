////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class HelpdeskLayerContext : DbContext
{
    /// <summary>
    /// Промежуточный/общий слой контекста базы данных
    /// </summary>
    public HelpdeskLayerContext(DbContextOptions options)
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
    /// Рубрики для обращений
    /// </summary>
    public DbSet<RubricIssueHelpdeskModelDB> Rubrics { get; set; }

    /// <summary>
    /// Обращения
    /// </summary>
    public DbSet<IssueHelpdeskModelDB> Issues { get; set; }

    /// <summary>
    /// Подписчики на обращения (участники сверх инициатора и исполнителя. сторонние наблюдатели или помощники/ассистенты)
    /// </summary>
    public DbSet<SubscriberIssueHelpdeskModelDB> SubscribersOfIssues { get; set; }

    /// <summary>
    /// Отметки когда кто просматривал (открывал) обращение. Используется для отслеживания какие обращения уже прочитаны, а в каких есть изменения.
    /// </summary>
    public DbSet<IssueReadMarkerHelpdeskModelDB> IssueReadMarkers { get; set; }

    /// <summary>
    /// Сообщения (ответы) в обращении. Любой пользователь имеющий права может написать сообщение в обращении клиента
    /// </summary>
    public DbSet<IssueMessageHelpdeskModelDB> IssuesMessages { get; set; }

    /// <summary>
    /// Сообщения из обращения любой пользователь может отметить как [являющийся ответом на обращении].
    /// </summary>
    /// <remarks>
    /// Наличие такого признака (или даже нескольких внутри одного обращения) даёт основание полагать, что обращение можно закрывать (отправить в готовое).
    /// </remarks>
    public DbSet<VoteHelpdeskModelDB> Votes { get; set; }

    /// <summary>
    /// Токены доступа к системе. Благодаря этой связи формируется уникальный персональный URL по которому все действия определяются как авторства этого Telegram аккаунта для запуска WebApp
    /// </summary>
    /// <remarks>
    /// По такой ссылке доступ только в HelpDesk.
    /// </remarks>
    public DbSet<AnonymTelegramAccessHelpdeskModelDB> AccessTokens { get; set; }

    /// <summary>
    /// Блокировщики
    /// </summary>
    public DbSet<LockUniqueTokenModelDB> Lockers { get; set; }

    /// <summary>
    /// События в обращениях
    /// </summary>
    public DbSet<PulseIssueModelDB> PulseEvents { get; set; }

    /// <summary>
    /// Пересланные сообщения
    /// </summary>
    public DbSet<ForwardMessageTelegramBotModelDB> ForwardedMessages { get; set; }

    /// <summary>
    /// Ответы на пересланные запросы от пользователей
    /// </summary>
    public DbSet<AnswerToForwardModelDB> AnswersToForwards { get; set; }

    /// <summary>
    /// Articles
    /// </summary>
    public DbSet<ArticleModelDB> Articles { get; set; }

    /// <summary>
    /// RubricsArticles
    /// </summary>
    public DbSet<RubricArticleJoinModelDB> RubricsArticlesJoins { get; set; }
}