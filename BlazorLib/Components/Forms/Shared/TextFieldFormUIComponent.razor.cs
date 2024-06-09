using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class TextFieldFormUIComponent : ComponentBase
{
    [Parameter, EditorRequired]
    public ConstructorFieldFormModelDB FieldObject { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;

    public bool IsMultiline
    {
        get => (bool?)FieldObject.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.IsMultiline, false) == true;
        private set
        {
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.IsMultiline, value);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    /// <summary>
    /// Параметр текстового поля формы
    /// </summary>
    public string? FieldParameter
    {
        get => FieldObject.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "")?.ToString();
        private set
        {
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, value);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }
}