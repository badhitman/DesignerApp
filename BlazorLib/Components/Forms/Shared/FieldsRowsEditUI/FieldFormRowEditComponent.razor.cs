using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

public partial class FieldFormRowEditComponent : FieldFormEditFormBaseComponent
{
    public TypesFieldsFormsEnum SelectedTypeFieldForEdirRow
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

    protected string? FieldsNames
    {
        get => (string?)Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, value);
            StateHasChangedHandler(Field);
        }
    }

    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");

    protected DeclarationAbstraction? _dc = null;

    public string? SelectedProgrammCalcField
    {
        get => (string?)Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, "");
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, value);

            Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);
            StateHasChangedHandler(Field);
            if (!string.IsNullOrWhiteSpace(SelectedProgrammCalcField))
                _dc = DeclarationAbstraction.GetHandlerService(SelectedProgrammCalcField);
        }
    }

    protected IEnumerable<EntryAltDescriptionModel> Entries = Enumerable.Empty<EntryAltDescriptionModel>();

    ConstructorFieldFormModelDB _field_copy = default!;

    protected TextFieldFormRowEditUIComponent? FieldTextUI;
    protected GeneratorFieldFormRowEditUIComponent? FieldGeneratorUI;

    protected override void OnInitialized()
    {
        _field_copy = new ConstructorFieldFormModelDB()
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
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalcAbstraction>();
        if (!string.IsNullOrWhiteSpace(SelectedProgrammCalcField))
            _dc = DeclarationAbstraction.GetHandlerService(SelectedProgrammCalcField);
    }

    public override void Update(ConstructorFieldFormModelDB field)
    {
        base.Update(field);
        FieldTextUI?.Update(field);
        FieldGeneratorUI?.Update(field);
    }
}