////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Конфигурация сервера
/// </summary>
public class RestApiConfigBaseModel
{
    /// <inheritdoc/>
    public static readonly string Configuration = "ApiAccess";

    /// <summary>
    /// Пользователи и их права (для удалённого доступа к серверу)
    /// </summary>
    public ExpressUserPermissionModel[]? Permissions { get; set; }

    /// <summary>
    /// TokenAccessHeaderName
    /// </summary>
    public required string TokenAccessHeaderName { get; set; } = "token-access";
}