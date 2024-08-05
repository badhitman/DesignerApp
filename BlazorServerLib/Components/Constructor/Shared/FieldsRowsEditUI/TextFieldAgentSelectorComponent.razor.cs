////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsRowsEditUI;

/// <summary>
/// Text field agent selector
/// </summary>
public partial class TextFieldAgentSelectorComponent : ComponentBase
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action<EntryAltDescriptionModel> SelectAgentHandle { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? SelectedAgent { get; set; }

    /// <inheritdoc/>
    public void SetCurrentAgent(string? selected_agent)
    {
        SelectedAgent = selected_agent;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected bool IsOpenMenu = false;
}
