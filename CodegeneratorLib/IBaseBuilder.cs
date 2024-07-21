////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// IBaseBuilder
/// </summary>
public interface IBaseBuilder
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
    public BaseBuilder UseParameter(ParameterModel metadata);

    /// <summary>
    /// AddParameter
    /// </summary>
    public BaseBuilder AddParameter(ParameterModel metadata);

    /// <summary>
    /// WriteSignatureMethod
    /// </summary>
    public BaseBuilder WriteSignatureMethod(StreamWriter writer, string methodName, string? returned = null);

    /// <summary>
    /// AddPaginationPayload
    /// </summary>
    public BaseBuilder AddPaginationPayload(string type_name, string db_set_name);

    /// <summary>
    /// Extract
    /// </summary>
    public T Extract<T>(bool db_inc = true) where T : BaseBuilder;
}