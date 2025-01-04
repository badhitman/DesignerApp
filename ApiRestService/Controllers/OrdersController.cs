////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;
using Microsoft.AspNetCore.Http.Extensions;

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
public class OrdersController(ICommerceRemoteTransmissionService commRepo, IHelpdeskRemoteTransmissionService hdRepo, ISerializeStorageRemoteTransmissionService storageRepo) : ControllerBase
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
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<OrdersSelectRequestModel> req)
        => await commRepo.OrdersSelect(new TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>>() { Payload = new TAuthRequestModel<OrdersSelectRequestModel>() { Payload = req.Payload, SenderActionUserId = GlobalStaticConstants.Roles.System } });

    /// <summary>
    /// Прикрепить файл к заказу (счёт, акт и т.п.). Имя файла должно быть уникальным в контексте заказа. Если файл с таким именем существует, тогда он будет перезаписан новым
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrdersWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME}/{{OrderId}}/{GlobalStaticConstants.Routes.ATTACHMENT_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.ADD_ACTION_NAME}")]
#if DEBUG
    [AllowAnonymous]
#else
    [Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrdersWriteCommerce)}")]
#endif
    public async Task<TResponseModel<StorageFileModelDB>> AttachmentForOrder([FromRoute] int OrderId, IFormFile uploadedFile)
    {
        TResponseModel<StorageFileModelDB> response = new();
        if (uploadedFile is null || uploadedFile.Length == 0)
        {
            response.AddError("Данные файла отсутствуют");
            return response;
        }

        TResponseModel<OrderDocumentModelDB[]> call = await commRepo.OrdersRead([OrderId]);

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

        string _file_name = uploadedFile.FileName.Trim();
        if (string.IsNullOrWhiteSpace(_file_name))
            _file_name = $"без имени: {DateTime.UtcNow}";

        using MemoryStream stream = new();
        uploadedFile.OpenReadStream().CopyTo(stream);
        StorageImageMetadataModel reqSave = new()
        {
            ApplicationName = GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME,
            PropertyName = GlobalStaticConstants.Routes.ATTACHMENT_CONTROLLER_NAME,
            PrefixPropertyName = GlobalStaticConstants.Routes.REST_CONTROLLER_NAME,
            AuthorUserIdentity = GlobalStaticConstants.Roles.System,
            FileName = _file_name,
            ContentType = uploadedFile.ContentType,
            OwnerPrimaryKey = OrderId,
            Referrer = Request.GetEncodedPathAndQuery(),
            Payload = stream.ToArray(),
        };

        return await storageRepo.SaveFile(reqSave);
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
    public async Task<TResponseModel<bool>> OrderStageSet([FromRoute] int OrderId, [FromRoute] StatusesDocumentsEnum Step)
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
        if (hd_obj.StatusDocument == Step)
        {
            response.AddInfo("Статус уже установлен!");
            return response;
        }
        TAuthRequestModel<StatusChangeRequestModel> status_change_req = new()
        {
            SenderActionUserId = GlobalStaticConstants.Roles.System,
            Payload = new()
            {
                DocumentId = hd_obj.Id,
                Step = Step,
            }
        };
        TResponseModel<bool> update_final = await hdRepo.StatusChange(status_change_req);
        response.AddRangeMessages(update_final.Messages);
        return response;
    }
}