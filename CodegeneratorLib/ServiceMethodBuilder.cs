////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Service-method builder
/// </summary>
public class ServiceMethodBuilder : BaseMethodBuilder
{
    /// <summary>
    /// Write method signature
    /// </summary>
    public ServiceMethodBuilder WriteSignatureMethod(StreamWriter writer, string methodName, string? returned = null)
    {
        MethodName = methodName;
        Returned = returned;
        ParametersSign = "";

        string? res = SummaryGet;
        if (ParametersGet is not null)
            res += $"{Environment.NewLine}{ParametersGet}";

        MethodSign = $"Task{(Returned is null ? "" : $"<{Returned}>")} {MethodName}";

        res += $"{Environment.NewLine}{tabs}public {MethodSign}";

        if (Parameters?.Count > 0)
            ParametersSign += $"{string.Join(", ", Parameters.Select(x => $"{x.Type} {x.Name}"))}";

        res += $"({ParametersSign})";
        //
        writer.WriteLine($"{res};");
        return this;
    }

    /// <summary>
    /// Write implementations
    /// </summary>
    public ServiceMethodBuilder WriteImplementation(StreamWriter writer)
    {
        if (Payload is null)
            throw new Exception();

        writer.WriteLine($"{tabs}/// <inheritdoc/>");
        writer.WriteLine($"{tabs}public async {MethodSign}({ParametersSign})");
        writer.WriteLine($"{tabs}{{");
        foreach (string p in Payload)
            writer.WriteLine($"{tabs}\t{p}");
        //
        writer.WriteLine($"{tabs}}}");
        return this;
    }

    /// <inheritdoc/>
    public override ServiceMethodBuilder AddPaginationPayload(string type_name, string db_set_name)
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
        //
        return this;
    }

    /// <inheritdoc/>
    public override void FlushParametersText()
    {
        ParametersGet = Parameters?.Count > 0 && _summaryText?.Length > 0
        ? $"{string.Join(Environment.NewLine, Parameters.Select(x => $"{tabs}/// <param name=\"{x.Name}\">{x.Description}</param>"))}"
        : null;
    }

    /// <inheritdoc/>
    public override void FlushSummaryText()
    {
        if (_summaryText?.Length > 0)
        {
            string res = $"{tabs}/// <summary>";
            foreach (string summary in _summaryText!)
                res = $"{res}{Environment.NewLine}{tabs}/// {summary}";

            SummaryGet = $"{res}{Environment.NewLine}{tabs}/// </summary>";
        }
        else
        {
            SummaryGet = $"{tabs}///{inheritdoc}";
            ParametersGet = null;
        }
    }
}