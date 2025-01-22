////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib.Components.Account;

/// <summary>
/// LoginWith2fa
/// </summary>
public partial class LoginWith2faComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// UserAlias
    /// </summary>
    [Parameter]
    public string? UserAlias { get; set; }

    /// <summary>
    /// ReturnUrl
    /// </summary>
    [Parameter]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// RememberMe
    /// </summary>
    [Parameter]
    public bool RememberMe { get; set; }

    /// <summary>
    /// RememberMachine
    /// </summary>
    [Parameter]
    public bool RememberMachine { get; set; }
}