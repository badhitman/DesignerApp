﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.GridFS;
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
    IMongoDatabase mongoFs,
    IHelpdeskRemoteTransmissionService HelpdeskRepo,
    ICommerceRemoteTransmissionService commRepo,
    IDbContextFactory<StorageContext> cloudParametersDbFactory)
    : IResponseReceive<StorageImageMetadataModel?, StorageFileModelDB?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SaveFileReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageFileModelDB?>> ResponseHandleAction(StorageImageMetadataModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<StorageFileModelDB?> res = new();
        GridFSBucket gridFS = new(mongoFs);
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        string _file_name = rx.Replace(req.FileName.Trim(), " ");
        if (string.IsNullOrWhiteSpace(_file_name))
            _file_name = $"без имени: {DateTime.UtcNow}";

        using MemoryStream stream = new(req.Payload);
        ObjectId _uf = await gridFS.UploadFromStreamAsync(_file_name, stream);
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        res.Response = new StorageFileModelDB()
        {
            ApplicationName = req.ApplicationName,
            AuthorIdentityId = req.AuthorUserIdentity,
            FileName = _file_name,
            NormalizedFileNameUpper = _file_name.ToUpper(),
            ContentType = req.ContentType,
            PropertyName = req.PropertyName,
            PointId = _uf.ToString(),
            CreatedAt = DateTime.UtcNow,
            OwnerPrimaryKey = req.OwnerPrimaryKey,
            PrefixPropertyName = req.PrefixPropertyName,
            ReferrerMain = req.Referrer,
            FileLength = req.Payload.Length,
        };

        await context.AddAsync(res.Response);
        await context.SaveChangesAsync();

        if (GlobalTools.IsImageFile(_file_name))
        {
            using MagickImage image = new(req.Payload);
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

        if (req.ApplicationName == GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME && req.OwnerPrimaryKey.HasValue && req.OwnerPrimaryKey.Value > 0)
        {
            TResponseModel<OrderDocumentModelDB[]> get_order = await commRepo.OrdersRead([req.OwnerPrimaryKey.Value]);
            if (!get_order.Success() || get_order.Response is null)
                res.AddRangeMessages(get_order.Messages);
            else
            {
                OrderDocumentModelDB orderDb = get_order.Response.Single();
                if (orderDb.HelpdeskId.HasValue && orderDb.HelpdeskId.Value > 0)
                {
                    PulseRequestModel reqPulse = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                Description = $"Файл (внутренний) '{_file_name}' {GlobalTools.SizeDataAsString(req.Payload.Length)} [{nameof(res.Response.PointId)}:{_uf}] добавлен.",
                                IssueId = orderDb.HelpdeskId.Value,
                                PulseType = PulseIssuesTypesEnum.Files,
                                Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME
                            },
                            SenderActionUserId = GlobalStaticConstants.Roles.System,
                        }
                    };
                    await HelpdeskRepo.PulsePush(reqPulse);
                }
            }
        }

        return res;
    }
}