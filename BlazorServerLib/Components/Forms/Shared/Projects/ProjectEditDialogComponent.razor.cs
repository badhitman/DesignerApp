using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <summary>
/// ProjectEditDialog
/// </summary>
public partial class ProjectEditDialogComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter]
    public required MudDialogInstance MudDialog { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ProjectViewModel ProjectForEdit { get; set; }
    ProjectViewModel ProjectEditObject = default!;

    /// <inheritdoc/>
    protected bool CanSave => !string.IsNullOrWhiteSpace(ProjectEditObject.Name) && !string.IsNullOrWhiteSpace(ProjectEditObject.SystemName) && (!ProjectForEdit.Equals(ProjectEditObject) || ProjectForEdit.Id < 1);

    async Task ResetForm()
    {
        ProjectEditObject = ProjectViewModel.Build(ProjectForEdit);
        if (_currentTemplateInputRichText is not null)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, ProjectEditObject.Description);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetForm();
    }

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    /// <inheritdoc/>
    protected async Task Submit()
    {
        IsBusyProgress = true;
        TResponseModel<int> res = await FormsRepo.CreateProject(ProjectForEdit, CurrentUser.UserId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
        {
            ProjectForEdit.Id = res.Response;
            ProjectEditObject.Reload(ProjectForEdit);
        }
        // MudDialog.Close(DialogResult.Ok(true));
    }

    /// <inheritdoc/>
    protected void Cancel() => MudDialog.Cancel();
}