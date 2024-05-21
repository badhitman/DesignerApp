namespace Telegram.Bot.Abstract;

/// <summary>
/// A marker interface for Update Receiver service
/// </summary>
public interface IReceiverService
{
    /// <summary>
    /// Receive
    /// </summary>
    Task ReceiveAsync(CancellationToken stoppingToken);
}
