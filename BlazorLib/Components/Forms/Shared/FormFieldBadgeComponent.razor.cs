using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

/// <summary>
/// Form field badge
/// </summary>
public partial class FormFieldBadgeComponent : ComponentBase
{
    /// <summary>
    /// Поле формы
    /// </summary>
    [Parameter, EditorRequired]
    public required ConstructorFieldFormBaseLowModel Field { get; set; }

    /// <summary>
    /// Описание в формате HTML/Markup
    /// </summary>
    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");
}