////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Edit form dialog
/// </summary>
public partial class EditFormDialogComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;


    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;


    /// <inheritdoc/>
    protected FieldsFormViewComponent? _fields_view_ref;

    /// <inheritdoc/>
    protected bool IsEdited => !Form.Equals(FormEditObject) || FormEditObject.Id == 0;

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    /// <inheritdoc/>
    protected ConstructorFormModelDB FormEditObject = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = default!;

    /// <inheritdoc/>
    protected void Close() => MudDialog.Close(DialogResult.Ok(Form));

    void ResetForm()
    {
        FormEditObject = ConstructorFormModelDB.Build(Form);
        _currentTemplateInputRichText?.SetValue(FormEditObject.Description);
    }

    /// <inheritdoc/>
    protected async Task SaveForm()
    {
        IsBusyProgress = true;
        TResponseModel<ConstructorFormModelDB> rest = await FormsRepo.FormUpdateOrCreate(FormEditObject);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

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
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
        ResetForm();
    }
}