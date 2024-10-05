////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Получить файл Commerce
/// </summary>
public class GetFileOrderReceive(IMongoDatabase mongoFs)
    : IResponseReceive<string?, byte[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ReadFileCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<byte[]?>> ResponseHandleAction(string? fileId)
    {
        ArgumentNullException.ThrowIfNull(fileId);
        TResponseModel<byte[]?> res = new();

        try
        {
            MemoryStream ms = new();
            IGridFSBucket gridFS = new GridFSBucket(mongoFs);
            await gridFS.DownloadToStreamAsync(new ObjectId(fileId), ms);
            res.Response = ms.ToArray();
        }
        catch (Exception ex)
        {
            res.AddError(ex.Message);
        }
        return res;
    }
}