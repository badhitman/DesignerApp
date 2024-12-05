////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib;

/// <summary>
/// BlazorBusyComponentRegistersModel
/// </summary>
public abstract class BlazorBusyComponentRegistersModel : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceRemoteTransmissionService CommerceRepo { get; set; } = default!;


    /// <summary>
    /// RegistersCache
    /// </summary>
    protected List<OfferAvailabilityModelDB> RegistersCache = [];

    /// <summary>
    /// CacheRegistersOfferUpdate
    /// </summary>
    protected async Task CacheRegistersOfferUpdate(IEnumerable<int> req, int warehouseId = 0, bool ClearCache = false)
    {
        if (ClearCache)
        {
            lock (this)
            {
                RegistersCache.Clear();
            }
        }

        req = [.. req.Where(x => x > 0 && !RegistersCache.Any(y => y.Id == x)).Distinct()];

        TPaginationRequestModel<RegistersSelectRequestBaseModel> reqData = new()
        {
            Payload = new()
            {
                OfferFilter = [.. req],
                WarehouseId = warehouseId,
                MinQuantity = 1,
            },
            PageNum = 0,
            PageSize = int.MaxValue,
            SortingDirection = VerticalDirectionsEnum.Up,
        };
        await SetBusy();
        TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>> rubrics = await CommerceRepo.OffersRegistersSelect(reqData);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(rubrics.Messages);
        if (rubrics.Success() && rubrics.Response is not null && rubrics.Response.Response.Count != 0)
        {
            lock (this)
            {
                RegistersCache.AddRange(rubrics.Response.Response.Where(x => !RegistersCache.Any(y => y.Id == x.Id)));
            }
        }
    }

    /// <summary>
    /// CacheRegistersGoodsUpdate
    /// </summary>
    protected async Task CacheRegistersGoodsUpdate(IEnumerable<int> req, int warehouseId = 0, bool ClearCache = false)
    {
        if (ClearCache)
            RegistersCache.Clear();

        req = [.. req.Where(x => x > 0 && !RegistersCache.Any(y => y.Id == x)).Distinct()];
        if (!req.Any())
            return;

        TPaginationRequestModel<RegistersSelectRequestBaseModel> reqData = new()
        {
            Payload = new()
            {
                NomenclatureFilter = [.. req],
                WarehouseId = warehouseId,
            },
            PageNum = 0,
            PageSize = int.MaxValue,
            SortingDirection = VerticalDirectionsEnum.Up,
        };
        await SetBusy();
        TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>> rubrics = await CommerceRepo.OffersRegistersSelect(reqData);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(rubrics.Messages);
        if (rubrics.Success() && rubrics.Response is not null && rubrics.Response.Response.Count != 0)
            RegistersCache.AddRange(rubrics.Response.Response);
    }
}