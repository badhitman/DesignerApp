////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib;

/// <summary>
/// BlazorRegistersComponent
/// </summary>
public abstract class BlazorRegistersComponent : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceTransmission CommerceRepo { get; set; } = default!;


    /// <summary>
    /// RegistersCache
    /// </summary>
    public List<OfferAvailabilityModelDB> RegistersCache = [];

    /// <summary>
    /// CacheRegistersUpdate
    /// </summary>
    protected async Task CacheRegistersUpdate(int[] offers, int[] goods, int warehouseId = 0, bool clearCache = false)
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

        if (goods.Length == 0 && offers.Length == 0)
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
        TPaginationResponseModel<OfferAvailabilityModelDB> offersRegisters = await CommerceRepo.OffersRegistersSelect(reqData);
        await SetBusy(false);

        if (offersRegisters.Response is not null && offersRegisters.Response.Count != 0)
        {
            lock (this)
            {
                RegistersCache.AddRange(offersRegisters.Response.Where(x => !RegistersCache.Any(y => y.Id == x.Id)));
            }
        }
    }
}