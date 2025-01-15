////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Получить сводку (метаданные) по пространствам хранилища
/// </summary>
/// <remarks>
/// Общий размер и количество группируется по AppName
/// </remarks>
public class FilesAreaGetMetadataReceive(ILogger<FilesSelectReceive> loggerRepo, ISerializeStorage serializeStorageRepo)
    : IResponseReceive<FilesAreaMetadataRequestModel?, TResponseModel<FilesAreaMetadataModel[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FilesAreaGetMetadataReceive;

    /// <summary>
    /// Получить сводку (метаданные) по пространствам хранилища
    /// </summary>
    /// <remarks>
    /// Общий размер и количество группируется по AppName
    /// </remarks>
    public async Task<TResponseModel<FilesAreaMetadataModel[]>?> ResponseHandleAction(FilesAreaMetadataRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        return await serializeStorageRepo.FilesAreaGetMetadata(req);
    }
}