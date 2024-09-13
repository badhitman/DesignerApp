////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Доставка
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersReadCommerce)},{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
public class OrdersController(ICommerceRemoteTransmissionService commRepo) : ControllerBase
{
    /// <summary>
    /// Подбор (поиск по параметрам) заказов
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrdersReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}"), LoggerNolog]
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
        => await commRepo.OrdersSelect(req);


    /// <summary>
    /// Обновить (или: создать, удалить) доставку
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DELIVERY_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
    public async Task<TResponseModel<int>> DeliveryOrderUpdate(DeliveryForOrderUpdateRequestModel delivery)
        => await commRepo.DeliveryOrderUpdate(delivery);

    /// <summary>
    /// Прикрепить файл к заказу (счёт, акт и т.п.)
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ATTACHMENT_ACTION_NAME}-{GlobalStaticConstants.Routes.ADD_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
    public async Task<TResponseModel<int>> AttachmentForOrder(AttachmentForOrderRequestModel att)
        => await commRepo.AttachmentForOrder(att);

    /// <summary>
    /// Обновить (или создать) строку документа
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ROW_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await commRepo.RowForOrderUpdate(row);
}