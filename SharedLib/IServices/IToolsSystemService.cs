﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IToolsSystemService
/// </summary>
public interface IToolsSystemService
{
    /// <summary>
    /// GetDirectory
    /// </summary>
    public Task<TResponseModel<ToolsFilesResponseModel[]>> GetDirectory(ToolsFilesRequestModel req);
}