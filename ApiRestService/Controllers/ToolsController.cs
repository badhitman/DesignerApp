////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SharedLib;
using System.IO.Compression;
using System.Text;

namespace ApiRestService.Controllers;

/// <summary>
/// Tools
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), LoggerNolog]
[Authorize(Roles = nameof(ExpressApiRolesEnum.SystemRoot))]
public class ToolsController(IToolsSystemService toolsRepo) : ControllerBase
{
    /// <summary>
    /// GetDirectory
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.SystemRoot"/>
    /// </remarks>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.GET_ACTION_NAME}")]
    public Task<TResponseModel<List<ToolsFilesResponseModel>>> GetDirectory(ToolsFilesRequestModel req)
        => toolsRepo.GetDirectory(req);

    /// <summary>
    /// AttachmentForOrder
    /// </summary>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}")]
    public TResponseModel<bool> FileUpdateOrCreate(IFormFile uploadedFile, [FromHeader] string RemoteDirectory)
    {
        TResponseModel<bool> response = new();
        RemoteDirectory = Encoding.UTF8.GetString(Convert.FromBase64String(RemoteDirectory));
        RemoteDirectory = RemoteDirectory.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

        if (uploadedFile is null || uploadedFile.Length == 0)
        {
            response.AddError("Данные файла отсутствуют");
            return response;
        }

        DirectoryInfo di = new(RemoteDirectory);
        if (!di.Exists)
        {
            response.AddError("Папка отсутствует");
            return response;
        }

        string _file_name = Path.Combine(RemoteDirectory, uploadedFile.FileName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Trim());
        FileInfo _file = new(_file_name);
        if (_file.Exists)
            _file.Delete();

        using MemoryStream stream = new();
        uploadedFile.OpenReadStream().CopyTo(stream);
        string _tmpFile = System.IO.Path.GetTempFileName();
        //Path.Combine(di.FullName, _file.FullName,"")
        System.IO.File.WriteAllBytes(_tmpFile, stream.ToArray());
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
        System.IO.File.Delete(_tmpFile);

        response.Response = true;
        return response;
    }

    /// <summary>
    /// AttachmentForOrder
    /// </summary>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.DELETE_ACTION_NAME}")]
    public TResponseModel<bool> FileDelete(DeleteRemoteFileRequestModel req)
    {
        TResponseModel<bool> response = new();
        DirectoryInfo di = new(req.RemoteDirectory);
        if (!di.Exists)
        {
            response.AddError("Папка отсутствует");
            return response;
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

        return response;
    }
}