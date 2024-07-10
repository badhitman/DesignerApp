////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.DirectoriesCatalog;

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
    protected IConstructorService ConstructorRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required FieldFormAkaDirectoryConstructorModelDB FieldObject { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public Action<FieldFormBaseLowConstructorModel, Type> StateHasChangedHandler { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }


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
    public void Update(FieldFormBaseLowConstructorModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseStrictModel<EntryModel[]> rest = await ConstructorRepo.GetDirectories(Form.ProjectId);
        IsBusyProgress = false;
        StateHasChangedHandler(FieldObject, GetType());

        Entries = rest.Response;
    }
}