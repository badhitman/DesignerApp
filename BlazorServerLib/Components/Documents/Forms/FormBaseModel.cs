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
    /// Поля формы для отслеживания их состояния
    /// </summary>
    public List<FieldBaseComponentModel> FieldsComponents { get; set; } = [];

    /// <summary>
    /// Форма изменена
    /// </summary>
    public override bool IsEdited => FieldsComponents.Any(x => x.IsEdited);

    /// <summary>
    /// Сохранить форму (обработчик команды)
    /// </summary>
    public abstract Task SaveForm();

    /// <summary>
    /// Отмена редактирования формы (обработчик команды)
    /// </summary>
    public virtual void ResetForm()
    {
        foreach (FieldBaseComponentModel fb in FieldsComponents)
        {
            fb.Reset();
            fb.StateHasChangedCall();
        }
    }


    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if (!FormsStack.Any(x => x.ID == ID))
            FormsStack.Add(this);

        return base.OnInitializedAsync();
    }
}