using BlazorLib.Components.Forms.Shared.FieldsClient;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Forms.FieldsClient;

/// <summary>
/// Field directory client
/// </summary>
public partial class FieldDirectoryClientComponent : FieldComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public ConstructorFormDirectoryLinkModelDB Field { get; set; } = default!;

    /// <summary>
    /// Объект справочника/списка (вместе с его элементами)
    /// </summary>
    [Parameter, EditorRequired]
    public EntryNestedModel? DirectoryObject { get; set; }

    int _selectedElement;
    /// <inheritdoc/>
    protected int SelectedElement
    {
        get => _selectedElement;
        set
        {
            _selectedElement = value;
            InvokeAsync(async () => await SetValue(DirectoryObject?.Childs.FirstOrDefault(x => x.Id == _selectedElement)?.Name ?? "error {5DEEC1C9-2648-472B-BFAF-AF305336CFC4}", Field.Name));
        }
    }

    string? FieldValue => SessionQuestionnairie?.SessionValues?.FirstOrDefault(x => x.Name.Equals(Field.Name, StringComparison.OrdinalIgnoreCase) && x.QuestionnairePageJoinFormId == PageJoinForm?.Id && x.GroupByRowNum == GroupByRowNum)?.Value;
    
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
                snackbarInject.Add($"{nameof(detect_value)} is null for '{FieldValue}'. error {{E43F1279-7687-434D-AD94-9D554D6AD669}}", MudBlazor.Severity.Error, c => c.DuplicatesBehavior = MudBlazor.SnackbarDuplicatesBehavior.Allow);
            else
                _selectedElement = detect_value.Id;
        }
        FieldsReferals?.Add(this);
    }
}