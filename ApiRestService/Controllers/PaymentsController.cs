////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Платежи
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), LoggerNolog, Authorize(Roles = $"{nameof(ExpressApiRolesEnum.PaymentsReadCommerce)}")]
public class PaymentsController(ICommerceRemoteTransmissionService commRepo) : ControllerBase
{
    /// <summary>
    /// Обновить/создать платёжный документ
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.PaymentsWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.PAYMENTS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}"), LoggerLog, Authorize(Roles = $"{nameof(ExpressApiRolesEnum.PaymentsWriteCommerce)}")]
    public async Task<TResponseModel<int>> PaymentDocumentUpdate(PaymentDocumentBaseModel payment)
        => await commRepo.PaymentDocumentUpdate(payment);

    /// <summary>
    /// Удалить платёжный документ
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.PaymentsWriteCommerce"/>
    /// </remarks>
    [HttpDelete($"/api/{GlobalStaticConstants.Routes.PAYMENTS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DELETE_ACTION_NAME}/{{payment_id}}"), LoggerLog, Authorize(Roles = $"{nameof(ExpressApiRolesEnum.PaymentsWriteCommerce)}")]
    public async Task<TResponseModel<bool>> PaymentDocumentDelete([FromRoute] int payment_id)
        => await commRepo.PaymentDocumentDelete(payment_id);
}