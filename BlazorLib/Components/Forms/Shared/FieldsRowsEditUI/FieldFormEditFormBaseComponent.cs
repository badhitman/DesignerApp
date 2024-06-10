using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

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
    public required ConstructorFieldFormModelDB Field { get; set; }

    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<ConstructorFieldFormModelDB> StateHasChangedHandler { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public ConstructorFormSessionModelDB? SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    public string DomID => $"{SessionQuestionnaire?.Id}_form-{Form.Id}_{Field.GetType().FullName}-{Field.Id}";

    /// <summary>
    /// Placeholder
    /// </summary>
    public string? Placeholder
    {
        get
        {
            return (string?)Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Placeholder, "");
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
    public virtual void Update(ConstructorFieldFormModelDB field)
    {
        Field.Update(field);
        StateHasChanged();
    }
}