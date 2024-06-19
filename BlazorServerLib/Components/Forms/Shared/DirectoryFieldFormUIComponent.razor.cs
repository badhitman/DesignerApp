using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

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
    [CascadingParameter]
    public Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }


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
        TResponseModel<EntryModel[]> rest = await FormsRepo.GetDirectories(Form.ProjectId);
        IsBusyProgress = false;
        StateHasChangedHandler(FieldObject, GetType());

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка 4EDB5A8E-E5E6-4DA3-A269-1C5128C1A2E9 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка 71E3091D-2EFE-43E8-A4CE-43934A73A8CF rest.Entries is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Entries = rest.Response;
    }
}