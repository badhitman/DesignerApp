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
/// Read file
/// </summary>
public class ReadFileReceive(IMongoDatabase mongoFs, IDbContextFactory<StorageContext> cloudParametersDbFactory)
    : IResponseReceive<int?, StorageFileResponseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadFileReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageFileResponseModel?>> ResponseHandleAction(int? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<StorageFileResponseModel?> res = new();
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        StorageFileModelDB? file_db = await context.CloudFiles.FirstOrDefaultAsync(x => x.Id == req);
        if (file_db is null)
        {
            res.AddError($"Файл #{req} не найден в БД");
            return res;
        }

        using MemoryStream stream = new();
        GridFSBucket gridFS = new(mongoFs);
        await gridFS.DownloadToStreamAsync(new ObjectId(file_db.PointId), stream);

        res.Response = new()
        {
            ApplicationName = file_db.ApplicationName,
            AuthorIdentityId = file_db.AuthorIdentityId,
            FileName = file_db.FileName,
            PropertyName = file_db.PropertyName,
            CreatedAt = file_db.CreatedAt,
            OwnerPrimaryKey = file_db.OwnerPrimaryKey,
            PointId = file_db.PointId,
            PrefixPropertyName = file_db.PrefixPropertyName,
            Payload = stream.ToArray(),
            Id = file_db.Id,
            ContentType = file_db.ContentType,
        };

        return res;
    }
}