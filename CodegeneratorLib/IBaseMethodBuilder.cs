////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// IBaseMethodBuilder
/// </summary>
public interface IBaseMethodBuilder
{
    /// <summary>
    /// Payload
    /// </summary>
    public List<string>? Payload { get; set; }

    /// <summary>
    /// Параметры
    /// </summary>
    public List<ParameterModel>? Parameters { get; set; }


    /// <summary>
    /// Method sign
    /// </summary>
    public string? MethodSign { get; set; }

    /// <summary>
    /// Parameters get text
    /// </summary>
    public string? ParametersGet { get; set; }

    /// <summary>
    /// Parameters signature
    /// </summary>
    public string? ParametersSign { get; set; }

    /// <summary>
    /// Возвращаемый тип
    /// </summary>
    public string? Returned { get; set; }

    /// <summary>
    /// Method name
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Summary get text
    /// </summary>
    public string? SummaryGet { get; set; }


    /// <summary>
    /// TabulationsSpiceSize
    /// </summary>
    public byte TabulationsSpiceSize { get; set; }


    /// <summary>
    /// Установить новый код метода
    /// </summary>
    public BaseMethodBuilder UsePayload(string payload);

    /// <summary>
    /// Установить новый код метода
    /// </summary>
    public BaseMethodBuilder UsePayload(string[] payload);

    /// <summary>
    /// Добавить код к методу
    /// </summary>
    public BaseMethodBuilder AddPayload(string payload);

    /// <summary>
    /// Добавить код к методу
    /// </summary>
    public BaseMethodBuilder AddPayload(string[] payload);

    /// <summary>
    /// Set new summary text
    /// </summary>
    public BaseMethodBuilder UseSummaryText(string summaryText);

    /// <summary>
    /// Set new summary text
    /// </summary>
    public BaseMethodBuilder UseSummaryText(string[] summaryText);

    /// <summary>
    /// Назначить новый параметр (удаляя все существующие)
    /// </summary>
    public BaseMethodBuilder UseParameter(ParameterModel metadata);

    /// <summary>
    /// Добавить параметр методу
    /// </summary>
    public BaseMethodBuilder AddParameter(ParameterModel metadata);

    /// <summary>
    /// Добавить код для работы с коллекциями (+пагинация)
    /// </summary>
    public BaseMethodBuilder AddPaginationPayload(string type_name, string db_set_name);


    /// <summary>
    /// Flush Parameters text (raw)
    /// </summary>
    public void FlushParametersText();

    /// <summary>
    /// Flush Summary text (raw)
    /// </summary>
    public void FlushSummaryText();


    /// <summary>
    /// Извлекает данные из билдера в новый объект (копию, приведённую к требуемому конечному типа) и очищает все метаданные
    /// </summary>
    public T Extract<T>(bool db_inc = true) where T : BaseMethodBuilder;
}