////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using HtmlGenerator.html5;
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


    /// <inheritdoc/>
    public string[]? SummaryText;

    /// <inheritdoc/>
    public List<string>? Payload { get; set; }

    /// <inheritdoc/>
    public List<ParameterModel>? Parameters { get; set; }


    /// <summary>
    /// Табы для отступов
    /// </summary>
    protected string tabs = "";

    /// <inheritdoc/>    
    public string? SummaryGet { get; set; }

    /// <inheritdoc/>   
    public string? ParametersSign { get; set; }

    /// <inheritdoc/>   
    public string? MethodName { get; set; }

    /// <inheritdoc/>   
    public string? Returned { get; set; }

    /// <inheritdoc/>  
    public string? MethodSign { get; set; }

    /// <inheritdoc/>  
    public string? ParametersGet { get; set; }


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
                    tabs += base_dom_root.TabString;

                FlushParametersText();
                FlushSummaryText();
            }
            //
            tabulationsSpiceSize = value;
        }
    }


    /// <inheritdoc/>
    public virtual BaseMethodBuilder AddParameter(ParameterModel metadata)
    {
        if (Parameters is null)
            Parameters = new() { { metadata } };
        else
            Parameters.Add(metadata);
        //
        FlushParametersText();
        return this;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual BaseMethodBuilder UseSummaryText(string[] summaryText)
    {
        SummaryText = summaryText;
        FlushSummaryText();
        //
        return this;
    }

    /// <inheritdoc/>
    public virtual BaseMethodBuilder UseSummaryText(string summaryText)
    {
        SummaryText = [summaryText];
        FlushSummaryText();
        //
        return this;
    }

    /// <inheritdoc/>
    public virtual BaseMethodBuilder AddPayload(string[] payload)
    {
        Payload ??= [];
        Payload.AddRange(payload);
        //
        return this;
    }

    /// <inheritdoc/>
    public virtual BaseMethodBuilder AddPayload(string payload)
    {
        Payload ??= [];
        Payload.Add(payload);
        //
        return this;
    }

    /// <inheritdoc/>
    public virtual BaseMethodBuilder UsePayload(string[] payload)
    {
        if (Payload is null)
            Payload = [];
        else
            Payload.Clear();

        Payload.AddRange(payload);
        //
        return this;
    }

    /// <inheritdoc/>
    public virtual BaseMethodBuilder UsePayload(string payload)
        => UsePayload([payload]);

    /// <inheritdoc/>
    public abstract BaseMethodBuilder AddPaginationPayload(string type_name, string db_set_name);


    /// <inheritdoc/>
    public abstract void FlushParametersText();

    /// <inheritdoc/>
    public abstract void FlushSummaryText();


    /// <inheritdoc/>
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
        SummaryText = null;
        Parameters = null;
        //
        return JsonConvert.DeserializeObject<T>(json_raw) ?? throw new Exception($"Ошибка десериализации [{typeof(T).FullName}] {json_raw}");
    }
}