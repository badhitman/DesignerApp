////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Номенклатура
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute))]
#if DEBUG
[AllowAnonymous]
#else
[Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersReadCommerce)},{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
public class GoodsController(ICommerceRemoteTransmissionService commRepo) : ControllerBase
{
    /// <summary>
    /// Подбор номенклатуры (поиск по параметрам)
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrdersReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.GOODS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}"), LoggerNolog]
    public async Task<TResponseModel<TPaginationResponseModel<GoodsModelDB>>> OrdersSelect(TPaginationRequestModel<GoodsSelectRequestModel> req)
        => await commRepo.GoodsSelect(req);

    /// <summary>
    /// Чтение номенклатуры (по идентификаторам)
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrdersReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.GOODS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.READ_ACTION_NAME}"), LoggerNolog]
    public async Task<TResponseModel<GoodsModelDB[]>> GoodsRead(int[] req)
        => await commRepo.GoodsRead(req);
}