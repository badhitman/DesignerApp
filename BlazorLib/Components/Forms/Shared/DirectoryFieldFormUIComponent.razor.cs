using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

/// <summary>
/// Directory field form UI
/// </summary>
public partial class DirectoryFieldFormUIComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ConstructorFormDirectoryLinkModelDB FieldObject { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; }

    /// <inheritdoc/>
    protected IEnumerable<EntryModel> Entries = [];
    /// <inheritdoc/>
    public int SelectedDirectoryField
    {
        get => FieldObject.DirectoryId;
        private set
        {
            FieldObject.DirectoryId = value;
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    /// <inheritdoc/>
    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        EntriesResponseModel rest = await FormsRepo.GetDirectories();
        IsBusyProgress = false;
        StateHasChangedHandler(FieldObject, GetType());

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{C66CD9F5-27EB-4FC6-91CB-61B7307E5858}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Entries is null)
        {
            SnackbarRepo.Add($"Ошибка {{20785094-5951-4F8E-AE4F-98D49C68B602}} rest.Entries is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Entries = rest.Entries;
    }
}