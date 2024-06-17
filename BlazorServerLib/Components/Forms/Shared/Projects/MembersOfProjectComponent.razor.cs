using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <summary>
/// MembersOfProject
/// </summary>
public partial class MembersOfProjectComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Project Id
    /// </summary>
    [Parameter, EditorRequired]
    public required ProjectViewModel ProjectView { get; set; }

    /// <inheritdoc/>
    public async Task AddMemeber()
    {

    }

    /// <inheritdoc/>
    public async Task RemoveMember(string user_id)
    {

    }
}