////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsClient;

/// <summary>
/// Field directory client
/// </summary>
public partial class FieldDirectoryClientComponent : FieldComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required LinkDirectoryToFormConstructorModelDB Field { get; set; }

    /// <summary>
    /// Объект справочника/списка (вместе с его элементами)
    /// </summary>
    [Parameter, EditorRequired]
    public required EntryNestedModel DirectoryObject { get; set; }

    int _selectedElement;
    /// <inheritdoc/>
    protected int SelectedElement
    {
        get => _selectedElement;
        set
        {
            _selectedElement = value;
            InvokeAsync(async () =>
            {
                if (_selectedElement == 0)
                    await SetValue(null, Field.Name);
                else
                {
                    string? _set_val = DirectoryObject?.Childs.FirstOrDefault(x => x.Id == _selectedElement)?.Name;
                    await SetValue(_set_val ?? "error {D21CC2F7-0B44-4BB3-A755-5A9C598D6E15}", Field.Name);
                }
            });
        }
    }

    string? FieldValue => SessionQuestionnaire?.DataSessionValues?.FirstOrDefault(x => x.Name.Equals(Field.Name, StringComparison.OrdinalIgnoreCase) && x.TabJoinDocumentSchemeId == PageJoinForm?.Id && x.RowNum == GroupByRowNum)?.Value;

    /// <inheritdoc/>
    public override string DomID => $"form-{Form.Id}_{Field.GetType().FullName}-{QuestionnairePage?.Id}-{Field.Id}";
    EntryModel? detect_value = null;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (!string.IsNullOrWhiteSpace(FieldValue))
        {
            detect_value = DirectoryObject?.Childs.Any() == true ? DirectoryObject.Childs.FirstOrDefault(x => x.Name.Equals(FieldValue, StringComparison.OrdinalIgnoreCase)) : null;

            if (detect_value is null)
                SnackbarRepo.Add($"{nameof(detect_value)} is null for '{FieldValue}'. error 2357552A-D878-4849-ADC5-98C070EC279F", MudBlazor.Severity.Error, c => c.DuplicatesBehavior = MudBlazor.SnackbarDuplicatesBehavior.Allow);
            else
                _selectedElement = detect_value.Id;
        }
        FieldsReferring?.Add(this);
    }
}