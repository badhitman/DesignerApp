////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.storage;

/// <summary>
/// Find parameters
/// </summary>
public class FindParametersReceive(ISerializeStorage serializeStorageRepo,ILogger<FindParametersReceive> LoggerRepo)
    : IResponseReceive<RequestStorageBaseModel?, FoundParameterModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FindCloudParameterReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FoundParameterModel[]?>> ResponseHandleAction(RequestStorageBaseModel? request)
    {
        ArgumentNullException.ThrowIfNull(request);
        LoggerRepo.LogDebug($"call `{GetType().Name}`: {JsonConvert.SerializeObject(request)}");
        return await serializeStorageRepo.Find(request);
    }
}