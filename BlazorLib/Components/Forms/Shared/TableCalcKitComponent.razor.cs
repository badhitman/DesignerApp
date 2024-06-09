﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

/// <summary>
/// Table calculation kit
/// </summary>
public partial class TableCalcKitComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public ConstructorFormSessionModelDB SessionQuestionnaire { get; set; } = default!;

    protected IQueryable<ConstructorFieldFormModelDB>? q => PageJoinForm.Form?.QueryFieldsOfNumericTypes(SelectedFieldObject?.FieldName);

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
            SnackbarRepo.Add($"В значении '{f}' не найден символ-сепаратор '{_separator}' (либо его позиция: крайняя). error {{296AF571-9F55-48A2-A3ED-3B6FD5938B30}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return null;
        }
        string id_str = f[..i];

        if (!int.TryParse(id_str, out int id_int) || id_int <= 0)
        {
            SnackbarRepo.Add($"Строка '{id_str}' не является [Int числом], либо его значение меньше нуля. error {{591E4EC7-7C67-495A-8D4B-20C6C7DBED2D}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return null;
        }

        string type_str = f[(id_str.Length + 1)..];
        ConstructorFieldFormBaseLowModel? fb = PageJoinForm.Form?.AllFields.FirstOrDefault(x => x.Id == id_int && x.GetType().Name.Equals(type_str));

        if (fb is null)
        {
            SnackbarRepo.Add($"Поле [{f}] не найдено в форме. error {{B749538D-C9FC-44DF-A1E4-F00C30B960DA}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return null;
        }
        return new() { FieldType = fb.GetType(), FieldId = fb.Id, FieldName = fb.Name };
    }

    FormTableCalcManager? TableCalc;

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

    IQueryable<ConstructorFieldFormModelDB>? Query(string? field_name) => PageJoinForm?.Form?.QueryFieldsOfNumericTypes(field_name);
    /// <inheritdoc/>
    protected IEnumerable<string> FieldsNames(string? field_name) => Query(field_name)?.Select(x => x.Name) ?? Enumerable.Empty<string>();

    /// <inheritdoc/>
    public void Update()
    {
        if (SelectedFieldObject is null)
            return;
        if (TableCalc is null)
            TableCalc = new(SelectedFieldObject, PageJoinForm, SessionQuestionnaire);
        else
            TableCalc.Update(SelectedFieldObject, PageJoinForm, SessionQuestionnaire);

        StateHasChanged();
    }
}