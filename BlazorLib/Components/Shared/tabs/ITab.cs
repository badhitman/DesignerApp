using Microsoft.AspNetCore.Components;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// Интерфейс вкладки
/// </summary>
public interface ITab
{
    /// <summary>
    /// Содержимое вкладки
    /// </summary>
    RenderFragment? ChildContent { get; }
}