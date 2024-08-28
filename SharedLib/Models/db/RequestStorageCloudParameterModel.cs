////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// RequestStorageCloudParameterModel
/// </summary>
[Index(nameof(ApplicationName), nameof(Name))]
public class RequestStorageCloudParameterModel
{
    /// <summary>
    /// Имя приложения, которое обращается к службе облачного хранения параметров
    /// </summary>
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Имя параметра
    /// </summary>
    public required string Name { get; set; }
}