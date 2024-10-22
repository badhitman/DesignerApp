////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


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

    /// <summary>
    /// Мультвыбор
    /// </summary>
    public bool IsMultiCheckBox
    {
        get => FieldObject.IsMultiSelect;
        set
        {
            FieldObject.IsMultiSelect = value;
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
        await SetBusy();
        
        TResponseModel<EntryModel[]> rest = await ConstructorRepo.GetDirectories(new() { ProjectId = Form.ProjectId });
        IsBusyProgress = false;
        StateHasChangedHandler(FieldObject, GetType());

        if (rest.Response is null)
            throw new Exception();
        Entries = rest.Response;
    }
}