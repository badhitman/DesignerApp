using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class ProgrammCalcFieldFormUIComponent : ComponentBase
{
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [Parameter, EditorRequired]
    public ConstructorFieldFormModelDB FieldObject { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;

    string? _fields_names;
    string? FieldsNames
    {
        get => _fields_names;
        set
        {
            _fields_names = value;
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, SelectedProgrammCalcField);
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, FieldsNames);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    IEnumerable<EntryAltDescriptionModel> Entries = Enumerable.Empty<EntryAltDescriptionModel>();
    string? selected_programm_calc_field;
    public string? SelectedProgrammCalcField
    {
        get => selected_programm_calc_field;
        private set
        {
            selected_programm_calc_field = value;
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, SelectedProgrammCalcField);
            FieldObject.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, FieldsNames);
            StateHasChangedHandler(FieldObject, GetType());
        }
    }

    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();

        if (Entries.Any())
            SelectedProgrammCalcField = Entries.First().Id;
    }

    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        FieldObject.Update(field);
        StateHasChanged();
    }
}