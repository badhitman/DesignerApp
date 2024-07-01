﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared;

/// <summary>
/// Form field badge
/// </summary>
public partial class FormFieldBadgeComponent : ComponentBase
{
    /// <summary>
    /// Поле формы
    /// </summary>
    [Parameter, EditorRequired]
    public required FieldFormBaseLowConstructorModel Field { get; set; }

    /// <summary>
    /// Описание в формате HTML/Markup
    /// </summary>
    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");
}