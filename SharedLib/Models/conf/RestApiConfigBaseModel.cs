////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация сервера
/// </summary>
public class RestApiConfigBaseModel
{
    /// <summary>
    /// Пользователи и их права (для удалённого доступа к сервру)
    /// </summary>
    public ExpressUserPermissionModel[]? Permissions { get; set; }
}