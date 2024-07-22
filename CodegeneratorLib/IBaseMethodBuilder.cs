////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
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
    /// Summary get text
    /// </summary>
    public string? SummaryGet { get; set; }

    /// <summary>
    /// TabulationsSpiceSize
    /// </summary>
    public byte TabulationsSpiceSize { get; set; }


    /// <summary>
    /// FlushParametersText
    /// </summary>
    public void FlushParametersText();

    /// <summary>
    /// FlushSummaryText
    /// </summary>
    public void FlushSummaryText();

    /// <summary>
    /// UseParameter
    /// </summary>
    public BaseMethodBuilder UseParameter(ParameterModel metadata);

    /// <summary>
    /// AddParameter
    /// </summary>
    public BaseMethodBuilder AddParameter(ParameterModel metadata);

    /// <summary>
    /// AddPaginationPayload
    /// </summary>
    public BaseMethodBuilder AddPaginationPayload(string type_name, string db_set_name);

    /// <summary>
    /// Extract
    /// </summary>
    public T Extract<T>(bool db_inc = true) where T : BaseMethodBuilder;
}