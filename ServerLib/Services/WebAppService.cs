////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using IdentityLib;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Telegram
/// </summary>
public class WebAppService(
    ITelegramRemoteTransmissionService tgRemoteRepo,
    UserManager<ApplicationUser> userManager,
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    IHttpContextAccessor httpContextAccessor,
    IMailProviderService mailRepo,
    IIdentityRemoteTransmissionService identityRepo,
    IOptions<TelegramBotConfigModel> webConfig,
    ILogger<WebAppService> LoggerRepo)
#pragma warning disable CS9107 // Параметр записан в состоянии включающего типа, а его значение также передается базовому конструктору. Значение также может быть записано базовым классом.
    : GetUserServiceAbstract(httpContextAccessor, userManager, LoggerRepo), IWebAppService
#pragma warning restore CS9107 // Параметр записан в состоянии включающего типа, а его значение также передается базовому конструктору. Значение также может быть записано базовым классом.
{
    #region Telegram
    /// <inheritdoc/>
    public async Task<TResponseModel<CheckTelegramUserAuthModel?>> CheckTelegramUser(CheckTelegramUserHandleModel user)
    {
        TResponseModel<CheckTelegramUserAuthModel?> res = new();
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramUserModelDb? tgUserDb = await identityContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == user.TelegramUserId);

        if (tgUserDb is null)
        {
            using IDbContextTransaction transaction = identityContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            ApplicationUser app_user = new()
            {
                ChatTelegramId = user.TelegramUserId,
                EmailConfirmed = true,

                Email = $"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}",
                NormalizedEmail = userManager.NormalizeEmail($"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}"),

                UserName = $"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}",
                NormalizedUserName = userManager.NormalizeEmail($"tg.{user.TelegramUserId}@{GlobalStaticConstants.FakeHost}"),

                FirstName = user.FirstName,
                NormalizedFirstNameUpper = user.FirstName.ToUpper(),
                LastName = user.LastName,
                NormalizedLastNameUpper = user.LastName?.ToUpper(),
            };

            await identityContext.AddAsync(app_user);
            await identityContext.SaveChangesAsync();

            tgUserDb = TelegramUserModelDb.Build(user, app_user.Id);
            await identityContext.AddAsync(tgUserDb);
            await identityContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        else
        {
            tgUserDb!.FirstName = user.FirstName;
            tgUserDb.NormalizedFirstNameUpper = user.FirstName.ToUpper();

            tgUserDb.LastName = user.LastName;
            tgUserDb.NormalizedLastNameUpper = user.LastName?.ToUpper();

            tgUserDb.Username = user.Username ?? "";
            tgUserDb.NormalizedUserNameUpper = user.Username?.ToUpper();

            tgUserDb.FirstName = user.FirstName;
            tgUserDb.NormalizedFirstNameUpper = user.FirstName.ToUpper();
            tgUserDb.LastName = user.LastName;
            tgUserDb.NormalizedLastNameUpper = user.LastName?.ToUpper();

            tgUserDb.TelegramId = user.TelegramUserId;
            tgUserDb.IsBot = user.IsBot;
            identityContext.Update(tgUserDb);
            await identityContext.SaveChangesAsync();
        }

        ApplicationUser? appUserDb = await identityContext.Users.FirstOrDefaultAsync(x => x.ChatTelegramId == user.TelegramUserId);

        if (appUserDb is not null)
        {
            res.Response = new()
            {
                FirstName = tgUserDb.FirstName,
                LastName = tgUserDb.LastName,
                UserIdentityId = appUserDb.Id,
                TwoFactorEnabled = appUserDb.TwoFactorEnabled,
                UserEmail = appUserDb.Email,
                Username = tgUserDb.Username,
                TelegramId = user.TelegramUserId,
                MainTelegramMessageId = tgUserDb.MainTelegramMessageId,
                AccessFailedCount = appUserDb.AccessFailedCount,
                EmailConfirmed = appUserDb.EmailConfirmed,
                IsBot = user.IsBot,
                LockoutEnd = appUserDb.LockoutEnd,
                PhoneNumber = appUserDb.PhoneNumber,
                PhoneNumberConfirmed = appUserDb.PhoneNumberConfirmed,
                LockoutEnabled = appUserDb.LockoutEnabled,
            };

            if (tgUserDb.UserIdentityId != appUserDb.Id)
            {
                await identityContext
                    .TelegramUsers
                    .Where(x => x.Id == tgUserDb.Id)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.UserIdentityId, appUserDb.Id));
            }

            await identityRepo.ClaimsUserFlush(appUserDb.Id);

        }
        else
            res.AddWarning("Пользователь Telegram не связан с учётной записью на сайте");

        return res;
    }

    async Task<(string id, string email, long? tg)> GetEmailAndIdForUser(string? userId = null)
    {
        string user_id, user_email;
        long? telegram_id;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            TResponseModel<UserInfoModel[]> rest = await identityRepo.GetUsersIdentity([userId]);
            if (!rest.Success() || rest.Response is null || rest.Response.Length != 1 || string.IsNullOrWhiteSpace(rest.Response[0].Email))
                throw new Exception();

            user_id = rest.Response[0].UserId;
            user_email = rest.Response[0].Email!;
            telegram_id = rest.Response[0].TelegramId;
        }
        else
        {
            ApplicationUserResponseModel user = await GetUser(userId);
            if (!user.Success() || user.ApplicationUser is null || string.IsNullOrWhiteSpace(user.ApplicationUser.Email))
                throw new Exception();

            user_id = user.ApplicationUser.Id;
            user_email = user.ApplicationUser.Email;
            telegram_id = user.ApplicationUser.ChatTelegramId;
        }
        return (user_id, user_email, telegram_id);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountState(bool email_notify = false, string? userId = null)
    {
        DateTime lifeTime = DateTime.UtcNow.AddMinutes(-webConfig.Value.TelegramJoinAccountTokenLifetimeMinutes);
        (string id, string email, long? tg) usr = await GetEmailAndIdForUser(userId);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramJoinAccountModelDb? act = await identityContext.TelegramJoinActions
            .FirstOrDefaultAsync(x => x.CreatedAt > lifeTime && x.UserIdentityId == usr.id);

        if (act is null)
            return TResponseModel<TelegramJoinAccountModelDb>.Build(ResponseBaseModel.CreateWarning("Токена нет"));

        if (email_notify)
        {
            if (MailAddress.TryCreate(usr.email, out _))
            {
                string msg;
                TResponseModel<string> bot_username_res = await tgRemoteRepo.GetBotUsername();
                string? bot_username = bot_username_res.Response;

                msg = $"Существует ссылка привязки Telegram аккаунта к учётной записи сайта действительная до {act.CreatedAt.AddMinutes(webConfig.Value.TelegramJoinAccountTokenLifetimeMinutes)} ({DateTime.UtcNow - lifeTime}).<br/>";
                msg += $"Нужно подтвердить операцию через Telegram бота. Для этого нужно в TelegramBot @{bot_username} отправить токен:<br/><u><b>{act.GuidToken}</b></u><br/>Или ссылкой: <a href='https://t.me/{bot_username}?start={act.GuidToken}'>https://t.me/{bot_username}?start={act.GuidToken}</a><br/>";
                await mailRepo.SendEmailAsync(usr.email, "Статус привязки Telegram к у/з", msg);

                await mailRepo.SendEmailAsync(usr.email, "Статус привязки Telegram к у/з", msg);
            }
            else
                LoggerRepo.LogError($"Ошибка уведомления на Email: {usr.email} - email не валидный. error {{BB9E05A4-37A3-4FBB-800B-9AED947A2B3B}}");
        }

        TResponseModel<TelegramJoinAccountModelDb> res = new() { Response = act };
        if (email_notify)
            res.AddAlert($"Проверьте свой ящик Email. Информация вам отправлена");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramJoinAccountModelDb>> TelegramJoinAccountCreate(string? userId = null)
    {
        (string id, string email, long? tg) usr = await GetEmailAndIdForUser(userId);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<TelegramJoinAccountModelDb> actions_del = identityContext.TelegramJoinActions
            .Where(x => x.UserIdentityId == usr.id);

        if (await actions_del.AnyAsync())
            identityContext.RemoveRange(actions_del);

        TelegramJoinAccountModelDb act = new()
        {
            GuidToken = Guid.NewGuid().ToString(),
            UserIdentityId = usr.id,
        };
        await identityContext.AddAsync(act);
        await identityContext.SaveChangesAsync();
        if (MailAddress.TryCreate(usr.email, out _))
        {
            TResponseModel<string> bot_username_res = await tgRemoteRepo.GetBotUsername();
            string? bot_username = bot_username_res.Response;
            //
            string msg = $"Создана ссылка привязки Telegram аккаунта к учётной записи сайта.<br/>";
            msg += $"Нужно подтвердить операцию через Telegram бота. Для этого нужно в TelegramBot @{bot_username} отправить токен:<br/><u><b>{act.GuidToken}</b></u><br/>Или ссылкой: <a href='https://t.me/{bot_username}?start={act.GuidToken}'>https://t.me/{bot_username}?start={act.GuidToken}</a><br/>";
            await mailRepo.SendEmailAsync(usr.email, "Статус привязки Telegram к у/з", msg);
        }

        return new() { Response = act, Messages = [new() { TypeMessage = ResultTypesEnum.Success, Text = "Токен сформирован" }] };
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountDeleteAction(string? userId = null)
    {
        (string id, string email, long? tg) usr = await GetEmailAndIdForUser(userId);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramJoinAccountModelDb[] act = await identityContext.TelegramJoinActions
            .Where(x => x.UserIdentityId == usr.id)
            .ToArrayAsync();
        if (act.Length == 0)
            return ResponseBaseModel.CreateInfo("Токена нет");
        else
        {
            identityContext.RemoveRange(act);
            int i = await identityContext.SaveChangesAsync();

            if (MailAddress.TryCreate(usr.email, out _))
                await mailRepo.SendEmailAsync(usr.email, "Удалён токен привязки Telegram к у/з", "Токен привязки аккаунта Telegram к учётной записи на сайте: удалён.");

            return ResponseBaseModel.CreateSuccess($"Токен удалён");
        }
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramJoinAccountConfirmTokenFromTelegram(TelegramJoinAccountConfirmModel req)
    {
        DateTime lifeTime = DateTime.UtcNow.AddMinutes(-webConfig.Value.TelegramJoinAccountTokenLifetimeMinutes);

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramJoinAccountModelDb? act = await identityContext.TelegramJoinActions
           .FirstOrDefaultAsync(x => x.CreatedAt > lifeTime && x.GuidToken == req.Token);
        if (act is null)
            return ResponseBaseModel.CreateError("Токен не существует");

        ApplicationUser? appUserDb = await identityContext.Users.FirstOrDefaultAsync(x => x.Id == act.UserIdentityId);
        if (appUserDb is null)
            return ResponseBaseModel.CreateError($"Пользователь (identity/{act.UserIdentityId}) для токена [{req.Token}] не найден в БД");

        //
        identityContext.Remove(act);
        await identityContext.SaveChangesAsync();
        //
        appUserDb.ChatTelegramId = req.TelegramId;
        identityContext.Update(appUserDb);
        await identityContext.SaveChangesAsync();

        await identityRepo.ClaimsUserFlush(appUserDb.Id);
        string msg;

        List<ApplicationUser> other_joins = await identityContext.Users
            .Where(x => x.ChatTelegramId == req.TelegramId && x.Id != appUserDb.Id)
            .ToListAsync();

        if (MailAddress.TryCreate(appUserDb.Email, out _))
        {
            msg = $"Аккаунт Telegram {req.TelegramId} подключился к учётной записи сайта воспользовавшись токеном из вашего профиля: <u><b>{act.GuidToken}</b></u>!<br/><br/>";

            msg += "Если это были не вы, то зайдите в профиль на сайте и удалите связь с Telegram.<br />";

            if (other_joins.Count != 0)
                msg += $"Другие привязки к этому Telegram аккаунту аннулируются: {string.Join("; ", other_joins.Select(x => x.Email))}";

            await mailRepo.SendEmailAsync(appUserDb.Email, "Подтверждение токена привязки Telegram к у/з", msg);
        }
        msg = "Токен успешно проверен, запрос на привязку вашего Telegram к учётной записи сформирован. Клиенту отправлено Email оповещение с информацией.";
        if (other_joins.Count != 0)
        {
            other_joins.ForEach(x => x.ChatTelegramId = null);
            identityContext.UpdateRange(other_joins);
            await identityContext.SaveChangesAsync();
        }

        TResponseModel<MessageComplexIdsModel> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт привязан к учётной записи '{appUserDb.Email}' сайта {webConfig.Value.ClearBaseUri}",
            UserTelegramId = req.TelegramId,
            From = "уведомление",
        });
        if (!tgCall.Success())
            LoggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess(msg);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveJoin(long telegramId)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userIdentityDb = await identityContext.Users.FirstOrDefaultAsync(x => x.ChatTelegramId == telegramId);
        if (userIdentityDb is null)
            return ResponseBaseModel.CreateError($"Пользователь с таким TelegramId ({telegramId}) не найден в БД");

        userIdentityDb.ChatTelegramId = null;
        identityContext.Update(userIdentityDb);
        await identityContext.SaveChangesAsync();

        if (MailAddress.TryCreate(userIdentityDb.Email, out _))
        {
            TResponseModel<TelegramUserBaseModel> tg_user_dump = await GetTelegramUserCachedInfo(telegramId);
            await mailRepo.SendEmailAsync(userIdentityDb.Email, "Удаление привязки Telegram к учётной записи", $"Telegram аккаунт {tg_user_dump.Response} отключён от вашей учётной записи на сайте.");
        }

        TelegramUserModelDb tg_user_info = await identityContext.TelegramUsers.FirstAsync(x => x.TelegramId == telegramId);

        await identityContext
            .UserClaims
            .Where(x => x.ClaimType == GlobalStaticConstants.TelegramIdClaimName && x.ClaimValue == userIdentityDb.ChatTelegramId.ToString())
            .ExecuteDeleteAsync();

        TResponseModel<MessageComplexIdsModel> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт отключён от учётной записи {userIdentityDb.Email} с сайта {webConfig.Value.ClearBaseUri}",
            UserTelegramId = tg_user_info.TelegramId,
            From = "уведомление",
        });
        if (!tgCall.Success())
            LoggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess($"Пользователю {userIdentityDb.Email} удалена связь с TelegramId ${telegramId}");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModel>> GetTelegramUserCachedInfo(long telegramId)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TResponseModel<TelegramUserBaseModel> res = new() { Response = TelegramUserBaseModel.Build(await identityContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == telegramId)) };
        if (res.Response is null)
            res.AddInfo($"Пользователь Telegram #{telegramId} не найден в кешэ БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TelegramAccountRemoveJoin(string userId)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser user = identityContext.Users.First(x => x.Id == userId);
        long? tg_user_dump = user.ChatTelegramId;
        user.ChatTelegramId = null;
        identityContext.Update(user);

        if (MailAddress.TryCreate(user.Email, out _))
            await mailRepo.SendEmailAsync(user.Email, "Удаление привязки Telegram к учётной записи", $"Аккаунт Telegram {tg_user_dump} отключён от вашей учётной записи на сайте");

        TResponseModel<MessageComplexIdsModel> tgCall = await tgRemoteRepo.SendTextMessageTelegram(new SendTextMessageTelegramBotModel()
        {
            Message = $"Ваш Telegram аккаунт отключён от учётной записи {user.Email} с сайта {webConfig.Value.ClearBaseUri}",
            UserTelegramId = (await identityContext.TelegramUsers.FirstAsync(x => x.TelegramId == tg_user_dump)).TelegramId,
            From = "уведомление",
        });
        if (!tgCall.Success())
            LoggerRepo.LogError(tgCall.Message());

        return ResponseBaseModel.CreateSuccess($"Пользователю {user.Email} удалена связь с TelegramId");
    }

    /// <inheritdoc/>
    public async Task<TPaginationStrictResponseModel<TelegramUserViewModel>> FindUsersTelegramAsync(FindRequestModel req)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        IQueryable<TelegramUserModelDb> query = identityContext.TelegramUsers
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.FindQuery))
        {
            string find_query = req.FindQuery.ToUpper();
            query = query.Where(x =>
            EF.Functions.Like(x.NormalizedFirstNameUpper, $"%{find_query.ToUpper()}%") ||
            (x.NormalizedUserNameUpper != null && EF.Functions.Like(x.NormalizedUserNameUpper, $"%{find_query.ToUpper()}%")) ||
            (x.NormalizedLastNameUpper != null && EF.Functions.Like(x.NormalizedLastNameUpper, $"%{find_query.ToUpper()}%")));
        }

        int total = query.Count();
        query = query.OrderBy(x => x.Id).Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        TelegramUserModelDb[] users_tg = await query.ToArrayAsync();
        if (users_tg.Length == 0)
            return new() { Response = [] };

        List<long> tg_users_ids = users_tg.Select(y => y.TelegramId).ToList();

        var users_identity_data = await identityContext.Users
            .Where(x => x.ChatTelegramId.HasValue && tg_users_ids.Contains(x.ChatTelegramId.Value))
            .Select(x => new { x.Id, x.Email, x.ChatTelegramId })
            .ToArrayAsync();

        TelegramUserViewModel? identity_get(TelegramUserModelDb ctx)
        {
            var id_data = users_identity_data.FirstOrDefault(x => x.ChatTelegramId == ctx.TelegramId);

            if (id_data is null)
                return null;

            return TelegramUserViewModel.Build(ctx, id_data.Id, id_data?.Email);
        }

#pragma warning disable CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
        return new()
        {
            Response = users_tg.Select(identity_get).Where(x => x is not null).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
#pragma warning restore CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateTelegramMainUserMessage(MainUserMessageModel setMainUserMessage)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        TelegramUserModelDb? user_db = await identityContext.TelegramUsers.FirstOrDefaultAsync(x => x.TelegramId == setMainUserMessage.UserId);
        if (user_db is null)
            return ResponseBaseModel.CreateError($"Пользователь Telegram #{setMainUserMessage.UserId} не найден в БД");
        if (user_db.MainTelegramMessageId == setMainUserMessage.MessageId)
            return ResponseBaseModel.CreateInfo($"Изменения {user_db} не требуются. Идентификатор `{nameof(user_db.MainTelegramMessageId)}` #{setMainUserMessage.MessageId} уже установлен");

        user_db.MainTelegramMessageId = setMainUserMessage.MessageId;
        identityContext.Update(user_db);
        await identityContext.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Успешно. Пользователю {user_db} установлен/обновлён идентификатор `{nameof(user_db.MainTelegramMessageId)}` set:{setMainUserMessage.MessageId}");
    }
    #endregion
}