namespace BlazorWebLib;

/// <summary>
/// IdleCircuitOptions
/// </summary>
public class IdleCircuitOptions
{
    /// <summary>
    /// IdleTimeout
    /// </summary>
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
