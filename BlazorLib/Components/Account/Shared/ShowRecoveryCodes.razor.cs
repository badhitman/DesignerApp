using Microsoft.AspNetCore.Components;

namespace BlazorLib.Components.Account.Shared;

/// <inheritdoc/>
public partial class ShowRecoveryCodes : ComponentBase
{
    /// <inheritdoc/>
    [Parameter]
    public string[] RecoveryCodes { get; set; } = [];

    /// <inheritdoc/>
    [Parameter]
    public string? StatusMessage { get; set; }
}