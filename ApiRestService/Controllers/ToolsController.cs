////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using SharedLib;
using Microsoft.Extensions.Options;

namespace ApiRestService.Controllers;

/// <summary>
/// Tools
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), LoggerNolog]
[Authorize(Roles = nameof(ExpressApiRolesEnum.SystemRoot))]
public class ToolsController(IToolsSystemService toolsRepo, IManualCustomCacheService memCache, IOptions<PartUploadSessionConfigModel> сonfigPartUploadSession) : ControllerBase
{
    static readonly MemCachePrefixModel PartUploadCacheSessionsPrefix = new($"{GlobalStaticConstants.Routes.PART_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPLOAD_ACTION_NAME}", GlobalStaticConstants.Routes.SESSIONS_CONTROLLER_NAME);
    static readonly MemCachePrefixModel PartUploadCacheFilesMarkersPrefix = new($"{GlobalStaticConstants.Routes.PART_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPLOAD_ACTION_NAME}", GlobalStaticConstants.Routes.MARK_ACTION_NAME);


    /// <summary>
    /// Загрузка порции файла
    /// </summary>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.PART_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPLOAD_ACTION_NAME}")]
    public async Task<ResponseBaseModel> PartUpload(IFormFile uploadedFile, [FromHeader(Name = $"{GlobalStaticConstants.Routes.SESSION_CONTROLLER_NAME}_{GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME}")] string sessionToken, [FromHeader(Name = $"{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}_{GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME}")] string fileToken)
    {
        if (uploadedFile is null || uploadedFile.Length == 0)
            return ResponseBaseModel.CreateError("Данные файла отсутствуют");

        sessionToken = Encoding.UTF8.GetString(Convert.FromBase64String(sessionToken));
        fileToken = Encoding.UTF8.GetString(Convert.FromBase64String(fileToken));

        using MemoryStream ms = new();
        uploadedFile.OpenReadStream().CopyTo(ms);

        PartUploadSessionModel? sessionUploadPart = await memCache.GetObjectAsync<PartUploadSessionModel>(new MemCacheComplexKeyModel(sessionToken, PartUploadCacheSessionsPrefix));

        if (sessionUploadPart is null)
            return ResponseBaseModel.CreateError("Сессия не найдена");

        FilePartMetadataModel currentPartMetadata = sessionUploadPart.FilePartsMetadata.First(x => x.PartFileId == fileToken);

        FilePartMetadataModel[] partsFiles = sessionUploadPart.FilePartsMetadata
            .SkipWhile(x => x.PartFileId.Equals(fileToken))
            .ToArray();

        if (partsFiles.Length == 0)
        {
            string _file_name = Path.Combine(sessionUploadPart.RemoteDirectory, uploadedFile.FileName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Trim());
            return await toolsRepo.UpdateFile(_file_name, sessionUploadPart.RemoteDirectory, ms.ToArray());
        }
        else
        {
            int _countMarkers = 0;
            await Task.WhenAll(partsFiles.Select(x => Task.Run(async () =>
            {
                string? marker = await memCache.GetStringValueAsync(new MemCacheComplexKeyModel(x.PartFileId, PartUploadCacheFilesMarkersPrefix));
                if (string.IsNullOrWhiteSpace(marker))
                    Interlocked.Increment(ref _countMarkers);
            })));

            if (_countMarkers == 0)
            {

            }
        }

        return ResponseBaseModel.CreateSuccess("Done!");

        // return await toolsRepo.PartUpload(new(sessionToken, fileToken, ms.ToArray(), uploadedFile.FileName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Trim()));
    }

    /// <summary>
    /// Создать сессию порционной (частями) загрузки файлов
    /// </summary>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.SESSION_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.PART_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPLOAD_ACTION_NAME}-{GlobalStaticConstants.Routes.START_ACTION_NAME}")]
    public async Task<TResponseModel<PartUploadSessionModel>> PartUploadSessionStart(PartUploadSessionStartRequestModel req)
    {
        TResponseModel<PartUploadSessionModel> res = new();
        DirectoryInfo _di = new(req.RemoteDirectory);
        if (!_di.Exists)
        {
            res.AddError($"Удалённая папка `{_di.FullName}` не существует.");
            return res;
        }

        res.Response = new()
        {
            FilePartsMetadata = [],
            SessionId = Guid.NewGuid().ToString(),

            RemoteDirectory = _di.FullName,
        };

        long scaleFileSize = req.FileSize;

        int partsCount = (int)Math.Ceiling(req.FileSize / (double)сonfigPartUploadSession.Value.PartUploadSize);
        for (uint i = 0; i < partsCount; i++)
        {
            if (scaleFileSize == 0)
                throw new Exception("Ошибка нарезки файла");

            FilePartMetadataModel _rq = new()
            {
                PartFileId = Guid.NewGuid().ToString(),
                PartFilePositionStart = req.FileSize - scaleFileSize, //i * сonfigPartUploadSession.Value.PartUploadSize,
                PartFilePositionEnd = (i + 1) * Math.Min(scaleFileSize, сonfigPartUploadSession.Value.PartUploadSize),
                PartFileIndex = i,
            };
            res.Response.FilePartsMetadata.Add(_rq);
            scaleFileSize -= _rq.PartFilePositionEnd - _rq.PartFilePositionStart;
        }

        await memCache.SetObjectAsync(new MemCacheComplexKeyModel(res.Response.SessionId, PartUploadCacheSessionsPrefix), res.Response, TimeSpan.FromSeconds(сonfigPartUploadSession.Value.PartUploadSessionTimeoutSeconds));
        return res;
    }

    /// <summary>
    /// Обновить файл (или создать если его не существует)
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.SystemRoot"/>
    /// </remarks>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}")]
    public async Task<TResponseModel<string>> FileUpdateOrCreate(IFormFile uploadedFile, [FromHeader(Name = $"{GlobalStaticConstants.Routes.REMOTE_CONTROLLER_NAME}_{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}")] string remoteDirectory)
    {
        TResponseModel<string> response = new();
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
    /// Выполнить команду shell/cmd
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
    /// Получить список файлов из директории
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.SystemRoot"/>
    /// </remarks>
    [HttpPost($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.GET_ACTION_NAME}")]
    public Task<TResponseModel<List<ToolsFilesResponseModel>>> GetDirectory(ToolsFilesRequestModel req)
        => toolsRepo.GetDirectory(req);

    /// <summary>
    /// Удалить файл
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