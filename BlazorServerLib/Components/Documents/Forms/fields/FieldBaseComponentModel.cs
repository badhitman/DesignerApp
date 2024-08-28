////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorWebLib;

/// <summary>
/// FieldBaseComponent
/// </summary>
public abstract class FieldBaseComponentModel : BlazorBusyComponentBaseModel
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
    [Parameter]
    public bool Readonly { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormBaseModel ParentForm { get; set; }


    /// <summary>
    /// Поле изменило своё значение от исходного
    /// </summary>
    public abstract bool IsEdited { get; }


    /// <summary>
    /// сбросить состояние поля в исходное
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// применить состояние поля в конечное
    /// </summary>
    public abstract void CommitChange();


    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentForm.FieldsComponents.Add(this);
    }
}