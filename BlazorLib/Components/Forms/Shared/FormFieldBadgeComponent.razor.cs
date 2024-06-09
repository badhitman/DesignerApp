using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class FormFieldBadgeComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public ConstructorFieldFormBaseLowModel Field { get; set; } = default!;

    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");
}