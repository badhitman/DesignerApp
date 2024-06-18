using BlazorWebLib.Components.Forms.Pages;
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
    public required ProjectViewModel ProjectForEdit { get; set; }
    ProjectViewModel projectObject = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ProjectsListComponent ParentListProjects { get; set; }


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }


    /// <inheritdoc/>
    protected bool CanSave => !string.IsNullOrWhiteSpace(projectObject.Name) && !string.IsNullOrWhiteSpace(projectObject.SystemName) && (!ProjectForEdit.Equals(projectObject) || ProjectForEdit.Id < 1);

    async Task ResetForm()
    {
        if (projectObject is null)
            projectObject = ProjectViewModel.Build(ProjectForEdit);
        else
            projectObject.Reload(ProjectForEdit);

        if (_currentTemplateInputRichText is not null)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText.UID, projectObject.Description);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ResetForm();
    }

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    /// <inheritdoc/>
    protected async Task SaveProject()
    {
        IsBusyProgress = true;
        if (projectObject.Id < 1)
        {
            TResponseModel<int> res = await FormsRepo.CreateProject(projectObject, ParentFormsPage.CurrentUser.UserId);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            if (res.Success())
            {
                ProjectForEdit.Id = res.Response;
                projectObject.Reload(ProjectForEdit);
                await ParentListProjects.ReloadListProjects();
                ParentListProjects.StateHasChangedCall();
            }
        }
        else
        {
            ResponseBaseModel res = await FormsRepo.UpdateProject(projectObject);
            IsBusyProgress = false;
            SnackbarRepo.ShowMessagesResponse(res.Messages);
            if (res.Success())
            {
                ProjectForEdit.Reload(projectObject);
                await ParentListProjects.ReloadListProjects();
                ParentListProjects.StateHasChangedCall();
            }
        }
        // MudDialog.Close(DialogResult.Ok(true));
    }

    /// <inheritdoc/>
    protected void Cancel() => MudDialog.Cancel();
}