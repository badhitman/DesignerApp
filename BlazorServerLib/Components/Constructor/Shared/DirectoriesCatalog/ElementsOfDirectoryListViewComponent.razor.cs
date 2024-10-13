////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

/// <summary>
/// Directory elements-list view
/// </summary>
public partial class ElementsOfDirectoryListViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int SelectedDirectoryId { get; set; }


    /// <inheritdoc/>
    public List<EntryModel>? EntriesElements { get; set; }

    /// <inheritdoc/>
    public async Task ReloadElements(int? selected_directory_id = 0, bool state_has_change = false)
    {
        if (selected_directory_id is not null)
            SelectedDirectoryId = selected_directory_id.Value;

        if (SelectedDirectoryId <= 0)
        {
            EntriesElements = null;
            return;
        }

        IsBusyProgress = true;
        TResponseModel<List<EntryModel>> rest = await ConstructorRepo.GetElementsOfDirectory(SelectedDirectoryId);
        IsBusyProgress = false;

        if (!rest.Success())
            SnackbarRepo.ShowMessagesResponse(rest.Messages);

        EntriesElements = rest.Response;

        if (state_has_change)
            StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (SelectedDirectoryId > 0)
            await ReloadElements();
    }
}