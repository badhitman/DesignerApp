////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;

namespace BlazorWebLib;

/// <summary>
/// Form base
/// </summary>
public abstract partial class FormBaseModel : FormBaseCore
{
    /// <summary>
    /// ID
    /// </summary>
    [Parameter, EditorRequired]
    public required string ID { get; set; }

    /// <summary>
    /// Формы в табе
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required List<FormBaseModel> FormsStack { get; set; }

    /// <summary>
    /// FormChangeAction
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required Action<FormBaseModel> FormChangeAction { get; set; }


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