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
    /// CacheRegistersUpdate
    /// </summary>
    protected async Task CacheRegistersUpdate(IEnumerable<int> offers, IEnumerable<int> goods, int warehouseId = 0, bool clearCache = false)
    {
        if (clearCache)
        {
            lock (this)
            {
                RegistersCache.Clear();
            }
        }

        offers = [.. offers.Where(x => x > 0 && !RegistersCache.Any(y => y.Id == x)).Distinct()];
        goods = [.. goods.Where(x => x > 0 && !RegistersCache.Any(y => y.Id == x)).Distinct()];

        if (!goods.Any() || !offers.Any())
            return;

        TPaginationRequestModel<RegistersSelectRequestBaseModel> reqData = new()
        {
            Payload = new()
            {
                OfferFilter = [.. offers],
                NomenclatureFilter = [.. goods],
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
}