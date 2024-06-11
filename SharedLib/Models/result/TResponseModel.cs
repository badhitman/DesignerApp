////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////


namespace SharedLib;

/// <summary>
/// Базовая модель ответа/результата на запрос
/// </summary>
public class TResponseModel<T> : ResponseBaseModel
{
    /// <inheritdoc/>
    public TResponseModel() { }

    /// <inheritdoc/>
    public TResponseModel(IEnumerable<ResultMessage> messages) { Messages = messages.ToList(); }

    /// <inheritdoc/>
    public T? Response { get; set; }

    /// <inheritdoc/>
    public static TResponseModel<T> Build(ResponseBaseModel sender)
    {
        return new TResponseModel<T>() { Messages = sender.Messages };
    }
}
/// <summary>
/// Базовая модель ответа/результата на запрос
/// </summary>
public class TResponseStrictModel<T> : ResponseBaseModel
{
    /// <inheritdoc/>
    public required T Response { get; set; }

    /// <inheritdoc/>
    public static TResponseModel<T> Build(ResponseBaseModel sender)
    {
        return new TResponseModel<T>() { Messages = sender.Messages };
    }
}