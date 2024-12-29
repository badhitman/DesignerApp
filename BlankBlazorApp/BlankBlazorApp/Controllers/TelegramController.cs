////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLib;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlankBlazorApp.Controllers;

/// <summary>
/// TelegramBotController
/// </summary>
[Route("[controller]/[action]"), ApiController]
[AllowAnonymous]
public class TelegramController(ITelegramRemoteTransmissionService tgRepo, IWebRemoteTransmissionService webRemoteRepo) : ControllerBase
{
    /// <summary>
    /// Authorize
    /// </summary>
    [HttpPost($"/{GlobalStaticConstants.Routes.TELEGRAM_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.AUTHORIZE_CONTROLLER_NAME}")]
    public async Task<IActionResult> Authorize([FromBody] TelegramAuthModel model)
    {
        if (string.IsNullOrEmpty(model.InitData))
            return NotFound(ResponseBaseModel.CreateError("string.IsNullOrEmpty(model.InitData)"));

        System.Collections.Specialized.NameValueCollection data = HttpUtility.ParseQueryString(model.InitData);

        SortedDictionary<string, string> dataDict = new(
            data.AllKeys.ToDictionary(x => x!, x => data[x]!),
            StringComparer.Ordinal);

        string dataCheckString = string.Join('\n', dataDict.Where(x => x.Key != "hash")
            .Select(x => $"{x.Key}={x.Value}"));

        TResponseModel<string?> getTgToken = await tgRepo.GetTelegramBotToken();
        if (!getTgToken.Success() || string.IsNullOrWhiteSpace(getTgToken.Response))
            return NotFound(ResponseBaseModel.CreateError("Не удалось прочитать токен TG бота"));

        byte[] tokenBytes = Encoding.UTF8.GetBytes(getTgToken.Response);
        byte[] secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("WebAppData"),
            tokenBytes);

        byte[] generatedHash = HMACSHA256.HashData(
            secretKey,
            Encoding.UTF8.GetBytes(dataCheckString));

        byte[] actualHash = Convert.FromHexString(dataDict["hash"]);

        if (actualHash.SequenceEqual(generatedHash))
        {
            string telegramUserJsonData = dataDict["user"];
            TelegramUserData userData = JsonConvert.DeserializeObject<TelegramUserData>(telegramUserJsonData)!;
            TResponseModel<CheckTelegramUserAuthModel?> uc = await webRemoteRepo.CheckTelegramUser(CheckTelegramUserHandleModel.Build(userData.Id, userData.FirstName, userData.LastName, userData.UserName, userData.IsBot));
        }

        return NotFound(ResponseBaseModel.CreateError("Ошибка"));

    }
}
