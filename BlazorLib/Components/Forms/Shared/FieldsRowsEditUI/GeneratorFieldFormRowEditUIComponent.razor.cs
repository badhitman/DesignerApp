using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

public partial class GeneratorFieldFormRowEditUIComponent : FieldFormEditFormBaseComponent
{
    [Inject]
    protected ILogger<GeneratorFieldFormRowEditUIComponent> _logger { get; set; } = default!;

    /// <summary>
    /// Параметры генератора
    /// </summary>
    public string? GeneratorFieldParameter
    {
        get
        {
            return Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "")?.ToString();
        }
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter);
            else
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, value);
            StateHasChangedHandler(Field);
        }
    }

    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");
    DeclarationAbstraction? _dc = null;
    IEnumerable<EntryAltDescriptionModel> Entries = Enumerable.Empty<EntryAltDescriptionModel>();
    string? GeneratorClass
    {
        get
        {
            return Field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, Entries.FirstOrDefault()?.Id)?.ToString();
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Field.UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor);
            }
            else
            {
                Field.SetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, value);
                _dc = DeclarationAbstraction.GetHandlerService(value);
                if (_dc is not null)
                {
                    if (_dc is NetworksAccessFieldValueGen na)
                        RequestModel = na.RequestModel;
                    else
                        RequestModel = new object();
                }
                else
                    snackbar.Add($"Тип данных не определён `{value}`. error {{85A5E044-20F1-4875-AA90-5430A4BE7BBF}}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            }

            StateHasChangedHandler(Field);
        }
    }

    protected object RequestModel = new();

    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<FieldValueGeneratorAbstraction>().ToArray();
        if (!string.IsNullOrWhiteSpace(GeneratorClass))
        {
            _dc = DeclarationAbstraction.GetHandlerService(GeneratorClass);
            if (_dc is not null)
            {
                if (_dc is NetworksAccessFieldValueGen na)
                    RequestModel = na.RequestModel;
            }
        }
    }
}