////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Table calculation kit
/// </summary>
public partial class TableCalculationKitComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IOptions<VirtualColumnCalculateGroupingTableModel[]> CalculationConfig { get; set; } = default!;
    /// <inheritdoc/>
    protected VirtualColumnCalculateGroupingTableModel[] _conf => CalculationConfig.Value;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public ConstructorFormSessionModelDB SessionQuestionnaire { get; set; } = default!;

    /// <inheritdoc/>
    protected IQueryable<ConstructorFieldFormModelDB>? QueryFieldsOfNumericTypes => PageJoinForm.Form?.QueryFieldsOfNumericTypes(SelectedFieldObject?.FieldName);

    const string _separator = ":";
    /// <inheritdoc/>
    protected string getFieldVal(ConstructorFieldFormBaseLowModel f) => $"{f.Id}{_separator}{f.GetType().Name}";
    SelectedFieldModel? GetFieldStruct(string? f)
    {
        if (string.IsNullOrWhiteSpace(f))
            return new();

        int i = f.IndexOf(_separator);
        if (i <= 0 || i == (f.Length - 1))
        {
            SnackbarRepo.Add($"В значении '{f}' не найден символ-сепаратор '{_separator}' (либо его позиция: крайняя). error A3D99C8E-2645-4148-A88A-95F8431F933D", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return null;
        }
        string id_str = f[..i];

        if (!int.TryParse(id_str, out int id_int) || id_int <= 0)
        {
            SnackbarRepo.Add($"Строка '{id_str}' не является [Int числом], либо его значение меньше нуля. error 2ADDB354-B526-43BF-86F5-1891FE950C02", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return null;
        }

        string type_str = f[(id_str.Length + 1)..];
        ConstructorFieldFormBaseLowModel? fb = PageJoinForm.Form?.AllFields.FirstOrDefault(x => x.Id == id_int && x.GetType().Name.Equals(type_str));

        if (fb is null)
        {
            SnackbarRepo.Add($"Поле [{f}] не найдено в форме. error 7BC10674-769F-4D9C-B99E-D235646A2E79", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return null;
        }
        return new() { FieldType = fb.GetType(), FieldId = fb.Id, FieldName = fb.Name };
    }

    FormTableCalculationManager? TableCalculation;

    SelectedFieldModel? SelectedFieldObject = null;
    /// <inheritdoc/>
    protected string? SelectedFieldValue
    {
        get
        {
            bool is_upd = false;
            if (SelectedFieldObject is null)
            {
                ConstructorFieldFormBaseLowModel? fb = PageJoinForm.Form?.AllFields.FirstOrDefault();
                if (fb is not null)
                {
                    is_upd = true;
                    SelectedFieldObject = new() { FieldType = fb.GetType(), FieldId = fb.Id, FieldName = fb.Name };
                }
            }
            if (is_upd)
                Update();

            if (SelectedFieldObject is null)
                return null;

            return $"{SelectedFieldObject.FieldId}{_separator}{SelectedFieldObject.FieldType.Name}";
        }
        set
        {
            if ($"{SelectedFieldObject?.FieldId}{_separator}{SelectedFieldObject?.FieldType.Name}".Equals(value) == true)
                return;

            SelectedFieldObject = GetFieldStruct(value);
            Update();
        }
    }

    IQueryable<ConstructorFieldFormModelDB>? GetQueryFieldsOfNumericTypesForFieldName(string? field_name) => PageJoinForm.Form?.QueryFieldsOfNumericTypes(field_name);
    
    /// <inheritdoc/>
    protected IEnumerable<string> FieldsNames(string? field_name) => GetQueryFieldsOfNumericTypesForFieldName(field_name)?.Select(x => x.Name) ?? Enumerable.Empty<string>();

    /// <inheritdoc/>
    public void Update()
    {
        if (SelectedFieldObject is null)
            return;
        if (TableCalculation is null)
            TableCalculation = new(SelectedFieldObject, PageJoinForm, SessionQuestionnaire);
        else
            TableCalculation.Update(SelectedFieldObject, PageJoinForm, SessionQuestionnaire);

        StateHasChanged();
    }
}