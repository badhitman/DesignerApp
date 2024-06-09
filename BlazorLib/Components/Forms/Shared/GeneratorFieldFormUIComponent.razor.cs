using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class GeneratorFieldFormUIComponent : ComponentBase
{
    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Parameter, EditorRequired]
    public ConstructorFieldFormModelDB FieldObject { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;

    string? _generation_options;
    string? OptionsGeneration
    {
        get => _generation_options;
        set
        {
            _generation_options = value;
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, OptionsGeneration);
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, SelectedGeneratorField);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    IEnumerable<EntryAltDescriptionModel> Entries = Enumerable.Empty<EntryAltDescriptionModel>();
    string? selected_generator_field;
    public string? SelectedGeneratorField
    {
        get => selected_generator_field;
        private set
        {
            selected_generator_field = value;
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, OptionsGeneration);
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, SelectedGeneratorField);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<FieldValueGeneratorAbstraction>().ToArray();

        if (Entries.Any())
            SelectedGeneratorField = Entries.First().Id;
    }

    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }
}