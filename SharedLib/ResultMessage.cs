namespace SharedLib;

/// <summary>
/// Сопроводительное сообщение к результату выполнения операции сервером
/// </summary>
public class ResultMessage
{
    /// <summary>
    /// Тип сообщения (ошибка, инфо и т.п.)
    /// </summary>
    public ResultTypesEnum TypeMessage { get; set; }

    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc/>        
    public override string ToString() => $"({TypeMessage}) {Text}";
}