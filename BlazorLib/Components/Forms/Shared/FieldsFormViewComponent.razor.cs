using BlazorLib.Components.Forms.Shared.FieldsClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class FieldsFormViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }

    /// <inheritdoc/>
    protected bool CanSave => !string.IsNullOrWhiteSpace(field_creating_field_ref?.FieldName);
    /// <inheritdoc/>
    protected AddingFieldFormViewComponent? field_creating_field_ref;

    /// <inheritdoc/>
    protected ClientStandardViewFormComponent? client_standard_ref;

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
                rest = await FormsRepo.GetForm(Form.Id);
                IsBusyProgress = false;
                SnackbarRepo.ShowMessagesResponse(rest.Messages);

                if (!rest.Success())
                {
                    SnackbarRepo.Add($"Ошибка {{C166EB84-FEF0-4377-9765-C8B9EE1F1065}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    return;
                }

                if (rest.Form is null)
                {
                    SnackbarRepo.Add($"Ошибка {{0B2C8557-E22F-4A00-A7CA-7EC1FE445784}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    return;
                }
                Form.Reload(rest.Form);
                if (client_standard_ref is not null)
                    await client_standard_ref.Update(Form);
                StateHasChanged();
            });
        }
        else
        {
            Form.Reload(form);
            _ = InvokeAsync(async () =>
            {
                if (client_standard_ref is not null)
                    await client_standard_ref.Update(Form);

                StateHasChanged();
            });
        }
    }

    /// <inheritdoc/>
    protected async Task CreateField()
    {
        if (_field_master is null)
        {
            SnackbarRepo.Add("child_field_form is null. error {1234C070-6C85-48A3-903F-11A872EE67D1}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        ResponseBaseModel rest;
        IsBusyProgress = true;
        if (_field_master is ConstructorFormDirectoryLinkModelDB directory_field)
        {
            rest = await FormsRepo.FormFieldDirectoryUpdateOrCreate(new ConstructorFormDirectoryLinkModelDB()
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
        else if (_field_master is ConstructorFieldFormModelDB standard_field)
        {
            rest = await FormsRepo.FormFieldUpdateOrCreate(new ConstructorFieldFormBaseModel()
            {
                Description = standard_field.Description,
                Hint = standard_field.Hint,
                Id = standard_field.Id,
                MetadataValueType = standard_field.MetadataValueType,
                Name = standard_field.Name,
                OwnerId = Form.Id,
                Required = standard_field.Required,
                TypeField = standard_field.TypeField
            });
        }
        else
        {
            SnackbarRepo.Add("Тип поля не определён {C23F9885-8BF1-49C9-A4A8-2CBC4352A099}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }

        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{2B43DA23-8284-4A91-A868-4710A3EB780D}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        ReloadFields();
        field_creating_field_ref?.SetTypeField();
        if (client_standard_ref is not null)
            await client_standard_ref.Update(Form);
    }

    ConstructorFieldFormBaseLowModel _field_master = default!;

    /// <inheritdoc/>
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
        else if (_sender is ConstructorFieldFormModelDB standard_field)
        {
            if (initiator == typeof(AddingFieldFormViewComponent))
            {
                _field_master.Required = standard_field.Required;
                _field_master.Name = standard_field.Name;

                if (_field_master is ConstructorFieldFormModelDB sfl)
                    standard_field.MetadataValueType = sfl.MetadataValueType;
            }
            else
            {
                standard_field.Required = _field_master.Required;
                standard_field.Name = _field_master.Name;
            }
            _field_master = standard_field;
        }
        else
        {
            SnackbarRepo.Add("Тип поля не определён {2D298C68-338E-409F-8290-A01FECFDF91D}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            _field_master = _sender;
        }

        StateHasChanged();
    }
}