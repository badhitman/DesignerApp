////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Создание токена изменения адреса электронной почты для указанного пользователя.
/// </summary>
public class GenerateChangeEmailTokenRequestModel
{
    /// <summary>
    /// Пользователь, для которого создаются коды восстановления.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// BaseAddress
    /// </summary>
    public required string BaseAddress { get; set; }

    /// <summary>
    /// NewEmail
    /// </summary>
    public required string NewEmail { get; set; }
}