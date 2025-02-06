////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая модель ответа/результата на запрос
/// </summary>
public class TResponseMQModel<T> : ResponseBaseModel
{
    /// <summary>
    /// Получен запрос
    /// </summary>
    public DateTime StartedServer { get; set; }

    /// <summary>
    /// Запрос обработан
    /// </summary>
    public DateTime FinalizedServer { get; set; }

    /// <summary>
    /// Duration
    /// </summary>
    public TimeSpan Duration() => FinalizedServer - StartedServer;

    /// <summary>
    /// Базовая модель ответа/результата на запрос
    /// </summary>
    public TResponseMQModel() { }

    /// <summary>
    /// Базовая модель ответа/результата на запрос
    /// </summary>
    public TResponseMQModel(IEnumerable<ResultMessage> messages) { Messages = messages.ToList(); }

    /// <summary>
    /// Полезная нагрузка ответа
    /// </summary>
    public T? Response { get; set; }

    /// <inheritdoc/>
    public static TResponseModel<T> Build(ResponseBaseModel sender)
    {
        return new() { Messages = sender.Messages };
    }
}
