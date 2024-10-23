////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.storage;

/// <summary>
/// FilesAreaGetMetadataReceive
/// </summary>
public class FilesAreaGetMetadataReceive(ILogger<FilesSelectReceive> loggerRepo, IDbContextFactory<StorageContext> cloudParametersDbFactory)
    : IResponseReceive<FilesAreaMetadataRequestModel?, FilesAreaMetadataModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FilesAreaGetMetadataReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FilesAreaMetadataModel[]?>> ResponseHandleAction(FilesAreaMetadataRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();

        IQueryable<StorageFileModelDB> q = context
            .CloudFiles
            .AsQueryable();

        if (req.ApplicationsNamesFilter is not null && req.ApplicationsNamesFilter.Length != 0)
            q = q.Where(x => req.ApplicationsNamesFilter.Contains(x.ApplicationName));

        var res = await q
            .GroupBy(x => x.ApplicationName)
            .Select(x => new
            {
                AppName = x.Key,
                CountFiles = x.Count(),
                SummSize = x.Sum(y => y.FileLength)
            })
            .ToArrayAsync();

        return new()
        {
            Response = [.. res.Select(x => new FilesAreaMetadataModel() { ApplicationName = x.AppName, CountFiles = x.CountFiles, SizeFilesSum = x.SummSize })]
        };
    }
}