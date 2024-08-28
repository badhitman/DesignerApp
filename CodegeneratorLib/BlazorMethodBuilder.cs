////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Blazor method builder
/// </summary>
public class BlazorMethodBuilder : BaseMethodBuilder
{
    /// <inheritdoc/>
    public override BaseMethodBuilder AddPaginationPayload(string type_name, string db_set_name)
    {
        return this;
    }

    /// <inheritdoc/>
    public override void FlushParametersText()
    {

    }

    /// <inheritdoc/>
    public override void FlushSummaryText()
    {

    }
}