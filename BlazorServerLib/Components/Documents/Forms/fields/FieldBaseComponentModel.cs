////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;

namespace BlazorWebLib;

/// <summary>
/// FieldBaseComponent
/// </summary>
public class FieldBaseComponentModel : ComponentBase
{
    /// <inheritdoc/>
    [Parameter]
    public string? Id { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Label { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Hint { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool Required { get; set; }
}