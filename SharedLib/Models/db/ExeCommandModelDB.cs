////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Запуск команды (bash/cmd)
/// </summary>
public class ExeCommandModelDB : ApiRestBaseModel
{
    /// <summary>
    /// FileName
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Arguments
    /// </summary>
    public required string Arguments { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj is ExeCommandModelDB _ec)
            return
                FileName.Equals(_ec.FileName) &&
                Arguments.Equals(_ec.Arguments);

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{FileName} {Arguments}".GetHashCode();
    }
}