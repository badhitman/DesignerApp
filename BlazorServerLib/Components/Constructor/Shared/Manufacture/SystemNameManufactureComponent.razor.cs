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

    string itemSystemName = default!;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value!.Tag}_{Item.Value!.Id}";

    bool IsEdit => !Item.Value!.Name.Equals(itemSystemName);

    async Task SaveSystemName()
    {
        if (Item.Value?.Tag is null)
            throw new Exception();

        IsBusyProgress = true;
        ResponseBaseModel res = await ManufactureRepo.SetOrDeleteSystemName(new UpdateSystemNameModel() { TypeData = Item.Value.Tag, ManufactureId = ManufactureParentView.Manufacture.Id });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success())
            Item.Value.Name = itemSystemName;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        itemSystemName = Item.Value?.Name ?? throw new Exception();
    }
}