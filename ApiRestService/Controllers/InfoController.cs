////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Информация
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute))]
#if !DEBUG
    [LoggerNolog]
#endif
public class InfoController : ControllerBase
{
    /// <summary>
    /// Получить информацию по текущему профилю (проверка токена доступа)
    /// </summary>
    /// <returns>Информация по текущему пользователю (имя и роли)</returns>
    [HttpGet("/api/info/my"), Authorize]
    public ExpressProfileResponseModel GetMyProfile()
    {
        ExpressProfileResponseModel res = new() { UserName = HttpContext.User.Identity?.Name };
        if (string.IsNullOrWhiteSpace(res.UserName))
        {
            res.AddWarning("Сессии не установлено имя");
        }
        res.Roles = HttpContext.User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
        return res;
    }

    /// <summary>
    /// Получить все роли, существующие в системе (публичный доступ без токена)
    /// </summary>
    /// <returns>Все роли, которыми система оперирует впринципе</returns>
    [HttpGet("/api/info/get-all-roles-names")]
    public IEnumerable<string?> GetAllRoles()
    {
        foreach (var item in Enum.GetValues(typeof(ExpressApiRolesEnum)))
        {
            yield return item.ToString();
        }
    }

    /// <summary>
    /// Ответ на запрос от редиректа Identity для входа
    /// </summary>
    /// <returns>Типовой ответ</returns>
    [HttpGet($"/api/values/{nameof(RedirectToLoginPath)}")]
    public ResponseBaseModel RedirectToLoginPath([FromQuery] string? ReturnUrl) => new() { Messages = new List<ResultMessage>() { new() { Text = "Требуется пройти авторизацию", TypeMessage = ResultTypesEnum.Error } } };

    /// <summary>
    /// Ответ на запрос от редиректа Identity для запрета доступа Identity
    /// </summary>
    /// <returns>Типовой ответ</returns>
    [HttpGet($"/api/values/{nameof(RedirectToAccessDeniedPath)}")]
    public ResponseBaseModel RedirectToAccessDeniedPath([FromQuery] string? ReturnUrl) => new() { Messages = new List<ResultMessage>() { new() { Text = "Доступ запрещён", TypeMessage = ResultTypesEnum.Error } } };
}