////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Save file
/// </summary>
public class SaveFileReceive(IMongoDatabase mongoFs, IDbContextFactory<StorageContext> cloudParametersDbFactory)
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

        string _file_name = req.FileName;
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
            Name = req.Name,
            PointId = _uf.ToString(),
            CreatedAt = DateTime.UtcNow,
            OwnerPrimaryKey = req.OwnerPrimaryKey,
            PrefixPropertyName = req.PrefixPropertyName,
            ReferrerMain = req.Referrer,
            FileLength = req.Payload.Length,
        };
        await context.AddAsync(res.Response);
        await context.SaveChangesAsync();

        return res;
    }
}