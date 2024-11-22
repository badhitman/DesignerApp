////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запуск команды (bash/cmd)
/// </summary>
public class ExeCommandModel
{
    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Arguments
    /// </summary>
    public required string Arguments { get; set; }
}