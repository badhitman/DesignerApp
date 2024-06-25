using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsRowsEditUI;

/// <summary>
/// Text field form row edit UI
/// </summary>
public partial class TextFieldFormRowEditUIComponent : FieldFormEditFormBaseComponent
{
    /// <summary>
    /// Параметр текстового поля формы
    /// </summary>
    public string? ParameterField
    {
        get
        {
            return Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "")?.ToString();
        }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, value);
            StateHasChangedHandler(Field);
        }
    }

    /// <inheritdoc/>
    protected TextFieldAgentSelectorComponent? _tfAgentSelectorComponent_ref;
    /// <inheritdoc/>
    public void SelectAgentAction(EntryAltDescriptionModel selected_element)
    {
        ParameterField = $"{selected_element.Name} #{selected_element.Id}";
        StateHasChanged();
        _tfAgentSelectorComponent_ref?.SetCurrentAgent(ParameterField);
    }

    /// <inheritdoc/>
    public PropsTypesMDFieldsEnum DescriptorField
    {
        get
        {
            if (Enum.TryParse(Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, PropsTypesMDFieldsEnum.None.ToString())?.ToString(), out PropsTypesMDFieldsEnum _mode))
                return _mode;

            return PropsTypesMDFieldsEnum.None;
        }
        set
        {
            if (value == PropsTypesMDFieldsEnum.None)
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);

            if (value == PropsTypesMDFieldsEnum.None)
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, value.ToString());
            StateHasChangedHandler(Field);
        }
    }

    string GetHelpTextForPlugin
    {
        get
        {
            string? pf = ParameterField;
            return DescriptorField switch
            {
                PropsTypesMDFieldsEnum.None => $"Плагин отключён{(string.IsNullOrWhiteSpace(pf) ? "" : $" (опция `{pf}` игнорируется)")}",
                PropsTypesMDFieldsEnum.TextMask => "Спецификация: a - это буквы, 0 - цифры и * - любой символ. остальные символы - как [разделители]",
                PropsTypesMDFieldsEnum.Template => "Статический текст или применение агентов поведения",
                _ => throw new Exception($"Тип опции [{DescriptorField}] не опознан")
            };
        }
    }

    /// <inheritdoc/>
    public bool IsMultiline
    {
        get
        {
            return (bool?)Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.IsMultiline, false) == true;
        }
        private set
        {
            if (!value)
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.IsMultiline);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.IsMultiline, value);
            StateHasChangedHandler(Field);
        }
    }
}