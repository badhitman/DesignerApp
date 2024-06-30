////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Projects;

/// <summary>
/// ProjectEditDialog
/// </summary>
public partial class ProjectEditDialogComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;

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
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <inheritdoc/>
    protected bool CanSave => !string.IsNullOrWhiteSpace(projectObject.Name) && !string.IsNullOrWhiteSpace(projectObject.SystemName) && (!ProjectForEdit.Equals(projectObject) || ProjectForEdit.Id < 1);

    void ResetForm()
    {
        if (projectObject is null)
            projectObject = ProjectViewModel.Build(ProjectForEdit);
        else
            projectObject.Reload(ProjectForEdit);

        _currentTemplateInputRichText?.SetValue(projectObject.Description);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ResetForm();
    }

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText;

    /// <inheritdoc/>
    protected async Task SaveProject()
    {
        IsBusyProgress = true;
        if (projectObject.Id < 1)
        {
            TResponseModel<int> res = await ConstructorRepo.CreateProject(projectObject, CurrentUser.UserId);
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
            ResponseBaseModel res = await ConstructorRepo.UpdateProject(projectObject);
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