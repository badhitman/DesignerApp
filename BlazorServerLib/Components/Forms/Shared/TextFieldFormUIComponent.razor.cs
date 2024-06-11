using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Text field form UI
/// </summary>
public partial class TextFieldFormUIComponent : ComponentBase
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public ConstructorFieldFormModelDB FieldObject { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }
}