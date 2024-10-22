////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// GoodCreatingFormComponent
/// </summary>
public partial class GoodCreatingFormComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// GoodCreatingHandler
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<GoodsModelDB> GoodCreatingHandler { get; set; }


    UnitsOfMeasurementEnum UMeas { get; set; } = UnitsOfMeasurementEnum.Thing;

    string? CreatingGoodsName { get; set; }

    bool CanSave => !string.IsNullOrWhiteSpace(CreatingGoodsName);

    async Task CreateNewGoods()
    {
        if (string.IsNullOrWhiteSpace(CreatingGoodsName))
            return;

        GoodsModelDB new_obj = new()
        {
            Name = CreatingGoodsName,
            BaseUnit = UMeas,
        };

        SetBusy();
        
        TResponseModel<int> res = await CommerceRepo.GoodUpdateReceive(new_obj);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success() && res.Response > 0)
        {
            new_obj.Id = res.Response;
            GoodCreatingHandler(new_obj);

            CreatingGoodsName = null;
            UMeas = UnitsOfMeasurementEnum.Thing;
        }
    }
}