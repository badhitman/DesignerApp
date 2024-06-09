﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

public partial class TextFieldFormRowEditUIComponent : FieldFormEditFormBaseComponent
{
    [Inject]
    protected ILogger<TextFieldFormRowEditUIComponent> _logger { get; set; } = default!;


    /// <summary>
    /// Параметр текстового поля формы // (DescriptorField == PropsTypesMDFieldsEnum.None ? "Плагин отключён" : "")
    /// </summary>
    public string? ParameterField
    {
        get
        {
            return Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "")?.ToString();
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

    protected TextFieldAgentSelectorComponent? _tfAgentSelectorComponent_ref;
    public void SelectAgentAction(EntryAltDescriptionModel selected_element)
    {
        ParameterField = $"{selected_element.Text} #{selected_element.Id}";
        StateHasChanged();
        _tfAgentSelectorComponent_ref?.SetCurrentAgent(ParameterField);
    }

    public PropsTypesMDFieldsEnum DescriptorField
    {
        get
        {
            if (Enum.TryParse(Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, PropsTypesMDFieldsEnum.None.ToString())?.ToString(), out PropsTypesMDFieldsEnum _mode))
                return _mode;

            return PropsTypesMDFieldsEnum.None;
        }
        set
        {
            Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);

            if (value == PropsTypesMDFieldsEnum.None)
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, value.ToString());
            StateHasChangedHandler(Field);
        }
    }

    public bool IsMultiline
    {
        get
        {
            return (bool?)Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.IsMultiline, false) == true;
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