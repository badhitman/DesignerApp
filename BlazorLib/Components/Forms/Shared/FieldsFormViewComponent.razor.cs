using BlazorLib.BlazorComponents;
using BlazorLib.Forms.FieldsClient;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class FieldsFormViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<FieldsFormViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    protected bool CanSave => !string.IsNullOrWhiteSpace(field_creating_field_ref?.FieldName);
    protected AddingFieldFormViewComponent? field_creating_field_ref;

    protected ClientStandardViewFormComponent? client_standart_ref;
    void ReloadFields(ConstructorFormModelDB? form = null)
    {
        if (Form.Id < 1)
            return;
        FormResponseModel rest;
        if (form is null)
        {
            IsBusyProgress = true;
            _ = InvokeAsync(async () =>
            {
                rest = await _forms.GetForm(Form.Id);
                IsBusyProgress = false;
                _snackbar.ShowMessagesResponse(rest.Messages);

                if (!rest.Success())
                {
                    _snackbar.Add($"Ошибка {{C166EB84-FEF0-4377-9765-C8B9EE1F1065}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    return;
                }

                if (rest.Form is null)
                {
                    _snackbar.Add($"Ошибка {{0B2C8557-E22F-4A00-A7CA-7EC1FE445784}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    return;
                }
                Form.Reload(rest.Form);
                if (client_standart_ref is not null)
                    await client_standart_ref.Update(Form);
                StateHasChanged();
            });
        }
        else
        {
            Form.Reload(form);
            _ = InvokeAsync(async () =>
            {
                if (client_standart_ref is not null)
                    await client_standart_ref.Update(Form);

                StateHasChanged();
            });
        }
    }

    protected async Task CreateField()
    {
        if (_field_master is null)
        {
            _snackbar.Add("child_field_form is null. error {1234C070-6C85-48A3-903F-11A872EE67D1}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        ResponseBaseModel rest;
        IsBusyProgress = true;
        if (_field_master is ConstructorFormDirectoryLinkModelDB directory_field)
        {
            rest = await _forms.FormFieldDirectoryUpdateOrCreate(new ConstructorFormDirectoryLinkModelDB()
            {
                Description = directory_field.Description,
                DirectoryId = directory_field.DirectoryId,
                Hint = directory_field.Hint,
                Name = directory_field.Name,
                Required = directory_field.Required,
                OwnerId = Form.Id,
                Id = directory_field.Id
            });
        }
        else if (_field_master is ConstructorFieldFormModelDB standart_field)
        {
            rest = await _forms.FormFieldUpdateOrCreate(new ConstructorFieldFormBaseModel()
            {
                Description = standart_field.Description,
                Hint = standart_field.Hint,
                Id = standart_field.Id,
                MetadataValueType = standart_field.MetadataValueType,
                Name = standart_field.Name,
                OwnerId = Form.Id,
                Required = standart_field.Required,
                TypeField = standart_field.TypeField
            });
        }
        else
        {
            _snackbar.Add("Тип поля не определён {C23F9885-8BF1-49C9-A4A8-2CBC4352A099}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }

        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{2B43DA23-8284-4A91-A868-4710A3EB780D}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        ReloadFields();
        field_creating_field_ref?.SetTypeField();
        if (client_standart_ref is not null)
            await client_standart_ref.Update(Form);
    }

    ConstructorFieldFormBaseLowModel _field_master = default!;

    protected void AddingFieldStateHasChangedAction(ConstructorFieldFormBaseLowModel _sender, Type initiator)
    {
        field_creating_field_ref?.Update(_sender);
        if (_sender is ConstructorFormDirectoryLinkModelDB directory_field)
        {
            if (initiator == typeof(AddingFieldFormViewComponent))
            {
                _field_master.Required = directory_field.Required;
                _field_master.Name = directory_field.Name;

                if (_field_master is ConstructorFormDirectoryLinkModelDB dl)
                    directory_field.DirectoryId = dl.DirectoryId;
            }
            else
            {
                directory_field.Required = _field_master.Required;
                directory_field.Name = _field_master.Name;
            }
            _field_master = directory_field;
        }
        else if (_sender is ConstructorFieldFormModelDB standart_field)
        {
            if (initiator == typeof(AddingFieldFormViewComponent))
            {
                _field_master.Required = standart_field.Required;
                _field_master.Name = standart_field.Name;

                if (_field_master is ConstructorFieldFormModelDB sfl)
                    standart_field.MetadataValueType = sfl.MetadataValueType;
            }
            else
            {
                standart_field.Required = _field_master.Required;
                standart_field.Name = _field_master.Name;
            }
            _field_master = standart_field;
        }
        else
        {
            _snackbar.Add("Тип поля не определён {2D298C68-338E-409F-8290-A01FECFDF91D}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            _field_master = _sender;
        }

        StateHasChanged();
    }
}