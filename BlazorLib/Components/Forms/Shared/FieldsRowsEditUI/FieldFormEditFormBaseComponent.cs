using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

public class FieldFormEditFormBaseComponent : ComponentBase
{
    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public Action<ConstructorFieldFormModelDB> StateHasChangedHandler { get; set; } = default!;

    [CascadingParameter]
    public ConstructorFormSessionModelDB? SessionQuestionnairie { get; set; }

    [Parameter, EditorRequired]
    public ConstructorFieldFormModelDB Field { get; set; } = default!;

    [Inject]
    protected ISnackbar snackbar { get; set; } = default!;

    public string DomID => $"{SessionQuestionnairie?.Id}_form-{Form.Id}_{Field.GetType().FullName}-{Field.Id}";

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

    public virtual void Update(ConstructorFieldFormModelDB field)
    {
        Field.Update(field);
        StateHasChanged();
    }
}