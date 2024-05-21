using Telegram.Bot.Abstract;

namespace Telegram.Bot.Services;

/// <summary>
/// Compose Polling and ReceiverService implementations
/// </summary>
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger) : PollingServiceBase<ReceiverService>(serviceProvider, logger)
{

}