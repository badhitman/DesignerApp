////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Documents;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib;

/// <summary>
/// Form base
/// </summary>
public abstract partial class FormBaseModel : DocumenBodyBaseComponent
{
    /// <summary>
    /// ID
    /// </summary>
    [Parameter, EditorRequired]
    public required string ID { get; set; }

    /// <summary>
    /// Form Metadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormFitModel FormMetadata { get; set; }

    /// <summary>
    /// Формы в табе
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required List<FormBaseModel> FormsStack { get; set; }

    /// <summary>
    /// Форма изменена
    /// </summary>
    public abstract bool IsEdited { get; }

    /// <summary>
    /// Сохранить форму (обработчик команды)
    /// </summary>
    public abstract Task SaveForm();

    /// <summary>
    /// Отмена редактирования формы (обработчик команды)
    /// </summary>
    public abstract void ResetForm();

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if (!FormsStack.Any(x => x.ID == ID))
            FormsStack.Add(this);

        return base.OnInitializedAsync();
    }
}