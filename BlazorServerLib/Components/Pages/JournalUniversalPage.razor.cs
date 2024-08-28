////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib.Components.Pages;

/// <summary>
/// Журнал документов (универсальный)
/// </summary>
public partial class JournalUniversalPage : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Тип документа
    /// </summary>
    [Parameter]
    public string? DocumentNameOrId { get; set; }

    /// <summary>
    /// ProjectId
    /// </summary>
    [Parameter, SupplyParameterFromQuery]
    public int? ProjectId { get; set; }
}