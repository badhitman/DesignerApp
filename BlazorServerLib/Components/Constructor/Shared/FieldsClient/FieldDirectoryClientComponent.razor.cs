////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsClient;

/// <summary>
/// Field directory client
/// </summary>
public partial class FieldDirectoryClientComponent : FieldComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FieldFormAkaDirectoryConstructorModelDB Field { get; set; }

    /// <summary>
    /// Объект справочника/списка (вместе с его элементами)
    /// </summary>
    [Parameter, EditorRequired]
    public required EntryNestedModel DirectoryObject { get; set; }

    EntryModel? _detect_value;
    EntryModel? detect_value
    {
        get => _detect_value;
        set
        {
            _detect_value = value;
        }
    }

    IEnumerable<EntryModel> _options = [];
    IEnumerable<EntryModel> options
    {
        get => _options;
        set
        {
            _options = value;
            InvokeAsync(async () =>
            {
                if (!_options.Any())
                    await SetValue(null, Field.Name);
                else
                    await SetValue($"[{string.Join(",", _options.Select(x => x.Id))}]", Field.Name);
            });
        }
    }

    Func<EntryModel?, string> converter = p => p?.Name ?? "";

    string? FieldValue => SessionDocument?.DataSessionValues?.FirstOrDefault(x => x.Name.Equals(Field.Name, StringComparison.OrdinalIgnoreCase) && x.TabJoinDocumentSchemeId == PageJoinForm?.Id && x.RowNum == GroupByRowNum)?.Value;

    /// <inheritdoc/>
    public override string DomID => $"form-{Form.Id}_{Field.GetType().FullName}-{DocumentPage?.Id}-{Field.Id}";

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        //options = DirectoryObject.Childs;

        if (!string.IsNullOrWhiteSpace(FieldValue))
        {
            int[] selectedIds = JsonConvert.DeserializeObject<int[]>(FieldValue) ?? [];
            if (selectedIds.Length != 0)
            {
                if (Field.IsMultiline)
                {
                    _options = DirectoryObject.Childs.Where(x => selectedIds.Contains(x.Id)).ToArray();
                }
                else
                {
                    detect_value = DirectoryObject.Childs.Any() ? DirectoryObject.Childs.FirstOrDefault(x => selectedIds.Contains(x.Id)) : null;

                    if (detect_value is null)
                        SnackbarRepo.Add($"{nameof(detect_value)} is null for '{FieldValue}'. error 2357552A-D878-4849-ADC5-98C070EC279F", MudBlazor.Severity.Error, c => c.DuplicatesBehavior = MudBlazor.SnackbarDuplicatesBehavior.Allow);
                }
            }
        }
        FieldsReferring?.Add(this);
    }
}