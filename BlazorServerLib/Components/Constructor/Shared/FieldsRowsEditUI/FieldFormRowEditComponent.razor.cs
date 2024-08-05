////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsRowsEditUI;

/// <summary>
/// Field form row edit
/// </summary>
public partial class FieldFormRowEditComponent : FieldFormEditFormBaseComponent
{
    /// <inheritdoc/>
    public TypesFieldsFormsEnum SelectedTypeFieldForEditRow
    {
        get
        {
            return Field.TypeField;
        }
        set
        {
            Field.TypeField = value;

            if (_field_copy.TypeField != Field.TypeField)
                Field.MetadataValueType = null;

            StateHasChangedHandler(Field);
            FieldTextUI?.Update(Field);
            FieldGeneratorUI?.Update(Field);
        }
    }

    /// <inheritdoc/>
    protected string? FieldsNames
    {
        get => (string?)Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, value);
            StateHasChangedHandler(Field);
        }
    }

    /// <inheritdoc/>
    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");

    /// <inheritdoc/>
    protected DeclarationAbstraction? _dc = null;

    /// <inheritdoc/>
    public string? SelectedProgramCalculationField
    {
        get => (string?)Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, "");
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, value);

            Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);
            StateHasChangedHandler(Field);
            if (!string.IsNullOrWhiteSpace(SelectedProgramCalculationField))
                _dc = DeclarationAbstraction.GetHandlerService(SelectedProgramCalculationField);
        }
    }

    /// <inheritdoc/>
    protected IEnumerable<CommandEntryModel> Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();

    FieldFormConstructorModelDB _field_copy = default!;

    /// <inheritdoc/>
    protected TextFieldFormRowEditUIComponent? FieldTextUI;
    /// <inheritdoc/>
    protected GeneratorFieldFormRowEditUIComponent? FieldGeneratorUI;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _field_copy = new FieldFormConstructorModelDB()
        {
            Css = Field.Css,
            Description = Field.Description,
            Hint = Field.Hint,
            Id = Field.Id,
            MetadataValueType = Field.MetadataValueType,
            Name = Field.Name,
            Owner = Field.Owner,
            OwnerId = Field.OwnerId,
            Required = Field.Required,
            SortIndex = Field.SortIndex,
            TypeField = Field.TypeField
        };

        if (!string.IsNullOrWhiteSpace(SelectedProgramCalculationField))
            _dc = DeclarationAbstraction.GetHandlerService(SelectedProgramCalculationField);
    }

    /// <inheritdoc/>
    public override void Update(FieldFormConstructorModelDB field)
    {
        base.Update(field);
        FieldTextUI?.Update(field);
        FieldGeneratorUI?.Update(field);
    }
}