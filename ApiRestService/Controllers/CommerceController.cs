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
[TypeFilter(typeof(RolesAuthorizationFilter), Arguments = [$"{nameof(ExpressApiRolesEnum.OrdersReadCommerce)},{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}"])]
public class CommerceController(ICommerceTransmission commRepo) : ControllerBase
{
    /// <summary>
    /// Подбор номенклатуры (поиск по параметрам)
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrdersReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.COMMERCE_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}")]
#if !DEBUG
    [LoggerNolog]
#endif
    public async Task<TPaginationResponseModel<NomenclatureModelDB>> OrdersSelect(TPaginationRequestModel<NomenclaturesSelectRequestModel> req)
        => await commRepo.NomenclaturesSelect(req);

    /// <summary>
    /// Чтение номенклатуры (по идентификаторам)
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrdersReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.COMMERCE_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.NOMENCLATURES_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.READ_ACTION_NAME}")]
#if !DEBUG
    [LoggerNolog]
#endif
    public async Task<TResponseModel<List<NomenclatureModelDB>>> NomenclaturesRead(int[] req)
        => await commRepo.NomenclaturesRead(new() { Payload = req, SenderActionUserId = GlobalStaticConstants.Roles.System });
}