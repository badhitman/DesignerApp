////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using Newtonsoft.Json;

namespace CodegeneratorLib;

/// <summary>
/// Base builder
/// </summary>
public abstract class BaseMethodBuilder : IBaseMethodBuilder
{
    /// <summary>
    /// inheritdoc
    /// </summary>
    public static readonly string inheritdoc = "<inheritdoc/>";


    /// <summary>
    /// Summary text
    /// </summary>
    protected string[]? _summaryText;

    /// <inheritdoc/>
    public List<string>? Payload { get; set; }

    /// <summary>
    /// Параметры
    /// </summary>
    public List<ParameterModel>? Parameters { get; set; }

    /// <summary>
    /// Табы для отступов
    /// </summary>
    protected string tabs = "";

    byte tabulationsSpiceSize;
    /// <summary>
    /// Размер отступа в табуляторах
    /// </summary>
    public required byte TabulationsSpiceSize
    {
        get => tabulationsSpiceSize;
        set
        {
            if (tabulationsSpiceSize != value)
            {
                tabs = "";
                for (int i = 0; i < value; i++)
                    tabs += "\t";

                FlushParametersText();
                FlushSummaryText();
            }
            //
            tabulationsSpiceSize = value;
        }
    }

    /// <inheritdoc/>    
    public string? SummaryGet { get; set; }

    /// <summary>
    /// Parameters signature
    /// </summary>
    public string? ParametersSign { get; set; }

    /// <summary>
    /// Method name
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Возвращаемый тип
    /// </summary>
    public string? Returned { get; set; }

    /// <summary>
    /// Method sign
    /// </summary>
    public string? MethodSign { get; set; }

    /// <summary>
    /// Parameters get text
    /// </summary>
    public string? ParametersGet { get; set; }



    /// <summary>
    /// Use parameter
    /// </summary>
    public virtual BaseMethodBuilder UseParameter(ParameterModel metadata)
    {
        if (Parameters is null)
            Parameters = new() { { metadata } };
        else
        {
            Parameters.Clear();
            Parameters.Add(metadata);
        }
        //
        FlushParametersText();
        return this;
    }

    /// <summary>
    /// Set summary text
    /// </summary>
    public BaseMethodBuilder UseSummaryText(string[] summaryText)
    {
        _summaryText = summaryText;
        FlushSummaryText();
        //
        return this;
    }

    /// <summary>
    /// Set summary text
    /// </summary>
    public BaseMethodBuilder UseSummaryText(string summaryText)
    {
        _summaryText = [summaryText];
        FlushSummaryText();
        //
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public BaseMethodBuilder AddPayload(string[] payload)
    {
        Payload ??= [];
        Payload.AddRange(payload);
        //
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public BaseMethodBuilder AddPayload(string payload)
    {
        Payload ??= [];
        Payload.Add(payload);
        //
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public BaseMethodBuilder UsePayload(string[] payload)
    {
        if (Payload is null)
            Payload = [];
        else
            Payload.Clear();

        Payload.AddRange(payload);
        //
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public BaseMethodBuilder UsePayload(string payload)
        => UsePayload([payload]);

    /// <inheritdoc/>
    public abstract void FlushParametersText();

    /// <inheritdoc/>
    public abstract void FlushSummaryText();

    /// <inheritdoc/>
    public abstract BaseMethodBuilder AddPaginationPayload(string type_name, string db_set_name);

    /// <inheritdoc/>
    public BaseMethodBuilder AddParameter(ParameterModel metadata)
    {
        if (Parameters is null)
            Parameters = new() { { metadata } };
        else
            Parameters.Add(metadata);
        //
        FlushParametersText();
        return this;
    }

    /// <summary>
    /// Возвращает копию текущего объекта и очищает все метаданные билдера
    /// </summary>
    public T Extract<T>(bool db_inc = true) where T : BaseMethodBuilder
    {
        if (db_inc)
            Payload!.Insert(0, "using LayerContext _db_context = appDbFactory.CreateDbContext();");

        string json_raw = JsonConvert.SerializeObject(this);

        Payload?.Clear();
        Parameters?.Clear();
        //
        ParametersSign = null;
        MethodName = null;
        Returned = null;
        SummaryGet = null;
        MethodSign = null;
        ParametersGet = null;
        //
        _summaryText = null;
        Parameters = null;
        //
        return JsonConvert.DeserializeObject<T>(json_raw) ?? throw new Exception($"Ошибка десериализации [{typeof(T).FullName}] {json_raw}");
    }
}