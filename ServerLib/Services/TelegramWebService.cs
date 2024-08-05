////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using IdentityLib;
using SharedLib;
using DbcLib;

namespace ServerLib;

/// <summary>
/// Telegram
/// </summary>
public class TelegramWebService(
    ITelegramRemoteTransmissionService tgRemoteRepo,
    //UserManager<ApplicationUser> userManager,
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    IDbContextFactory<MainDbAppContext> mainContextFactory,
    IHttpContextAccessor httpContextAccessor,
    IMailProviderService mailRepo,
    IOptions<WebConfigModel> webConfig,
    ILogger<TelegramWebService> LoggerRepo)
#pragma warning disable CS9107 // Параметр записан в состоянии включающего типа, а его значение также передается базовому конструктору. Значение также может быть записано базовым классом.
    : GetUserServiceAbstract(httpContextAccessor, identityDbFactory), ITelegramWebService
#pragma warning restore CS9107 // Параметр записан в состоянии включающего типа, а его значение также передается базовому конструктору. Значение также может быть записано базовым классом.
{
    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
    {
        TResponseModel<CheckTelegramUserModel?> res = new();
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TelegramUserModelDb? tgUserDb = await mainContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == user.TelegramUserId);
        res.Response = tgUserDb is null ? null : CheckTelegramUserModel.Build(tgUserDb);
        if (res.Response is null)
        {
            res.Response = CheckTelegramUserModel.Build(user);
            tgUserDb = TelegramUserModelDb.Build(user);
            mainContext.Add(tgUserDb);
            mainContext.SaveChanges();
        }
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IdentityUserRecord? userIdentityDb = null;

        try
        {
            userIdentityDb = await identityContext.Users
                        .Where(x => x.TelegramId == user.TelegramUserId)
                        .Select(x => new IdentityUserRecord(
                            x.Email,
                            x.EmailConfirmed,
                            x.PhoneNumber,
                            x.PhoneNumberConfirmed,
                            x.TwoFactorEnabled,
                            x.LockoutEnd,
                            x.LockoutEnabled,
                            x.AccessFailedCount
                        ))
                        .SingleOrDefaultAsync();
        }
        catch (Exception ex)
        {
            res.Messages.InjectException(ex);
            return res;
        }

        if (userIdentityDb is not null)
        {
            res.Response!.UserEmail = userIdentityDb.Email;
            res.Response.PhoneNumber = userIdentityDb.PhoneNumber;
            res.Response.PhoneNumberConfirmed = userIdentityDb.PhoneNumberConfirmed;
            res.Response.EmailConfirmed = userIdentityDb.EmailConfirmed;
            res.Response.AccessFailedCount = userIdentityDb.AccessFailedCount;
            res.Response.LockoutEnd = userIdentityDb.LockoutEnd;
            res.Response.LockoutEnabled = userIdentityDb.LockoutEnabled;
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountState(bool email_notify = false, string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<TelegramJoinAccountModelDb>() { Messages = user.Messages };

        DateTime lifeTime = DateTime.Now.AddMinutes(-webConfig.Value.TelegramJoinAccountTokenLifetimeMinutes);
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TelegramJoinAccountModelDb? act = await mainContext.TelegramJoinActions
            .FirstOrDefaultAsync(x => x.CreatedAt > lifeTime && x.UserIdentityId == user.ApplicationUser.Id);

        if (act is null)
            return TResponseModel<TelegramJoinAccountModelDb>.Build(ResponseBaseModel.CreateWarning("Токена нет"));

        if (email_notify)
        {
            if (MailAddress.TryCreate(user.ApplicationUser.Email, out _))
            {
                string msg;
                TResponseModel<string?> bot_username_res = await tgRemoteRepo.GetBotUsername();
                string? bot_username = bot_username_res.Response;

                msg = $"Существует ссылка привязки Telegram аккаунта к учётной записи сайта действительная до {act.CreatedAt.AddMinutes(webConfig.Value.TelegramJoinAccountTokenLifetimeMinutes)} ({DateTime.Now - lifeTime}).<br/>";
                msg += $"Нужно подтвердить операцию через Telegram бота. Для этого нужно в TelegramBot @{bot_username} отправить токен:<br/><u><b>{act.GuidToken}</b></u><br/>Или ссылкой: <a href='https://t.me/{bot_username}?start={act.GuidToken}'>https://t.me/{bot_username}?start={act.GuidToken}</a><br/>";
                await mailRepo.SendEmailAsync(user.ApplicationUser.Email, "Статус привязки Telegram к у/з", msg);

                await mailRepo.SendEmailAsync(user.ApplicationUser.Email, "Статус привязки Telegram к у/з", msg);
            }
            else
                LoggerRepo.LogError($"Ошибка уведомления на Email: {user.ApplicationUser.Email} - email не валидный. error {{BB9E05A4-37A3-4FBB-800B-9AED947A2B3B}}");
        }

        TResponseModel<TelegramJoinAccountModelDb> res = new() { Response = act };
        if (email_notify)
            res.AddAlert($"Проверьте свой ящик Email. Информация вам отправлена");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountCreate(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return new TResponseModel<TelegramJoinAccountModelDb>() { Messages = user.Messages };

        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        IQueryable<TelegramJoinAccountModelDb> actions_del = mainContext.TelegramJoinActions
            .Where(x => x.UserIdentityId == user.ApplicationUser.Id);

        if (await actions_del.AnyAsync())
            mainContext.RemoveRange(actions_del);

        TelegramJoinAccountModelDb act = new()
        {
            GuidToken = Guid.NewGuid().ToString(),
            UserIdentityId = user.ApplicationUser.Id
        };
        await mainContext.AddAsync(act);
        await mainContext.SaveChangesAsync();
        if (MailAddress.TryCreate(user.ApplicationUser.Email, out _))
        {
            TResponseModel<string?> bot_username_res = await tgRemoteRepo.GetBotUsername();
            string? bot_username = bot_username_res.Response;
            //
            string msg = $"Создана ссылка привязки Telegram аккаунта к учётной записи сайта.<br/>";
            msg += $"Нужно подтвердить операцию через Telegram бота. Для этого нужно в TelegramBot @{bot_username} отправить токен:<br/><u><b>{act.GuidToken}</b></u><br/>Или ссылкой: <a href='https://t.me/{bot_username}?start={act.GuidToken}'>https://t.me/{bot_username}?start={act.GuidToken}</a><br/>";
            await mailRepo.SendEmailAsync(user.ApplicationUser.Email, "Статус привязки Telegram к у/з", msg);
        }

        return new TResponseModel<TelegramJoinAccountModelDb>() { Response = act, Messages = [new() { TypeMessage = ResultTypesEnum.Success, Text = "Токен сформирован" }] };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDeleteAction(string? userId = null)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return ResponseBaseModel.Create(user.Messages);

        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TelegramJoinAccountModelDb[] act = await mainContext.TelegramJoinActions
            .Where(x => x.UserIdentityId == user.ApplicationUser.Id)
            .ToArrayAsync();
        if (act.Length == 0)
            return ResponseBaseModel.CreateInfo("Токена нет");
        else
        {
            mainContext.RemoveRange(act);
            int i = await mainContext.SaveChangesAsync();

            if (MailAddress.TryCreate(user.ApplicationUser.Email, out _))
                await mailRepo.SendEmailAsync(user.ApplicationUser.Email, "Удалён токен привязки Telegram к у/з", "Токен привязки аккаунта Telegram к учётной записи на сайте: удалён.");

            return ResponseBaseModel.CreateSuccess($"Токен удалён");
        }
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountConfirmTokenFromTelegram(TelegramJoinAccountConfirmModel req)
    {
        DateTime lifeTime = DateTime.Now.AddMinutes(-webConfig.Value.TelegramJoinAccountTokenLifetimeMinutes);
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TelegramJoinAccountModelDb? act = await mainContext.TelegramJoinActions
           .FirstOrDefaultAsync(x => x.CreatedAt > lifeTime && x.GuidToken == req.Token);
        if (act is null)
            return ResponseBaseModel.CreateError("Токен не существует");


        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userIdentityDb = await identityContext.Users.FirstOrDefaultAsync(x => x.Id == act.UserIdentityId);
        if (userIdentityDb is null)
            return ResponseBaseModel.CreateError($"Пользователь (identity/{act.UserIdentityId}) для токена [{req.Token}] не найден в БД");

        //
        mainContext.Remove(act);
        await mainContext.SaveChangesAsync();
        //
        userIdentityDb.TelegramId = req.TelegramUser.TelegramId;
        identityContext.Update(userIdentityDb);
        await identityContext.SaveChangesAsync();

        string msg;

        List<ApplicationUser> other_joins = await identityContext.Users
            .Where(x => x.TelegramId == req.TelegramUser.TelegramId && x.Id != userIdentityDb.Id)
            .ToListAsync();

        if (MailAddress.TryCreate(userIdentityDb.Email, out _))
        {
            msg = $"Аккаунт Telegram {req.TelegramUser} подключился к учётной записи сайта воспользовавшись токеном из вашего профиля: <u><b>{act.GuidToken}</b></u>!<br/><br/>";

            msg += "Если это были не вы, то зайдите в профиль на сайте и удалите связь с Telegram.<br />";

            if (other_joins.Count != 0)
                msg += $"Другие привязки к этому Telegram аккаунту аннулируются: {string.Join("; ", other_joins.Select(x => x.Email))}";

            await mailRepo.SendEmailAsync(userIdentityDb.Email, "Подтверждение токена привязки Telegram к у/з", msg);
        }
        msg = "Токен успешно проверен, запрос на привязку вашего Telegram к учётной записи сформирован. Клиенту отправлено Email оповещение с информацией.";
        if (other_joins.Count != 0)
        {
            other_joins.ForEach(x => x.TelegramId = null);
            identityContext.UpdateRange(other_joins);
            await identityContext.SaveChangesAsync();
        }

        TResponseModel<int?> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт привязан к учётной записи '{userIdentityDb.Email}' сайта {webConfig.Value.ClearBaseUri}",
            UserTelegram = req.TelegramUser,
            From = "уведомление",
        });
        if (!tgCall.Success())
            LoggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModelDb?>> GetTelegramUserCachedInfo(long telegramId)
    {
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TResponseModel<TelegramUserBaseModelDb?> res = new() { Response = TelegramUserBaseModelDb.Build(await mainContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == telegramId)) };
        if (res.Response is null)
            res.AddError($"Пользователь Telegram #{telegramId} не найден в кешэ БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveJoin(long telegramId)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userIdentityDb = await identityContext.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
        if (userIdentityDb is null)
            return ResponseBaseModel.CreateError($"Пользователь с таким TelegramId ({telegramId}) не найден в БД");

        userIdentityDb.TelegramId = null;
        identityContext.Update(userIdentityDb);
        await identityContext.SaveChangesAsync();

        if (MailAddress.TryCreate(userIdentityDb.Email, out _))
        {
            TResponseModel<TelegramUserBaseModelDb?> tg_user_dump = await GetTelegramUserCachedInfo(telegramId);
            await mailRepo.SendEmailAsync(userIdentityDb.Email, "Удаление привязки Telegram к учётной записи", $"Telegram аккаунт {tg_user_dump.Response} отключён от вашей учётной записи на сайте.");
        }
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TelegramUserModelDb tg_user_info = await mainContext.TelegramUsers.FirstAsync(x => x.TelegramId == telegramId);
        TResponseModel<int?> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт отключён от учётной записи {userIdentityDb.Email} с сайта {webConfig.Value.ClearBaseUri}",
            UserTelegram = tg_user_info,
            From = "уведомление",
        });
        if (!tgCall.Success())
            LoggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess($"Пользователю {userIdentityDb.Email} удалена связь с TelegramId ${telegramId}");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveJoin(string userId)
    {
        ApplicationUserResponseModel user = await GetUser(userId);
        if (!user.Success() || user.ApplicationUser is null)
            return ResponseBaseModel.Create(user.Messages);

        if (user.ApplicationUser.TelegramId is null)
            return ResponseBaseModel.CreateInfo("Пользователю не назначен TelegramId");

        long telegram_id = user.ApplicationUser.TelegramId!.Value;
        TResponseModel<TelegramUserBaseModelDb?> tg_user_dump = await GetTelegramUserCachedInfo(telegram_id);
        user.ApplicationUser.TelegramId = null;

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        identityContext.Update(user.ApplicationUser);

        if (MailAddress.TryCreate(user.ApplicationUser.Email, out _))
            await mailRepo.SendEmailAsync(user.ApplicationUser.Email, "Удаление привязки Telegram к учётной записи", $"Аккаунт Telegram {tg_user_dump} отключён от вашей учётной записи на сайте");

        TResponseModel<int?> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт отключён от учётной записи {user.ApplicationUser.Email} с сайта {webConfig.Value.ClearBaseUri}",
            UserTelegram = tg_user_dump.Response!,
            From = "уведомление",
        });
        if (!tgCall.Success())
            LoggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess($"Пользователю {user.ApplicationUser.Email} удалена связь с TelegramId");
    }

    /// <inheritdoc/>
    public async Task<TPaginationStrictResponseModel<TelegramUserViewModel>> FindUsersTelegramAsync(FindRequestModel req)
    {
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        IQueryable<TelegramUserModelDb> query = mainContext.TelegramUsers
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.FindQuery))
        {
            string find_query = req.FindQuery.ToUpper();
            query = query.Where(x =>
            EF.Functions.Like(x.NormalizedFirstName, $"%{find_query.ToUpper()}%") ||
            (x.NormalizedName != null && EF.Functions.Like(x.NormalizedName, $"%{find_query.ToUpper()}%")) ||
            (x.NormalizedLastName != null && EF.Functions.Like(x.NormalizedLastName, $"%{find_query.ToUpper()}%")));
        }

        int total = query.Count();
        query = query.Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        TelegramUserModelDb[] users_tg = await query.ToArrayAsync();
        if (users_tg.Length == 0)
            return new TPaginationStrictResponseModel<TelegramUserViewModel>() { Response = [] };

        List<long> tg_users_ids = users_tg.Select(y => y.TelegramId).ToList();

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        var users_identity_data = await identityContext.Users
            .Where(x => x.TelegramId.HasValue && tg_users_ids.Contains(x.TelegramId.Value))
            .Select(x => new { x.Id, x.Email, x.TelegramId })
            .ToArrayAsync();

        TelegramUserViewModel identity_get(TelegramUserModelDb ctx)
        {
            var id_data = users_identity_data.FirstOrDefault(x => x.TelegramId == ctx.TelegramId);
            return TelegramUserViewModel.Build(ctx, id_data?.Id, id_data?.Email);
        }

        return new TPaginationStrictResponseModel<TelegramUserViewModel>()
        {
            Response = users_tg.Select(x => identity_get(x)).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainUserMessage)
    {
        using MainDbAppContext mainContext = mainContextFactory.CreateDbContext();
        TelegramUserModelDb? user_db = await mainContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == setMainUserMessage.UserId);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь Telegram #{setMainUserMessage.UserId} не найден в БД");
        if (user_db.MainTelegramMessageId == setMainUserMessage.MessageId)
            return ResponseBaseModel.CreateInfo($"Изменения {user_db} не требуются. Идентификатор `{nameof(user_db.MainTelegramMessageId)}` #{setMainUserMessage.MessageId} уже установлен");

        user_db.MainTelegramMessageId = setMainUserMessage.MessageId;
        mainContext.Update(user_db);
        await mainContext.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Успешно. Пользователю {user_db} установлен/обновлён идентификатор `{nameof(user_db.MainTelegramMessageId)}` set:{setMainUserMessage.MessageId}");
    }
}