namespace SharedLib;

/// <inheritdoc/>
public class TelegramJoinAccountResponseModel : ResponseBaseModel
{
    /// <inheritdoc/>
    public TelegramJoinAccountModelDB? TelegramJoinAccount { get; set; }

    /// <inheritdoc/>
    public static TelegramJoinAccountResponseModel Build(ResponseBaseModel responseBaseModel)
    {
        return new TelegramJoinAccountResponseModel() { Messages = responseBaseModel.Messages };
    }
}