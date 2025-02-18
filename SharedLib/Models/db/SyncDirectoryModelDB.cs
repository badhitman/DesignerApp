﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Папка синхронизации
/// </summary>
public class SyncDirectoryModelDB : ApiRestBaseModel
{
    /// <summary>
    /// LocalDirectory
    /// </summary>
    public string? LocalDirectory { get; set; }

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public string? RemoteDirectory { get; set; }
}