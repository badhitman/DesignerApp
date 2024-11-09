////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IToolsSystemService
/// </summary>
public interface IToolsSystemService
{
    /// <summary>
    /// ExeCommand
    /// </summary>
    public Task<TResponseModel<string>> ExeCommand(ExeCommandModel req);

    /// <summary>
    /// GetDirectory
    /// </summary>
    public Task<TResponseModel<List<ToolsFilesResponseModel>>> GetDirectory(ToolsFilesRequestModel req);

    /// <summary>
    /// UpdateFile
    /// </summary>
    public Task<TResponseModel<string>> UpdateFile(string fileScopeName, string remoteDirectory, byte[] bytes);

    /// <summary>
    /// UpdateFile
    /// </summary>
    public Task<TResponseModel<bool>> DeleteFile(DeleteRemoteFileRequestModel req);
}