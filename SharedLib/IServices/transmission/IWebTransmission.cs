////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалённый вызов команд в Web службе
/// </summary>
public interface IWebTransmission
{
    /// <summary>
    /// Получить `web config` сайта
    /// </summary>
    public Task<TelegramBotConfigModel> GetWebConfig();
}