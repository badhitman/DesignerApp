////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using BlazorWebLib.Components.Constructor.Pages;
using BlazorLib.Components.Shared.tabs;

namespace BlazorWebLib.Components.Constructor.Shared.Form;

/// <summary>
/// Edit form dialog
/// </summary>
public partial class EditFormDialogComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;


    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public FormConstructorModelDB Form { get; set; } = default!;

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [Parameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    TabSetComponent tab_set_ref = default!;

    /// <inheritdoc/>
    protected FieldsFormViewComponent? _fields_view_ref;

    /// <inheritdoc/>
    protected bool IsEdited => !Form.Equals(FormEditObject) || FormEditObject.Id == 0;

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    /// <inheritdoc/>
    protected FormConstructorModelDB FormEditObject = default!;

    /// <inheritdoc/>
    protected void Close() => MudDialog.Close(DialogResult.Ok(Form));

    void ResetForm()
    {
        FormEditObject = FormConstructorModelDB.Build(Form);
        _currentTemplateInputRichText?.SetValue(FormEditObject.Description);
    }

    /// <inheritdoc/>
    protected async Task SaveForm()
    {
        IsBusyProgress = true;
        TResponseModel<FormConstructorModelDB> rest = await ConstructorRepo.FormUpdateOrCreate(FormEditObject);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }
        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка B64393D8-9C60-4A84-9790-15EFA6A74ABB rest content form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Form.Reload(rest.Response);
        ResetForm();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ResetForm();
    }
}