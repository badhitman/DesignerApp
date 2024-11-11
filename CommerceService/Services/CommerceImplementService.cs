////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Json;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;

namespace CommerceService;

/// <summary>
/// Commerce
/// </summary>
public class CommerceImplementService(
    IDbContextFactory<CommerceContext> commerceDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService hdRepo,
    ITelegramRemoteTransmissionService tgRepo,
    IHttpClientFactory HttpClientFactory,
    ILogger<CommerceImplementService> loggerRepo,
    WebConfigModel _webConf,
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo) : ICommerceService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssuesGet(OrdersByIssuesSelectRequestModel req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (req.IssueIds.Length != 0)
            q = q.Where(x => req.IssueIds.Any(y => y == x.HelpdeskId));
        else
            return new()
            {
                Response = [],
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Запрос не может быть пустым" }]
            };

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, GoodsModelDB?> inc_query = q
            .Include(x => x.Organization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

        return new()
        {
            Response = req.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await q.ToArrayAsync()],
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] req)
    {
        TResponseModel<OrderDocumentModelDB[]> res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .Where(x => req.Any(y => x.Id == y));

        res.Response = await q
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods)
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusChange(StatusChangeRequestModel req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        TResponseModel<bool> res = new()
        {
            Response = await context
                    .OrdersDocuments
                    .Where(x => x.HelpdeskId == req.IssueId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.StatusDocument, req.Step)) != 0,
        };

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        TResponseModel<UserInfoModel[]?> actor = await webTransmissionRepo.GetUsersIdentity([req.AuthorIdentityUserId]);

        if (!actor.Success() || actor.Response is null || actor.Response.Length == 0)
        {
            res.AddRangeMessages(actor.Messages);
            return res;
        }
        string msg, waMsg;
        DateTime dtu = DateTime.UtcNow;
        req.LastAtUpdatedUTC = dtu;
        req.PrepareForSave();
        if (req.Id < 1)
        {
            req.CreatedAtUTC = dtu;
            TResponseModel<int?> res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);

            using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction();
            try
            {
                req.StatusDocument = StatusesDocumentsEnum.Created;
                await context.AddAsync(req);
                await context.SaveChangesAsync();
                res.Response = req.Id;

                TAuthRequestModel<IssueUpdateRequestModel> issue_new = new()
                {
                    SenderActionUserId = req.AuthorIdentityUserId,
                    Payload = new()
                    {
                        Name = req.Name,
                        RubricId = res_RubricIssueForCreateOrder.Response,
                        Description = $"Новый заказ.\n{req.Information}".Trim(),
                    },
                };

                TResponseModel<int> issue = await hdRepo.IssueCreateOrUpdate(issue_new);

                if (!issue.Success())
                {
                    await transaction.RollbackAsync();
                    res.Messages.AddRange(issue.Messages);
                    return res;
                }

                req.HelpdeskId = issue.Response;
                context.Update(req);
                await context.SaveChangesAsync();

                transaction.Commit();
                CultureInfo cultureInfo = new("ru-RU");

                if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
                {
                    TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                    _webConf.BaseUri = wc.Response?.ClearBaseUri;
                }

                string subject_email = "Создан новый заказ";
                TResponseModel<string?> CommerceNewOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderSubjectNotification);
                if (CommerceNewOrderSubjectNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderSubjectNotification.Response))
                    subject_email = CommerceNewOrderSubjectNotification.Response;

                DateTime _dt = DateTime.UtcNow.GetCustomTime();
                string _dtAsString = $"{_dt.ToString("d", cultureInfo)} {_dt.ToString("t", cultureInfo)}";

                string _about_order = $"'{req.Name}' {_dtAsString}";
                string ReplaceTags(string raw, bool clearMd = false)
                {
                    return raw.Replace(GlobalStaticConstants.OrderDocumentName, req.Name)
                    .Replace(GlobalStaticConstants.OrderDocumentDate, $"{_dtAsString}")
                    .Replace(GlobalStaticConstants.OrderStatusInfo, StatusesDocumentsEnum.Created.DescriptionInfo())
                    .Replace(GlobalStaticConstants.OrderLinkAddress, clearMd ? $"{_webConf.BaseUri}/issue-card/{req.HelpdeskId}" : $"<a href='{_webConf.BaseUri}/issue-card/{req.HelpdeskId}'>{_about_order}</a>")
                    .Replace(GlobalStaticConstants.HostAddress, clearMd ? _webConf.BaseUri : $"<a href='{_webConf.BaseUri}'>{_webConf.BaseUri}</a>");
                }

                subject_email = ReplaceTags(subject_email);

                res.AddSuccess(subject_email);
                msg = $"<p>Заказ <b>'{issue_new.Payload.Name}' от [{_dtAsString}]</b> успешно создан.</p>" +
                        $"<p>/<a href='{_webConf.ClearBaseUri}'>{_webConf.ClearBaseUri}</a>/</p>";
                string msg_for_tg = msg.Replace("<p>", "").Replace("</p>", "");

                waMsg = $"Заказ '{issue_new.Payload.Name}' от [{_dtAsString}] успешно создан.\n{_webConf.ClearBaseUri}";

                TResponseModel<string?> CommerceNewOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotification);
                if (CommerceNewOrderBodyNotification.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotification.Response))
                    msg = CommerceNewOrderBodyNotification.Response;
                msg = ReplaceTags(msg);

                TResponseModel<string?> CommerceNewOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationTelegram);
                if (CommerceNewOrderBodyNotificationTelegram.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationTelegram.Response))
                    msg_for_tg = CommerceNewOrderBodyNotificationTelegram.Response;
                msg_for_tg = ReplaceTags(msg_for_tg);

                List<Task> servicesCalls =
                [
                    webTransmissionRepo.SendEmail(new() { Email = actor.Response[0].Email!, Subject = subject_email, TextMessage = msg }),
                ];

                if (actor.Response[0].TelegramId.HasValue)
                    servicesCalls.Add(tgRepo.SendTextMessageTelegram(new() { Message = msg_for_tg, UserTelegramId = actor.Response[0].TelegramId!.Value }));

                if (!string.IsNullOrWhiteSpace(actor.Response[0].PhoneNumber) && GlobalTools.IsPhoneNumber(actor.Response[0].PhoneNumber!))
                {
                    TResponseModel<string?> wappiToken = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiTokenApi);
                    TResponseModel<string?> wappiProfileId = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiProfileId);

                    if (wappiToken.Success() && !string.IsNullOrWhiteSpace(wappiToken.Response) && wappiProfileId.Success() && !string.IsNullOrWhiteSpace(wappiProfileId.Response))
                    {
                        TResponseModel<string?> CommerceNewOrderBodyNotificationWhatsapp = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationWhatsapp);
                        if (CommerceNewOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationWhatsapp.Response))
                            waMsg = CommerceNewOrderBodyNotificationWhatsapp.Response;
                        waMsg = ReplaceTags(waMsg, true);

                        servicesCalls.Add(Task.Run(async () =>
                        {
                            using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Wappi.ToString());
                            if (!client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
                                client.DefaultRequestHeaders.Add("Authorization", wappiToken.Response);

                            using HttpResponseMessage response = await client.PostAsJsonAsync($"/api/sync/message/send?profile_id={wappiProfileId.Response}", new SendMessageRequestModel() { Body = waMsg, Recipient = actor.Response[0].PhoneNumber! });
                            string rj = await response.Content.ReadAsStringAsync();
                            SendMessageResponseModel sendWappiRes = JsonConvert.DeserializeObject<SendMessageResponseModel>(rj)!;
                        }));
                    }
                }

                loggerRepo.LogInformation(msg_for_tg);
                await Task.WhenAll(servicesCalls);
                return res;
            }
            catch (Exception ex)
            {
                loggerRepo.LogError(ex, $"Не удалось создать заявку-заказ: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
                res.Messages.InjectException(ex);
                return res;
            }
        }

        OrderDocumentModelDB? order_document = await context.OrdersDocuments.FirstOrDefaultAsync(x => x.Id == req.Id);
        if (order_document is null)
        {
            res.AddError($"Документ #{req.Id} не найден");
            return res;
        }

        if (order_document.Name == req.Name && order_document.IsDisabled == req.IsDisabled)
        {
            res.AddInfo($"Документ #{req.Id} не требует обновления");
            return res;
        }

        res.Response = await context.OrdersDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SenderActionUserId) && !req.Payload.SenderActionUserId.Equals(GlobalStaticConstants.Roles.System))
            q = q.Where(x => x.AuthorIdentityUserId == req.Payload.SenderActionUserId);

        if (req.Payload.Payload.OrganizationFilter.HasValue && req.Payload.Payload.OrganizationFilter.Value != 0)
            q = q.Where(x => x.OrganizationId == req.Payload.Payload.OrganizationFilter);

        if (req.Payload.Payload.AddressForOrganizationFilter.HasValue && req.Payload.Payload.AddressForOrganizationFilter.Value != 0)
            q = q.Where(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == req.Payload.Payload.AddressForOrganizationFilter));

        if (req.Payload.Payload.OfferFilter.HasValue && req.Payload.Payload.OfferFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.OfferId == req.Payload.Payload.OfferFilter));

        if (req.Payload.Payload.GoodsFilter.HasValue && req.Payload.Payload.GoodsFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.GoodsId == req.Payload.Payload.GoodsFilter));

        if (req.Payload.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.Payload.AfterDateUpdate));

        if (req.Payload.Payload.StatusesFilter is not null && req.Payload.Payload.StatusesFilter.Length != 0)
            q = q.Where(x => req.Payload.Payload.StatusesFilter.Any(y => y == x.StatusDocument));

        IOrderedQueryable<OrderDocumentModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.CreatedAtUTC)
           : q.OrderByDescending(x => x.CreatedAtUTC);

        IQueryable<OrderDocumentModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, GoodsModelDB?> inc_query = pq
            .Include(x => x.Organization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.Payload.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
            },
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
    {
        TResponseModel<bool> res = new() { Response = true };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int[] orders_ids = await context
            .OrdersDocuments
            .Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id))
            .Select(x => x.Id)
            .ToArrayAsync();

        if (orders_ids.Length == 0)
        {
            res.AddError($"Документы не найдены");
            return res;
        }

        DateTime dtu = DateTime.UtcNow;

        await context.OrdersDocuments
                .Where(x => orders_ids.Any(y => y == x.Id))
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.Response = await context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0;
        res.AddSuccess("Команда удаления выполнена");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;

        await context.OrdersDocuments
                .Where(x => x.Id == req.OrderDocumentId)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        if (req.Id < 1)
        {
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен к заказу");
            res.Response = req.Id;
            return res;
        }

        res.Response = await context.RowsOfOrdersDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Amount, req.Amount)
            .SetProperty(p => p.Quantity, req.Quantity));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForWarehouseDocumentUpdate(RowOfWarehouseDocumentModelDB req)
    {

        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;

        await context.WarehouseDocuments
                .Where(x => x.Id == req.WarehouseDocumentId)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        if (req.Id < 1)
        {
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен к документу");
            res.Response = req.Id;
            return res;
        }

        res.Response = await context.RowsOfWarehouseDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Quantity, req.Quantity));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForWarehouseDocumentDelete(int[] req)
    {
        TResponseModel<bool> res = new() { Response = true };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int[] documents_ids = await context
            .WarehouseDocuments
            .Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id))
            .Select(x => x.Id)
            .ToArrayAsync();

        if (documents_ids.Length == 0)
        {
            res.AddError($"Документы не найдены");
            return res;
        }

        DateTime dtu = DateTime.UtcNow;

        await context.WarehouseDocuments
                .Where(x => documents_ids.Any(y => y == x.Id))
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.Response = await context.RowsOfWarehouseDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0;
        res.AddSuccess("Команда удаления выполнена");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>> WarehouseDocumentsSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WarehouseDocumentModelDB> q = context
            .WarehouseDocuments
            .AsQueryable();

        if (req.Payload.OfferFilter.HasValue && req.Payload.OfferFilter.Value != 0)
            q = q.Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id && y.OfferId == req.Payload.OfferFilter));

        if (req.Payload.GoodsFilter.HasValue && req.Payload.GoodsFilter.Value != 0)
            q = q.Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id && y.GoodsId == req.Payload.GoodsFilter));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.AfterDateUpdate));

        IOrderedQueryable<WarehouseDocumentModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.CreatedAtUTC)
           : q.OrderByDescending(x => x.CreatedAtUTC);

        IQueryable<WarehouseDocumentModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WarehouseDocumentModelDB, GoodsModelDB?> inc_query = pq
            .Include(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
            },
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WarehouseDocumentUpdate(WarehouseDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 1)
        {
            req.CreatedAtUTC = dtu;
            req.LastAtUpdatedUTC = dtu;
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.Response = req.Id;
        }
        else
        {
            res.Response = await context.WarehouseDocuments
                .Where(x => x.Id == req.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.Name, req.Name)
                .SetProperty(p => p.Description, req.Description)
                .SetProperty(p => p.DeliveryData, req.DeliveryData)
                .SetProperty(p => p.IsDisabled, req.IsDisabled)
                .SetProperty(p => p.LastAtUpdatedUTC, dtu));
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehouseDocumentsRead(int[] req)
    {
        TResponseModel<WarehouseDocumentModelDB[]> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WarehouseDocumentModelDB> q = context
            .WarehouseDocuments
            .Where(x => req.Any(y => x.Id == y));

        res.Response = await q
            .Include(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods)
            .ToArrayAsync();

        return res;
    }
}