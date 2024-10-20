////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Заказы
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute))]
#if DEBUG
[AllowAnonymous]
#else
[Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersReadCommerce)},{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
public class OrdersController(ICommerceRemoteTransmissionService commRepo, IHelpdeskRemoteTransmissionService hdRepo, IMongoDatabase mongoFs) : ControllerBase
{
    /// <summary>
    /// Подбор (поиск по параметрам) заказов
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrdersReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}")]
#if !DEBUG
    [LoggerNolog]
#endif
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
        => await commRepo.OrdersSelect(req);

    /// <summary>
    /// Прикрепить файл к заказу (счёт, акт и т.п.). Имя файла должно быть уникальным в контексте заказа. Если файл с таким именем существует, тогда он будет перезаписан новым
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME}/{{OrderId}}/{GlobalStaticConstants.Routes.ATTACHMENT_ACTION_NAME}-{GlobalStaticConstants.Routes.ADD_ACTION_NAME}")]
#if DEBUG
    [AllowAnonymous]
#else
    [Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
    public async Task<ResponseBaseModel> AttachmentForOrder([FromRoute] int OrderId, IFormFile uploadedFile)
    {
        TResponseModel<int> response = new();
        TResponseModel<OrderDocumentModelDB[]> call = await commRepo.OrdersRead([OrderId]);

        if (!call.Success())
        {
            response.AddRangeMessages(call.Messages);
            return response;
        }
        else if (call.Response?.Length != 1)
        {
#if !DEBUG
            response.AddError($"Заказ #{OrderId} не найден или у вас не достаточно прав для выполнения команды");
            return response;
#endif
        }

        GridFSBucket gridFS = new(mongoFs);

        if (uploadedFile != null)
        {
            string _file_name = uploadedFile.FileName.Trim();
            if (string.IsNullOrWhiteSpace(_file_name))
                _file_name = $"без имени: {DateTime.UtcNow}";

            // путь к папке Files
            //string path = Path.Combine(GlobalStaticConstants.Routes.FILES_CONTROLLER_NAME, GlobalStaticConstants.Routes.MONGO_CONTROLLER_NAME, GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME, OrderId.ToString(), _file_name);

            using Stream stream = uploadedFile.OpenReadStream();
            ObjectId _uf = await gridFS.UploadFromStreamAsync(_file_name, stream);
            AttachmentForOrderRequestModel att_file = new()
            {
                FileName = _file_name,
                FilePoint = _uf.ToString(),
                FileSize = uploadedFile.Length,
                OrderDocumentId = OrderId
            };
            return await commRepo.AttachmentForOrder(att_file);
        }

        return ResponseBaseModel.CreateWarning("Данные не записаны");
    }

    /// <summary>
    /// Обновить (или создать) строку документа
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ROW_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}")]
#if DEBUG
    [AllowAnonymous]
#else
    [Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB row)
        => await commRepo.RowForOrderUpdate(row);

    /// <summary>
    /// Удалить строки из заказа
    /// </summary>
    /// <param name="rows_ids">Идентификаторы строк, которые следует удалить</param>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
#if DEBUG
    [AllowAnonymous]
#else
    [Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
    [HttpDelete($"/api/{GlobalStaticConstants.Routes.ORDERS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ROW_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.DELETE_ACTION_NAME}")]
    public async Task<TResponseModel<bool>> RowForOrderDelete([FromBody] int[] rows_ids)
        => await commRepo.RowsForOrderDelete(rows_ids);

    /// <summary>
    /// Установить статус заказа
    /// </summary>
    /// <param name="OrderId">Идентификатор заказа</param>
    /// <param name="Step">Статус заказа, который нужно установить</param>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME}/{{OrderId}}/{GlobalStaticConstants.Routes.STAGE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}/{{Step}}")]
#if DEBUG
    [AllowAnonymous]
#else
    [Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
    public async Task<TResponseModel<bool>> OrderStageSet([FromRoute] int OrderId, [FromRoute] HelpdeskIssueStepsEnum Step)
    {
        TResponseModel<OrderDocumentModelDB[]> call = await commRepo.OrdersRead([OrderId]);
        TResponseModel<bool> response = new() { Response = false };
        if (!call.Success())
        {
            response.AddRangeMessages(call.Messages);
            return response;
        }
        else if (call.Response?.Length != 1)
        {
            response.AddError($"Заказ #{OrderId} не найден или у вас не достаточно прав для выполнения команды");
            return response;
        }

        OrderDocumentModelDB order_doc = call.Response.Single();

        if (order_doc.HelpdeskId.HasValue != true)
        {
            response.AddError($"Заказ #{OrderId} не найден или у вас не достаточно прав для выполнения команды");
            return response;
        }

        TAuthRequestModel<IssuesReadRequestModel> req_hd = new()
        {
            SenderActionUserId = GlobalStaticConstants.Roles.System,
            Payload = new()
            {
                IssuesIds = [order_doc.HelpdeskId.Value]
            }
        };
        TResponseModel<IssueHelpdeskModelDB[]> find_helpdesk = await hdRepo.IssuesRead(req_hd);
        if (!find_helpdesk.Success() || find_helpdesk.Response is null || find_helpdesk.Response.Length != 1)
        {
            response.AddRangeMessages(find_helpdesk.Messages);
            return response;
        }
        IssueHelpdeskModelDB hd_obj = find_helpdesk.Response.Single();
        if (hd_obj.StepIssue == Step)
        {
            response.AddInfo("Статус уже установлен!");
            return response;
        }
        TAuthRequestModel<StatusChangeRequestModel> status_change_req = new()
        {
            SenderActionUserId = GlobalStaticConstants.Roles.System,
            Payload = new()
            {
                IssueId = hd_obj.Id,
                Step = Step,
            }
        };
        TResponseModel<bool> update_final = await hdRepo.StatusChange(status_change_req);
        response.AddRangeMessages(update_final.Messages);
        return response;
    }
}