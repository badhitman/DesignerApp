////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib;

/// <summary>
/// FormBaseCore
/// </summary>
public abstract partial class FormBaseCore : DocumenBodyBaseComponent
{
    /// <summary>
    /// Form Metadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormFitModel FormMetadata { get; set; }

    /// <summary>
    /// TabMetadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabFitModel TabMetadata { get; set; }
}
