using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

public partial class TextFieldAgentSelectorComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public Action<EntryAltDescriptionModel> SelectAgentHandle { get; set; } = default!;

    [Parameter]
    public string? SelectedAgent { get; set; }

    public void SetCurrentAgent(string? selected_agent)
    {
        SelectedAgent = selected_agent;
        StateHasChanged();
    }

    protected bool IsOpenMenu = false;
}
