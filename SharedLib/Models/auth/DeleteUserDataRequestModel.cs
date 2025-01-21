////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Удалить Identity данные пользователя
/// </summary>
public class DeleteUserDataRequestModel
{
    /// <summary>
    /// UserId
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public required string Password { get; set; }
}