﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsClient;

/// <summary>
/// Generator client view
/// </summary>
public partial class GeneratorClientViewComponent : FieldComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public FieldFormConstructorModelDB Field { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public bool ReadOnly { get; set; }

    IEnumerable<string> Elements = [];
    string? _selectedElement;
    /// <inheritdoc/>
    public string SelectedElement
    {
        get => _selectedElement ?? "";
        set
        {
            _selectedElement = value;
            InvokeAsync(async () => await SetValue(_selectedElement, Field.Name));
        }
    }

    protected private Task<IEnumerable<string>> SearchElements(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(Elements);
        return Task.FromResult(Elements.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }

    string? FieldValue => SessionDocument?.DataSessionValues?.FirstOrDefault(x => x.Name.Equals(Field.Name, StringComparison.OrdinalIgnoreCase) && x.RowNum == GroupByRowNum)?.Value;

    /// <inheritdoc/>
    public override string DomID => $"form-{Form.Id}_{Field.GetType().FullName}-{DocumentPage?.Id}-{Field.Id}";
    FieldValueGeneratorAbstraction? _gen = null;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _selectedElement = FieldValue;
        if (string.IsNullOrWhiteSpace(Field.MetadataValueType))
            return;

        _gen = DeclarationAbstraction.GetHandlerService(Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, "")!.ToString()!) as FieldValueGeneratorAbstraction;
        if (_gen is null)
        {
            SnackbarRepo.Add($"Параметры поля-генератора имеют не корректный формат. Не найден генератор.\n\n{Field.MetadataValueType}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (SessionDocument is not null)
        {
            TResponseModel<string[]> res_elements = _gen.GetListElements(Field, SessionDocument, PageJoinForm, GroupByRowNum);
            SnackbarRepo.ShowMessagesResponse(res_elements.Messages);
            if (res_elements.Success() && res_elements.Response is not null)
                Elements = res_elements.Response;
        }

        FieldsReferring?.Add(this);
    }
}