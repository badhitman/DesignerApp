////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;
using SharedLib;

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
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.CMD_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.EXE_ACTION_NAME}")]
    public TResponseModel<string> ExeCommand(ExeCommandModel req)
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

        return res;
    }

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
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.SystemRoot"/>
    /// </remarks>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}")]
    public async Task<TResponseModel<bool>> FileUpdateOrCreate(IFormFile uploadedFile, [FromHeader] string remoteDirectory)
    {
        TResponseModel<bool> response = new();
        remoteDirectory = Encoding.UTF8.GetString(Convert.FromBase64String(remoteDirectory));
        remoteDirectory = remoteDirectory.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

        if (uploadedFile is null || uploadedFile.Length == 0)
        {
            response.AddError("Данные файла отсутствуют");
            return response;
        }

        string _file_name = Path.Combine(remoteDirectory, uploadedFile.FileName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Trim());
        using MemoryStream ms = new();
        uploadedFile.OpenReadStream().CopyTo(ms);

        return await toolsRepo.UpdateFile(_file_name, remoteDirectory, ms.ToArray());
    }

    /// <summary>
    /// AttachmentForOrder
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.SystemRoot"/>
    /// </remarks>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.DELETE_ACTION_NAME}")]
    public async Task<TResponseModel<bool>> FileDelete(DeleteRemoteFileRequestModel req)
    {
        return await toolsRepo.DeleteFile(req);
    }
}