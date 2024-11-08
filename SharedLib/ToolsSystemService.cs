////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Diagnostics;

namespace SharedLib;

/// <summary>
/// ToolsSystemService
/// </summary>
public class ToolsSystemService() : IToolsSystemService
{
    /// <inheritdoc/>
    public Task<TResponseModel<List<ToolsFilesResponseModel>>> GetDirectory(ToolsFilesRequestModel req)
    {
        TResponseModel<List<ToolsFilesResponseModel>> res = new();
        FileVersionInfo? mfi = null;
        DirectoryInfo _di = new(req.RemoteDirectory);
        if (!_di.Exists)
            res.AddError($"Папки {req.RemoteDirectory} ({_di.FullName}) не существует");
        else
        {
            string[] all_files = GlobalTools.GetFiles(_di.FullName).ToArray();
            res.Response = all_files.Select(x =>
            {
                FileInfo fileInfo = new(x);

                mfi = req.CalculationVersion
                ? FileVersionInfo.GetVersionInfo(fileInfo.FullName)
                : null;

                return new ToolsFilesResponseModel()
                {
                    FullName = x,
                    ScopeName = x.StartsWith(req.RemoteDirectory) ? x[req.RemoteDirectory.Length..] : x,
                    Size = fileInfo.Length,
                    Version = mfi?.FileVersion,
                };
            }).ToList();
        }
        return Task.FromResult(res);
    }
}
