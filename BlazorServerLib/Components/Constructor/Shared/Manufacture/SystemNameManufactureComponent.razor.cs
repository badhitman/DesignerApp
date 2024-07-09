using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Manufacture;

/// <summary>
/// SystemNameManufactureComponent
/// </summary>
public partial class SystemNameManufactureComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IManufactureService ManufactureRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required TreeItemData<EntryTagModel> Item { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ManufactureComponent ManufactureParentView { get; set; }

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }


    TreeItemDataModel ItemModel = default!;

    string itemSystemName = default!;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value!.Tag}_{Item.Value!.Id}";

    bool IsEdit => !itemSystemName.Equals(ParentFormsPage.SystemNamesManufacture.FirstOrDefault(x => x.TypeDataId == Item.Value!.Id && x.TypeDataName.Equals(Item.Value.Tag))?.SystemName ?? "");

    /// <inheritdoc/>
    protected MarkupString InformationMS => (MarkupString)(ItemModel.Information ?? "");

    async Task SaveSystemName()
    {
        if (Item.Value?.Tag is null)
            throw new Exception();

        IsBusyProgress = true;
        ResponseBaseModel res = await ManufactureRepo
            .SetOrDeleteSystemName(new UpdateSystemNameModel() { SystemName = itemSystemName, Qualification = ItemModel.Qualification, TypeDataId = Item.Value.Id, TypeDataName = Item.Value.Tag, ManufactureId = ManufactureParentView.Manufacture.Id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success())
        {
            await ParentFormsPage.GetSystemNames();
            ParentFormsPage.StateHasChangedCall();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = (TreeItemDataModel)Item;
        SystemNameEntryModel? _sn = ParentFormsPage
            .SystemNamesManufacture
            .FirstOrDefault(x => x.Qualification == ItemModel.Qualification && x.TypeDataId == Item.Value!.Id && x.TypeDataName == Item.Value.Tag);

        itemSystemName = _sn?.SystemName ?? "";
    }
}