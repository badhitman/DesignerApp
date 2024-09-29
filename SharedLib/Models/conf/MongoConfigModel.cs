////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// MongoConfigModel
/// </summary>
public class MongoConfigModel : HostConfigModel
{
    /// <summary>
    /// Login
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// FilesSystemName
    /// </summary>
    public required string FilesSystemName { get; set; } = "files-system";

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password)
            ? $"{Sheme}://{Host}:{Port}"
            : $"{Sheme}://{Login}:{Password}@{Host}:{Port}";
    }
}