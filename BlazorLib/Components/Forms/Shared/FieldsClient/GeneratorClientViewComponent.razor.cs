using BlazorLib.BlazorComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Forms.FieldsClient;

public partial class GeneratorClientViewComponent : FieldComponentBaseModel
{
    [Inject]
    protected ILogger<GeneratorClientViewComponent> _logger { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFieldFormModelDB Field { get; set; } = default!;

    [Parameter, EditorRequired]
    public bool ReadOnly { get; set; }

    IEnumerable<string> Elements = Enumerable.Empty<string>();
    string? _selectedElement;
    public string SelectedElement
    {
        get => _selectedElement ?? "";
        set
        {
            _selectedElement = value;
            InvokeAsync(async () => await SetValue(_selectedElement, Field.Name));
        }
    }

    protected private async Task<IEnumerable<string>> Search1(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return Elements;
        return Elements.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    string? FieldValue => SessionQuestionnairie?.SessionValues?.FirstOrDefault(x => x.Name.Equals(Field.Name, StringComparison.OrdinalIgnoreCase) && x.GroupByRowNum == GroupByRowNum)?.Value;

    public override string DomID => $"form-{Form.Id}_{Field.GetType().FullName}-{QuestionnairePage?.Id}-{Field.Id}";
    FieldValueGeneratorAbstraction? _gen = null;
    protected override void OnInitialized()
    {
        _selectedElement = FieldValue;
        if (string.IsNullOrWhiteSpace(Field.MetadataValueType))
            return;

        _gen = DeclarationAbstraction.GetHandlerService(Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, "")!.ToString()!) as FieldValueGeneratorAbstraction;
        if (_gen is null)
        {
            snackbarInject.Add($"Параметры поля-генератора имеют не корректный формат. Не найден генератор.\n\n{Field.MetadataValueType}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (SessionQuestionnairie is not null)
        {
            SimpleStringArrayResponseModel res_elements = _gen.GetListElements(Field, SessionQuestionnairie, PageJoinForm, GroupByRowNum);
            snackbarInject.ShowMessagesResponse(res_elements.Messages);
            if (res_elements.Success && res_elements.Elements is not null)
                Elements = res_elements.Elements;
        }

        FieldsReferals?.Add(this);
    }
}