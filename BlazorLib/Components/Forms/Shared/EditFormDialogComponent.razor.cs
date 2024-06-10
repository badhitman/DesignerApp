using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class EditFormDialogComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;

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
    protected bool IsEdited => Form.Name != FormNameOrigin || Form.Description != FormDescriptionOrigin || Form.Css != FormCssOrigin || Form.AddRowButtonTitle != AddRowButtonTitleOrigin;

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    /// <inheritdoc/>
    protected string FormNameOrigin { get; set; } = "";
    /// <inheritdoc/>
    protected string? FormDescriptionOrigin { get; set; }
    /// <inheritdoc/>
    protected string? FormCssOrigin { get; set; }
    /// <inheritdoc/>
    protected string? AddRowButtonTitleOrigin { get; set; }

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = default!;

    /// <inheritdoc/>
    protected void Close() => MudDialog.Close(DialogResult.Ok(Form));

    async Task ResetForm()
    {
        FormNameOrigin = Form.Name;
        FormDescriptionOrigin = Form.Description;
        FormCssOrigin = Form.Css;
        AddRowButtonTitleOrigin = Form.AddRowButtonTitle;
        if (_currentTemplateInputRichText is not null)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, FormDescriptionOrigin);
    }

    /// <inheritdoc/>
    protected async Task SaveForm()
    {
        IsBusyProgress = true;
        FormResponseModel rest = await FormsRepo.FormUpdateOrCreate(new ConstructorFormBaseModel() { Id = Form.Id, Name = FormNameOrigin, Css = FormCssOrigin, Description = FormDescriptionOrigin, AddRowButtonTitle = AddRowButtonTitleOrigin });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (rest.Form is null)
        {
            SnackbarRepo.Add($"Ошибка {{266FF830-05BC-442A-B277-16FAEACB0F4A}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Form.Reload(rest.Form);
        await ResetForm();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
        await ResetForm();
        base.OnInitialized();
    }
}