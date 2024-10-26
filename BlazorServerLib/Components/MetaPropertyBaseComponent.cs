////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;

namespace BlazorWebLib;

/// <summary>
/// MetaPropertyBaseComponent
/// </summary>
public class MetaPropertyBaseComponent : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// Приложения
    /// </summary>
    [Parameter, EditorRequired]
    public required string[] ApplicationsNames { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Parameter, EditorRequired]
    public required string PropertyName { get; set; }

    /// <summary>
    /// Префикс
    /// </summary>
    [Parameter]
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Идентификатор [PK] владельца объекта
    /// </summary>
    [Parameter]
    public int? OwnerPrimaryKey { get; set; }

    /// <summary>
    /// ManageMode
    /// </summary>
    [Parameter]
    public bool ManageMode { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    [Parameter, EditorRequired]
    public  required string Title { get; set; }
}