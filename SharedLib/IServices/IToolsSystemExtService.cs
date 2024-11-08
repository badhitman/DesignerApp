////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IToolsSystemExtService
/// </summary>
public interface IToolsSystemExtService : IToolsSystemService
{
    /// <summary>
    /// GetMe
    /// </summary>
    public Task<TResponseModel<ExpressProfileResponseModel>> GetMe();

    /// <summary>
    /// UpdateFile
    /// </summary>
    public Task<TResponseModel<bool>> UpdateFile(ToolsFilesResponseModel tFile, byte[] bytes);



    /// <summary>
    /// UpdateFile
    /// </summary>
    public Task<TResponseModel<bool>> DeleteFile(DeleteRemoteFileRequestModel req);
}