////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// NomenclatureCreatingFormComponent
/// </summary>
public partial class NomenclatureCreatingFormComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// GoodCreatingHandler
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<NomenclatureModelDB> GoodCreatingHandler { get; set; }

    /// <summary>
    /// ContextName
    /// </summary>
    [Parameter]
    public string? ContextName { get; set; }


    UnitsOfMeasurementEnum UMeas { get; set; } = UnitsOfMeasurementEnum.Thing;

    string? CreatingGoodsName { get; set; }

    bool CanSave => !string.IsNullOrWhiteSpace(CreatingGoodsName);

    async Task CreateNewGoods()
    {
        if (string.IsNullOrWhiteSpace(CreatingGoodsName))
            return;

        NomenclatureModelDB new_obj = new()
        {
            Name = CreatingGoodsName,
            BaseUnit = UMeas,
            ContextName = ContextName,
            IsDisabled = true,
        };

        await SetBusy();

        TResponseModel<int> res = await CommerceRepo.NomenclatureUpdateReceive(new_obj);
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