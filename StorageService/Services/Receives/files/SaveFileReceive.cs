﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using MongoDB.Driver;
using RemoteCallLib;
using MongoDB.Bson;
using ImageMagick;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Save file
/// </summary>
public class SaveFileReceive(
    ILogger<SaveFileReceive> LoggerRepo,
    IMongoDatabase mongoFs,
    IHelpdeskRemoteTransmissionService HelpdeskRepo,
    ICommerceRemoteTransmissionService commRepo,
    IOptions<WebConfigModel> webConfig,
    IDbContextFactory<StorageContext> cloudParametersDbFactory) : IResponseReceive<TAuthRequestModel<StorageImageMetadataModel>?, TResponseModel<StorageFileModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveFileReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageFileModelDB>?> ResponseHandleAction(TAuthRequestModel<StorageImageMetadataModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<StorageFileModelDB> res = new();
        GridFSBucket gridFS = new(mongoFs);
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        string _file_name = rx.Replace(req.Payload.FileName.Trim(), " ");
        if (string.IsNullOrWhiteSpace(_file_name))
            _file_name = $"без имени: {DateTime.UtcNow}";

        using MemoryStream stream = new(req.Payload.Payload);
        ObjectId _uf = await gridFS.UploadFromStreamAsync(_file_name, stream);
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        res.Response = new StorageFileModelDB()
        {
            ApplicationName = req.Payload.ApplicationName,
            AuthorIdentityId = req.Payload.AuthorUserIdentity,
            FileName = _file_name,
            NormalizedFileNameUpper = _file_name.ToUpper(),
            ContentType = req.Payload.ContentType,
            PropertyName = req.Payload.PropertyName,
            PointId = _uf.ToString(),
            CreatedAt = DateTime.UtcNow,
            OwnerPrimaryKey = req.Payload.OwnerPrimaryKey,
            PrefixPropertyName = req.Payload.PrefixPropertyName,
            ReferrerMain = req.Payload.Referrer,
            FileLength = req.Payload.Payload.Length,
        };

        await context.AddAsync(res.Response);
        await context.SaveChangesAsync();

        if (GlobalTools.IsImageFile(_file_name))
        {
            using MagickImage image = new(req.Payload.Payload);
            //
            string _h = $"Height:{image.Height}", _w = $"Width:{image.Width}";
            await context.AddAsync(new TagModelDB()
            {
                ApplicationName = GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME,
                PropertyName = GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME,
                CreatedAt = DateTime.UtcNow,
                NormalizedTagNameUpper = _h.ToUpper(),
                TagName = _h,
                OwnerPrimaryKey = res.Response.Id,
                PrefixPropertyName = GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME,
            });
            await context.AddAsync(new TagModelDB()
            {
                ApplicationName = GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME,
                PropertyName = GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME,
                CreatedAt = DateTime.UtcNow,
                NormalizedTagNameUpper = _w.ToUpper(),
                TagName = _w,
                OwnerPrimaryKey = res.Response.Id,
                PrefixPropertyName = GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME,
            });
            await context.AddAsync(new TagModelDB()
            {
                ApplicationName = GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME,
                PropertyName = GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME,
                CreatedAt = DateTime.UtcNow,
                NormalizedTagNameUpper = nameof(GlobalTools.IsImageFile).ToUpper(),
                TagName = nameof(GlobalTools.IsImageFile),
                OwnerPrimaryKey = res.Response.Id,
                PrefixPropertyName = GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME,
            });
        }

        if (req.Payload.OwnerPrimaryKey.HasValue && req.Payload.OwnerPrimaryKey.Value > 0)
        {
            PulseRequestModel reqPulse;
            string msg;
            switch (req.Payload.ApplicationName)
            {
                case GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME:
                    TResponseModel<OrderDocumentModelDB[]> get_order = await commRepo.OrdersRead(new() { Payload = [req.Payload.OwnerPrimaryKey.Value], SenderActionUserId = req.SenderActionUserId });
                    if (!get_order.Success() || get_order.Response is null)
                        res.AddRangeMessages(get_order.Messages);
                    else
                    {
                        OrderDocumentModelDB orderDb = get_order.Response.Single();
                        if (orderDb.HelpdeskId.HasValue && orderDb.HelpdeskId.Value > 0)
                        {
                            msg = $"В <a href=\"{webConfig.Value.ClearBaseUri}/issue-card/{orderDb.HelpdeskId.Value}\">заказ #{orderDb.Id}</a> добавлен файл '<u>{_file_name}</u>' {GlobalTools.SizeDataAsString(req.Payload.Payload.Length)}";
                            LoggerRepo.LogInformation($"{msg} [{nameof(res.Response.PointId)}:{_uf}]");
                            reqPulse = new()
                            {
                                Payload = new()
                                {
                                    Payload = new()
                                    {
                                        Description = msg,
                                        IssueId = orderDb.HelpdeskId.Value,
                                        PulseType = PulseIssuesTypesEnum.Files,
                                        Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME
                                    },
                                    SenderActionUserId = GlobalStaticConstants.Roles.System,
                                }
                            };

                            await HelpdeskRepo.PulsePush(reqPulse, false);
                        }
                    }
                    break;
                case GlobalStaticConstants.Routes.ISSUE_CONTROLLER_NAME:
                    msg = $"В <a href=\"{webConfig.Value.ClearBaseUri}/issue-card/{req.Payload.OwnerPrimaryKey.Value}\">заявку #{req.Payload.OwnerPrimaryKey.Value}</a> добавлен файл '<u>{_file_name}</u>' {GlobalTools.SizeDataAsString(req.Payload.Payload.Length)}";
                    LoggerRepo.LogInformation($"{msg} [{nameof(res.Response.PointId)}:{_uf}]");
                    reqPulse = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                Description = msg,
                                IssueId = req.Payload.OwnerPrimaryKey.Value,
                                PulseType = PulseIssuesTypesEnum.Files,
                                Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME
                            },
                            SenderActionUserId = GlobalStaticConstants.Roles.System,
                        }
                    };
                    await HelpdeskRepo.PulsePush(reqPulse, false);
                    break;
            }
        }

        return res;
    }
}