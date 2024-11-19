////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// string Number
/// <code>AND</code>
/// string Text
/// </summary>
public class EntryAltExtModel
{
    /// <inheritdoc/>
    public required string Number { get; set; }

    /// <inheritdoc/>
    public required string Text { get; set; }

    /// <inheritdoc/>
    public override string ToString() 
        => $"[{Number}]: '{Text}'";

    /// <inheritdoc/>
    public static bool operator ==(EntryAltExtModel e1, EntryAltExtModel e2)
        => e1.Number == e2.Number && e1.Text == e2.Text;

    /// <inheritdoc/>
    public static bool operator !=(EntryAltExtModel e1, EntryAltExtModel e2)
        => e1.Number != e2.Number || e1.Text != e2.Text;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is EntryAltExtModel _e)
            return _e.Number == Number && _e.Text == Text;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Number} {Text}".GetHashCode();
    }
}