////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared;

/// <inheritdoc/>
public partial class GeneratorFieldFormUIComponent : ComponentBase
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required FieldFormConstructorModelDB FieldObject { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required Action<FieldFormBaseLowConstructorModel, Type> StateHasChangedHandler { get; set; }

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

    IEnumerable<EntryAltDescriptionModel> Entries = [];
    string? selected_generator_field;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<FieldValueGeneratorAbstraction>().ToArray();

        if (Entries.Any())
            SelectedGeneratorField = Entries.First().Id;
    }

    /// <inheritdoc/>
    public void Update(FieldFormBaseLowConstructorModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }
}