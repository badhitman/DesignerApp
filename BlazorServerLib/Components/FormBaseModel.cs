////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components;

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
    [Parameter, EditorRequired]
    public required FormFitModel FormMetadata { get; set; }

    /// <summary>
    /// Parent tab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentConstructorComponent ParentTab { get; set; }

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
    public abstract Task ResetForm();

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if (!ParentTab.FormsStack.Any(x => x.ID == ID))
            ParentTab.FormsStack.Add(this);

        return base.OnInitializedAsync();
    }
}