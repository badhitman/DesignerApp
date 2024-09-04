////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib.Components.Commerce;

/// <summary>
/// Организации пользователя
/// </summary>
public partial class OrganizationsForUserPage : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Пользователь, для которого отобразить организации
    /// </summary>
    [Parameter]
    public string? UserId { get; set; }
}