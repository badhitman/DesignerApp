using Telegram.Bot.Abstract;

namespace Telegram.Bot.Services;

/// <summary>
/// Compose Receiver and UpdateHandler implementation
/// </summary>
/// <inheritdoc/>
public class ReceiverService(
    ITelegramBotClient botClient,
    UpdateHandler updateHandler,
    ILogger<ReceiverServiceBase<UpdateHandler>> logger) : ReceiverServiceBase<UpdateHandler>(botClient, updateHandler, logger)
{

}