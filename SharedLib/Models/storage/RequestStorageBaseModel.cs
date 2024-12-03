////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// RequestStorageCloudParameterModel
/// </summary>
[Index(nameof(ApplicationName), nameof(PropertyName))]
public class RequestStorageBaseModel
{
    /// <summary>
    /// Имя приложения, которое обращается к службе облачного хранения параметров
    /// </summary>
    public required string ApplicationName { get; set; }


    /// <summary>
    /// Имя
    /// </summary>
    public required string PropertyName { get; set; }

    /// <summary>
    /// Normalize
    /// </summary>
    public virtual void Normalize()
    {
        ApplicationName = ApplicationName.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}