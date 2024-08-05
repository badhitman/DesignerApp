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

    /// <inheritdoc/>
    public string SystemName { get; set; }

    /// <summary>
    /// Заголовок вкладки
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Tooltip вкладки
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// IsDisabled
    /// </summary>
    public bool IsDisabled { get; set; }
}