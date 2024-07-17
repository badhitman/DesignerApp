namespace SharedLib;

/// <summary>
/// Summary builder
/// </summary>
public class SummaryBuilder
{
    const string inheritdoc = "<inheritdoc/>";

    /// <summary>
    /// Summary builder
    /// </summary>
    public SummaryBuilder(byte setTabulationsSpiceSize = 0)
    {
        TabulationsSpiceSize = setTabulationsSpiceSize;
    }

    string tabs = "";
    byte tabulationsSpiceSize;
    /// <summary>
    /// Размер отступа в табуляторах
    /// </summary>
    public byte TabulationsSpiceSize
    {
        get => tabulationsSpiceSize;
        private set
        {
            if (tabulationsSpiceSize != value)
            {
                tabs = "";
                for (int i = 0; i < value; i++)
                    tabs += "\t";

                if (_summaryText is not null)
                    SetSummary();
            }

            tabulationsSpiceSize = value;
        }
    }

    /// <summary>
    /// Summary text
    /// </summary>
    public string SummaryText { get; private set; } = inheritdoc;

    string[]? _summaryText;
    /// <summary>
    /// Set summary text
    /// </summary>
    public SummaryBuilder UseSummaryText(string[] summaryText)
    {
        _summaryText = summaryText;
        SetSummary();
        return this;
    }

    void SetSummary()
    {
        string res = $"{tabs}/// <summary>";
        foreach (string summary in _summaryText!)
            res = $"{res}\n{tabs}/// {summary}";

        SummaryText = $"{res}\n{tabs}/// </summary>";
    }

    /// <summary>
    /// Set inheritdoc
    /// </summary>
    public SummaryBuilder UseInheritdoc()
    {
        SummaryText = inheritdoc;
        return this;
    }

    /// <summary>
    /// Use tabulations spice size
    /// </summary>
    public SummaryBuilder UseTabulationsSpiceSize(byte tabulationsSpiceSize)
    {
        TabulationsSpiceSize = tabulationsSpiceSize;
        return this;
    }
}