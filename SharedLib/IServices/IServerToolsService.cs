////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IServerToolsService
/// </summary>
public interface IServerToolsService
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
    /// Обновить файл (или создать, если его нет)
    /// </summary>
    /// <remarks>
    /// Если файла не существует, то создаёт его. В противном случае перезаписывает/обновляет.
    /// </remarks>
    public Task<TResponseModel<string>> UpdateFile(string fileScopeName, string remoteDirectory, byte[] bytes);

    /// <summary>
    /// UpdateFile
    /// </summary>
    public Task<TResponseModel<bool>> DeleteFile(DeleteRemoteFileRequestModel req);
}