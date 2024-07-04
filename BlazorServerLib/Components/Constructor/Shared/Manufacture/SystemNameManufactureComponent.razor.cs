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

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required SystemNameEntryModel[] SystemNamesManufacture { get; set; }


    string itemSystemName = default!;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value!.Tag}_{Item.Value!.Id}";

    bool IsEdit => !itemSystemName.Equals(SystemNamesManufacture.FirstOrDefault(x => x.TypeDataId == Item.Value!.Id && x.TypeDataName.Equals(Item.Value.Tag))?.SystemName ?? "");

    async Task SaveSystemName()
    {
        if (Item.Value?.Tag is null)
            throw new Exception();

        IsBusyProgress = true;
        ResponseBaseModel res = await ManufactureRepo.SetOrDeleteSystemName(new UpdateSystemNameModel() { TypeDataId = Item.Value.Id, SystemName = itemSystemName, TypeDataName = Item.Value.Tag, ManufactureId = ManufactureParentView.Manufacture.Id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success())
        {
            await ManufactureParentView.GetSystemNames();
            ManufactureParentView.StateHasChangedCall();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        itemSystemName = SystemNamesManufacture.FirstOrDefault(x => x.TypeDataId == Item.Value!.Id && x.TypeDataName == Item.Value.Tag)?.SystemName ?? "";
    }
}