////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using Telegram.Bot;
using SharedLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить файл из Telegram
/// </summary>
public class GetFileTelegramReceive(ITelegramBotClient _botClient)
    : IResponseReceive<string, TResponseModel<byte[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadFileTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]>?> ResponseHandleAction(string? fileId)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        TResponseModel<byte[]> res = new();
        try
        {
            Telegram.Bot.Types.File fileTg = await _botClient.GetFile(fileId);
            MemoryStream ms = new();

            if (string.IsNullOrWhiteSpace(fileTg.FilePath))
            {
                res.AddError($"Ошибка получения {nameof(fileTg.FilePath)}");
                return res;
            }

            await _botClient.DownloadFile(fileTg.FilePath, ms);
            res.Response = ms.ToArray();
        }
        catch (Exception ex)
        {
            res.AddError(ex.Message);
        }
        return res;
    }
}