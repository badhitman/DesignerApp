////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Попытка удалить предоставленную внешнюю информацию для входа из указанного userId
/// и возвращение флага, указывающий, удалось ли удаление или нет
/// </summary>
public class RemoveLoginRequestModel
{
    /// <summary>
    /// LoginProvider
    /// </summary>
    public required string LoginProvider { get; set; }

    /// <summary>
    /// ProviderKey
    /// </summary>
    public required string ProviderKey { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }
}