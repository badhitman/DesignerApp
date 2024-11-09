﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

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

        DirectoryInfo _di = new(req.RemoteDirectory);
        if (!_di.Exists)
            res.AddError($"Папки {req.RemoteDirectory} ({_di.FullName}) не существует");
        else
        {
            string[] all_files = GlobalTools.GetFiles(_di.FullName).ToArray();
            res.Response = all_files.Select(x =>
            {
                FileInfo fileInfo = new(x);

                string? Hash = null;
                if (req.CalculationHash)
                {
                    using MD5 md5 = MD5.Create();
                    using FileStream stream = File.OpenRead(fileInfo.FullName);
                    Hash = Convert.ToBase64String(md5.ComputeHash(stream));
                }

                return new ToolsFilesResponseModel()
                {
                    FullName = x,
                    ScopeName = x.StartsWith(req.RemoteDirectory) ? x[req.RemoteDirectory.Length..] : x,
                    Size = fileInfo.Length,
                    Hash = Hash,
                };
            }).ToList();
        }
        return Task.FromResult(res);
    }

    /// <inheritdoc/>
    public Task<TResponseModel<bool>> DeleteFile(DeleteRemoteFileRequestModel req)
    {
        TResponseModel<bool> response = new();
        DirectoryInfo di = new(req.RemoteDirectory);
        if (!di.Exists)
        {
            response.AddError("Папка отсутствует");
            return Task.FromResult(response);
        }

        string _file_name = Path.Combine(req.RemoteDirectory, req.SafeScopeName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Trim());
        FileInfo _file = new(_file_name);
        if (_file.Exists)
        {
            _file.Delete();
            response.Response = true;
        }
        else
        {
            response.AddWarning($"Файл отсутствует: {_file.FullName}");
            response.Response = false;
        }

        return Task.FromResult(response);
    }

    /// <inheritdoc/>
    public Task<TResponseModel<bool>> UpdateFile(string fileName, string remoteDirectory, byte[] bytes)
    {
        TResponseModel<bool> response = new();

        DirectoryInfo di = new(remoteDirectory);
        if (!di.Exists)
        {
            response.AddError("Папка отсутствует");
            return Task.FromResult(response);
        }

        string _file_name = Path.Combine(remoteDirectory, fileName);
        FileInfo _file = new(_file_name);
        if (_file.Exists)
            _file.Delete();

        using MemoryStream stream = new(bytes);

        string _tmpFile = Path.GetTempFileName();

        File.WriteAllBytes(_tmpFile, stream.ToArray());
        using (ZipArchive archive = ZipFile.OpenRead(_tmpFile))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                _file = new(Path.Combine(di.FullName, entry.FullName));
                if (_file.Exists)
                    _file.Delete();
                entry.ExtractToFile(_file.FullName);
            }
        }
        File.Delete(_tmpFile);

        response.Response = true;
        return Task.FromResult(response);
    }

    /// <inheritdoc/>
    public Task<TResponseModel<string>> ExeCommand(ExeCommandModel req)
    {
        TResponseModel<string> res = new();

        try
        {
            res.Response = GlobalTools.RunCommandWithBash(req.Arguments, req.FileName);
        }
        catch (Exception ex)
        {
            res.Messages.InjectException(ex);
        }

        return Task.FromResult(res);
    }
}
