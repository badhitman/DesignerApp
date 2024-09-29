////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Заказы
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersReadCommerce)},{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
public class OrdersController(ICommerceRemoteTransmissionService commRepo, IMongoDatabase mongoFs) : ControllerBase
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
    /// Прикрепить файл к заказу (счёт, акт и т.п.)
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{{OrderId}}/{GlobalStaticConstants.Routes.ATTACHMENT_ACTION_NAME}-{GlobalStaticConstants.Routes.ADD_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
    public async Task<TResponseModel<int>> AttachmentForOrder([FromRoute] int OrderId, [FromBody] IFormFile uploadedFile)
    {
        TResponseModel<int> response = new();
        TResponseModel<OrderDocumentModelDB[]> call = await commRepo.OrdersRead([OrderId]);

        if (call.Success() || call.Response?.Length != 1)
        {
            response.AddRangeMessages(call.Messages);
            return response;
        }

        IGridFSBucket gridFS = new GridFSBucket(mongoFs);

        if (uploadedFile != null)
        {
            string _file_name = uploadedFile.FileName.Trim();
            if (string.IsNullOrWhiteSpace(_file_name))
                _file_name = $"без имени: {DateTime.UtcNow}";


            // путь к папке Files
            string path = Path.Combine(GlobalStaticConstants.Routes.FILES_CONTROLLER_NAME
                , GlobalStaticConstants.Routes.MONGO_CONTROLLER_NAME
                , GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME
                , OrderId.ToString(), _file_name);

            using Stream stream = uploadedFile.OpenReadStream();
            ObjectId file_id = await gridFS.UploadFromStreamAsync(path, stream);

        }

        return await commRepo.AttachmentForOrder(new AttachmentForOrderRequestModel() { FileName = "", FilePoint = "", FileSize = 0, OrderDocumentId = OrderId });
    }

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