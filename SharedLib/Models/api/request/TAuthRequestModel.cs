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
    /// Пользователь, который отправил запрос (id Identity)
    /// </summary>
    public required string SenderActionUserId { get; set; }

    /// <summary>
    /// Request
    /// </summary>
    public required T Payload { get; set; }
}