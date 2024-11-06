////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Tools
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), LoggerNolog]
#if DEBUG
[AllowAnonymous]
#else
[Authorize(Roles = nameof(ExpressApiRolesEnum.SystemRoot))]
#endif
public class ToolsController(IToolsSystemService toolsRepo) : ControllerBase
{
    /// <summary>
    /// GetDirectory
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.SystemRoot"/>
    /// </remarks>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.GET_ACTION_NAME}")]
#if DEBUG
    [AllowAnonymous]
#endif
    public Task<TResponseModel<ToolsFilesResponseModel[]>> GetDirectory(ToolsFilesRequestModel req)
        => toolsRepo.GetDirectory(req);
}