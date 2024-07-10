////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Shared.FieldsClient;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Constructor.Pages;

namespace BlazorWebLib.Components.Constructor.Shared.Form;

/// <summary>
/// Fields form view
/// </summary>
public partial class FieldsFormViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    /// <inheritdoc/>
    protected bool CanSave => !string.IsNullOrWhiteSpace(field_creating_field_ref?.FieldName);
    /// <inheritdoc/>
    protected AddingFieldFormViewComponent? field_creating_field_ref;

    /// <inheritdoc/>
    protected ClientStandardViewFormComponent? client_standard_ref;

    void ReloadFields(FormConstructorModelDB? form = null)
    {
        if (Form.Id < 1)
            return;
        TResponseModel<FormConstructorModelDB> rest;
        if (form is null)
        {
            IsBusyProgress = true;
            _ = InvokeAsync(async () =>
            {
                rest = await ConstructorRepo.GetForm(Form.Id);
                IsBusyProgress = false;
                SnackbarRepo.ShowMessagesResponse(rest.Messages);

                if (rest.Response is null)
                    SnackbarRepo.Add($"Ошибка 129E30BB-F331-4EA1-961C-33F52E13443F rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

                if (!rest.Success())
                {
                    SnackbarRepo.Add($"Ошибка 32E7BE10-7C10-4FC0-80A0-23CF9D176278 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    return;
                }

                if (rest.Response is null)
                    return;

                Form.Reload(rest.Response);
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

    FieldFormBaseLowConstructorModel? _field_master;

    /// <inheritdoc/>
    protected async Task CreateField()
    {
        if (_field_master is null)
        {
            SnackbarRepo.Add("child_field_form is null. error FEF46EC6-F26F-4FE2-B569-FFA5D8470171", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        ResponseBaseModel rest;
        IsBusyProgress = true;
        if (_field_master is LinkDirectoryToFormConstructorModelDB directory_field)
        {
            rest = await ConstructorRepo.FormFieldDirectoryUpdateOrCreate(new LinkDirectoryToFormConstructorModelDB()
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
        else if (_field_master is FieldFormConstructorModelDB standard_field)
        {
            standard_field.OwnerId = Form.Id;
            rest = await ConstructorRepo.FormFieldUpdateOrCreate(standard_field);
        }
        else
        {
            SnackbarRepo.Add("Тип поля не определён 050A59F3-028D-41C8-81AC-34F66EBF3840", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }

        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }

        ReloadFields();
        field_creating_field_ref?.SetTypeField();
        if (client_standard_ref is not null)
            await client_standard_ref.Update(Form);
    }

    /// <inheritdoc/>
    protected void AddingFieldStateHasChangedAction(FieldFormBaseLowConstructorModel _sender, Type initiator)
    {
        if (_field_master is null)
        {
            _field_master = _sender;
            field_creating_field_ref?.Update(_sender);
            StateHasChanged();
            return;
        }

        bool change_type = _field_master.GetType() != _sender.GetType();

        field_creating_field_ref?.Update(_sender);
        if (_sender is LinkDirectoryToFormConstructorModelDB directory_field)
        {
            if (initiator == typeof(AddingFieldFormViewComponent))
            {
                _field_master.Required = directory_field.Required;
                _field_master.Name = directory_field.Name;

                if (_field_master is LinkDirectoryToFormConstructorModelDB dl)
                    directory_field.DirectoryId = dl.DirectoryId;
            }
            else
            {
                directory_field.Required = _field_master.Required;
                directory_field.Name = _field_master.Name;
            }
            _field_master = directory_field;
        }
        else if (_sender is FieldFormConstructorModelDB standard_field)
        {

            if (initiator == typeof(AddingFieldFormViewComponent))
            {
                _field_master.Required = standard_field.Required;
                _field_master.Name = standard_field.Name;

                //if (_field_master is FieldFormConstructorModelDB sfl)
                //    standard_field.MetadataValueType = sfl.MetadataValueType;

                if (!change_type && _field_master is FieldFormConstructorModelDB sfl)
                    standard_field.MetadataValueType = sfl.MetadataValueType;
                else
                    standard_field.MetadataValueType = null;
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
            string msg = $"Тип поля не распознан: {_sender.GetType().FullName}";
            SnackbarRepo.Add($"{msg} 7AC47E91-6CA2-433F-A981-E1E585E04695", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            throw new Exception(msg);
        }

        StateHasChanged();
    }
}