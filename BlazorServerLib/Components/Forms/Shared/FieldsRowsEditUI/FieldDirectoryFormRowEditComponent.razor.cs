using BlazorLib;
using BlazorLib.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsRowsEditUI;

/// <summary>
/// Field directory form row edit
/// </summary>
public partial class FieldDirectoryFormRowEditComponent : BlazorBusyComponentBaseModel, IDomBaseComponent
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    ///// <inheritdoc/>
    //[CascadingParameter, EditorRequired]
    //public ConstructorFormModelDB Form { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public Action<ConstructorFormDirectoryLinkModelDB> StateHasChangedHandler { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public ConstructorFormDirectoryLinkModelDB Field { get; set; } = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryModel> Entries = default!;

    /// <inheritdoc/>
    public string DomID => $"{Field.GetType().FullName}_{Field.Id}";

    /// <inheritdoc/>
    public int SelectedDirectoryField
    {
        get => Field.DirectoryId;
        private set
        {
            Field.DirectoryId = value;
            if (Field.Directory is not null)
                Field.Directory.Id = value;
            StateHasChangedHandler(Field);
        }
    }

    /// <inheritdoc/>
    public void Update(ConstructorFormDirectoryLinkModelDB field)
    {
        Field.Update(field);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<EntryModel[]> rest = await FormsRepo.GetDirectories();
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (rest.Response is null)
            throw new Exception($"Ошибка {{4BBC7550-5753-4D96-9E17-3E9B21F08493}} rest.Content.Entries is null");

        Entries = rest.Response;
        StateHasChanged();
    }
}