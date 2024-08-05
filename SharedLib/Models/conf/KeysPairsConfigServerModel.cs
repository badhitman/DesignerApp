////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Полный конфиг ключей (публичный +приватный)
/// </summary>
public class KeysPairsConfigServerModel : KeysPairsConfigClientModel
{
    /// <summary>
    /// Приватный ключ
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;
}