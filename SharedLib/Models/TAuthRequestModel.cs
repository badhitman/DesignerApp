////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Авторизованный запрос (от имени пользователя)
/// </summary>
public class TAuthRequestModel<T>
{
    /// <summary>
    /// User id (Identity)
    /// </summary>
    public required string UserIdentityId { get; set; }

    /// <summary>
    /// Request
    /// </summary>
    public required T Request { get; set; }
}