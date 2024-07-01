////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared;

/// <summary>
/// Program calculation field form UI
/// </summary>
public partial class ProgramCalculationFieldFormUIComponent : ComponentBase
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public FieldFormConstructorModelDB FieldObject { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public Action<FieldFormBaseLowConstructorModel, Type> StateHasChangedHandler { get; set; } = default!;

    string? _fields_names;
    string? FieldsNames
    {
        get => _fields_names;
        set
        {
            _fields_names = value;
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, SelectedProgramCalculationField);
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, FieldsNames);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    readonly IEnumerable<EntryAltDescriptionModel> Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
    string? selected_program_calculation_field;
    /// <inheritdoc/>
    public string? SelectedProgramCalculationField
    {
        get => selected_program_calculation_field;
        private set
        {
            selected_program_calculation_field = value;
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, SelectedProgramCalculationField);
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, FieldsNames);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Entries.Any())
            SelectedProgramCalculationField = Entries.First().Id;
    }

    /// <inheritdoc/>
    public void Update(FieldFormBaseLowConstructorModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }
}