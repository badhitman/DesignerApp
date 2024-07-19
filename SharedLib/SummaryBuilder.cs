namespace SharedLib;

/// <summary>
/// Summary builder
/// </summary>
public class SummaryBuilder
{
    const string inheritdoc = "<inheritdoc/>";

    Dictionary<string, ParameterModel>? parameters;
    string tabs = "";
    string[]? _summaryText;

    /// <summary>
    /// Payload
    /// </summary>
    public List<string> Payload { get; set; } = [];

    /// <summary>
    /// Method name
    /// </summary>
    public string MethodName { get; set; } = "";

    /// <summary>
    /// Method sign
    /// </summary>
    public string MethodSign { get; set; } = "";

    /// <summary>
    /// Parameters sign
    /// </summary>
    public string ParametersSign { get; set; } = "";

    /// <summary>
    /// Returned
    /// </summary>
    public string? Returned { get; set; }

    /// <summary>
    /// Parameters get text
    /// </summary>
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
                    tabs += "\t";

                if (parameters is not null)
                    FlushParametersText();

                if (_summaryText is not null)
                    FlushSummaryText();
            }

            tabulationsSpiceSize = value;
        }
    }

    /// <summary>
    /// Summary get text
    /// </summary>
    public string SummaryGet { get; set; } = inheritdoc;

    /// <summary>
    /// Write method signature
    /// </summary>
    public SummaryBuilder WriteSignatureMethod(StreamWriter writer, string methodName, string? returned = null)
    {
        MethodName = methodName;
        Returned = returned;
        ParametersSign = "";

        string res = SummaryGet;
        if (ParametersGet is not null)
            res += $"{Environment.NewLine}{ParametersGet}";

        MethodSign = $"Task{(Returned is null ? "" : $"<{Returned}>")} {MethodName}";

        res += $"{Environment.NewLine}{tabs}public {MethodSign}";

        if (parameters?.Count > 0)
            ParametersSign += $"{string.Join(", ", parameters.Select(x => $"{x.Value.Type} {x.Key}"))}";

        res += $"({ParametersSign})";

        writer.WriteLine($"{res};");
        return this;
    }

    /// <summary>
    /// Write implementations
    /// </summary>
    public void WriteImplementation(StreamWriter writer)
    {
        writer.WriteLine($"{tabs}/// <inheritdoc/>");
        writer.WriteLine($"{tabs}public async {MethodSign}({ParametersSign})");
        writer.WriteLine($"{tabs}{{");
        foreach (string p in Payload)
            writer.WriteLine($"{tabs}\t{p}");

        writer.WriteLine($"{tabs}}}");
    }

    /// <summary>
    /// Use parameter
    /// </summary>
    public SummaryBuilder UseParameter(string name, ParameterModel metadata)
    {
        if (parameters is null)
            parameters = new() { { name, metadata } };
        else
        {
            parameters.Clear();
            parameters.Add(name, metadata);
        }
        FlushParametersText();
        return this;
    }

    /// <summary>
    /// Add parameter
    /// </summary>
    public SummaryBuilder AddParameter(string name, ParameterModel metadata)
    {
        if (parameters is null)
            parameters = new() { { name, metadata } };
        else
            parameters.Add(name, metadata);

        FlushParametersText();
        return this;
    }

    /// <summary>
    /// Set summary text
    /// </summary>
    public SummaryBuilder UseSummaryText(string[] summaryText)
    {
        _summaryText = summaryText;
        FlushSummaryText();
        return this;
    }

    /// <summary>
    /// Set summary text
    /// </summary>
    public SummaryBuilder UseSummaryText(string summaryText)
    {
        _summaryText = [summaryText];
        FlushSummaryText();
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public SummaryBuilder AddPayload(string[] payload)
    {
        Payload.AddRange(payload);
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public SummaryBuilder AddPayload(string payload)
    {
        Payload.Add(payload);
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public SummaryBuilder UsePayload(string[] payload)
    {
        Payload.Clear();
        Payload.AddRange(payload);
        return this;
    }

    /// <summary>
    /// Use payload
    /// </summary>
    public SummaryBuilder UsePayload(string payload)
        => UsePayload([payload]);

    /// <summary>
    /// Add pagination payload
    /// </summary>
    public SummaryBuilder AddPaginationPayload(string type_name, string db_set_name)
    {
        AddPayload($"IQueryable<{type_name}>? query = {db_set_name}.AsQueryable();");
        AddPayload($"TPaginationResponseModel<{type_name}> result = new(pagination_request)");
        AddPayload("{");
        AddPayload($"\tTotalRowsCount = await query.CountAsync()");
        AddPayload("};");
        AddPayload("switch (result.SortBy)");
        AddPayload("{");
        AddPayload("\tdefault:");
        AddPayload("\t\tquery = result.SortingDirection == VerticalDirectionsEnum.Up");
        AddPayload("\t\t\t? query.OrderByDescending(x => x.Id)");
        AddPayload("\t\t\t: query.OrderBy(x => x.Id);");
        AddPayload("\t\tbreak;");
        AddPayload("}");
        AddPayload("query = query.Skip((result.PageNum - 1) * result.PageSize).Take(result.PageSize);");
        AddPayload("result.Response = await query.ToListAsync();");

        return this;
    }


    void FlushParametersText()
    {
        ParametersGet = parameters?.Count > 0
        ? $"{string.Join(Environment.NewLine, parameters.Select(x => $"{tabs}/// <param name=\"{x.Key}\">{x.Value.Description}</param>"))}"
        : null;
    }

    void FlushSummaryText()
    {
        if (_summaryText?.Length > 0)
        {
            string res = $"{tabs}/// <summary>";
            foreach (string summary in _summaryText!)
                res = $"{res}{Environment.NewLine}{tabs}/// {summary}";

            SummaryGet = $"{res}{Environment.NewLine}{tabs}/// </summary>";
        }
        else
            SummaryGet = inheritdoc;
    }

    /// <inheritdoc/>
    public SummaryBuilder Constructor(bool db_inc = true)
    {
        if (db_inc)
            Payload.Insert(0, "using LayerContext _db_context = appDbFactory.CreateDbContext();");

        SummaryBuilder res = GlobalTools.CreateDeepCopy(this);
        Payload.Clear();


        return res;
    }
}