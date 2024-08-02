////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;

namespace BlazorWebLib;

/// <summary>
/// FieldBaseComponent
/// </summary>
public abstract class FieldBaseComponentModel : ComponentBase
{
    /// <inheritdoc/>
    [Parameter]
    public string? Id { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Label { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Hint { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool Required { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormBaseModel ParentForm { get; set; }

    /// <summary>
    /// Поле изменило своё значение от исходного
    /// </summary>
    public abstract bool IsEdited { get; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentForm.FieldsComponents.Add(this);
    }
}