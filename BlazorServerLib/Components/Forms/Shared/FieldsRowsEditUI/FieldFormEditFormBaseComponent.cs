////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsRowsEditUI;

/// <summary>
/// Field form edit form base
/// </summary>
public class FieldFormEditFormBaseComponent : ComponentBase, IDomBaseComponent
{
    /// <summary>
    /// Snackbar
    /// </summary>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required FieldFormConstructorModelDB Field { get; set; }

    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<FieldFormConstructorModelDB> StateHasChangedHandler { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public SessionOfDocumentDataModelDB? SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    public string DomID => $"{SessionQuestionnaire?.Id}_form-{Form.Id}_{Field.GetType().FullName}-{Field.Id}";

    /// <summary>
    /// Placeholder
    /// </summary>
    public string? Placeholder
    {
        get
        {
            return (string?)Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Placeholder, "");
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Placeholder);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Placeholder, value);
            StateHasChangedHandler(Field);
        }
    }

    /// <summary>
    /// Update field + <c>StateHasChanged()</c>
    /// </summary>
    public virtual void Update(FieldFormConstructorModelDB field)
    {
        Field.Update(field);
        StateHasChanged();
    }
}