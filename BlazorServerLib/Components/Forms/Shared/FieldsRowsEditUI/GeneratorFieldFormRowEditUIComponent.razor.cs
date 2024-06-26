////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsRowsEditUI;

/// <summary>
/// Generator field form row edit UI
/// </summary>
public partial class GeneratorFieldFormRowEditUIComponent : FieldFormEditFormBaseComponent
{
    /// <summary>
    /// Параметры генератора
    /// </summary>
    public string? GeneratorFieldParameter
    {
        get
        {
            return Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter, "")?.ToString();
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

    /// <inheritdoc/>
    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");

    DeclarationAbstraction? _dc = null;
    IEnumerable<EntryAltDescriptionModel> Entries = Enumerable.Empty<EntryAltDescriptionModel>();

    string? GeneratorClass
    {
        get
        {
            return Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, Entries.FirstOrDefault()?.Id)?.ToString();
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
                    //if (_dc is NetvorksAccessFieldValueGen na)
                    //    RequestModel = na.RequestModel;
                    //else
                        RequestModel = new object();
                }
                else
                    SnackbarRepo.Add($"Тип данных не определён `{value}`. error BD37DC45-22F0-4D78-B879-0897611F681A", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            }

            StateHasChangedHandler(Field);
        }
    }

    /// <inheritdoc/>
    protected object RequestModel = new();

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<FieldValueGeneratorAbstraction>().ToArray();
        if (!string.IsNullOrWhiteSpace(GeneratorClass))
        {
            _dc = DeclarationAbstraction.GetHandlerService(GeneratorClass);
            if (_dc is not null)
            {
                //if (_dc is NetworksAccessFieldValueGen na)
                //    RequestModel = na.RequestModel;
            }
        }
    }
}