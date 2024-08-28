////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
        return new() { Messages = sender.Messages };
    }
}