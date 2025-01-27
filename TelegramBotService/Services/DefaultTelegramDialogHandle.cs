////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;
using System.Net.Mail;

namespace TelegramBotService;

/// <summary>
/// Default: handle telegram dialog
/// </summary>
public class DefaultTelegramDialogHandle(IIdentityTransmission identityRepo, ILogger<DefaultTelegramDialogHandle> _logger, TelegramBotConfigModel webConf)
    : ITelegramDialogService
{
    /// <inheritdoc/>
    public async Task<TelegramDialogResponseModel> TelegramDialogHandle(TelegramDialogRequestModel tgDialog)
    {
        TelegramDialogResponseModel resp = new()
        {
            Response = $"Привет {tgDialog.TelegramUser}!\n",
            MainTelegramMessageId = tgDialog.TelegramUser.MainTelegramMessageId
        };
        switch (tgDialog.MessageText.ToLower())
        {
            case "/logout":
                resp.Response += "Действительно выйти?.";
                resp.ReplyKeyboard = [[new ButtonActionModel() { Data = "/logout.2", Title = "Подтвердить выход" }]];
                break;
            case "/logout.2":
                ResponseBaseModel rest = await identityRepo.TelegramJoinAccountDelete(new() { TelegramId = tgDialog.TelegramUser.TelegramId, ClearBaseUri = webConf.ClearBaseUri ?? "https://" });
                if (!rest.Success())
                {
                    _logger.LogError(rest.Message());
                    resp.Response = $"Во время выполнения запроса произошла ошибка: {rest.Message()}";
                    resp.ReplyKeyboard = [[new ButtonActionModel() { Data = "/start", Title = "Проверить?" }]];
                }
                else
                {
                    resp.Response = $"Учётная запись на сайте больше не связана с Telegram аккаунтом [{tgDialog.TelegramUser}]. Для повторной ассоциации Telegram аккаунта с у/з сайта необходимо заново пройти процедуру привязки {webConf.BaseUri}.";
                    resp.MainTelegramMessageId = null;
                    _logger.LogInformation(resp.Response);
                }
                break;
            default:
                if (tgDialog.TelegramUser is CheckTelegramUserAuthModel ctu && MailAddress.TryCreate(ctu.UserEmail, out MailAddress? ma) && ma is not null)
                {
                    if (ma.Host != GlobalStaticConstants.FakeHost)
                    {
                        resp.Response += "Для удаления связи с учётной записью сайта нажмите 'Выход'.";
                        resp.ReplyKeyboard = [[new ButtonActionModel() { Data = "/logout", Title = "Выход" }]];
                    }
                }
                else
                {
                    resp.Response += $"Вы не авторизованы. Привяжите свой Telegram аккаунт к учётной записи на сайте <a href='{webConf.BaseUri}Account/Manage/Telegram'>{webConf.BaseUri}Account/Manage/Telegram</a>, что бы получить доступ";
                }
                resp.MainTelegramMessageId = tgDialog.TypeMessage == MessagesTypesEnum.TextMessage
                    ? null
                    : resp.MainTelegramMessageId;
                break;
        }

        return resp;
    }
}