using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <summary>
/// Строка таблицы проектов
/// </summary>
public partial class ProjectTableRowComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IDialogService DialogService { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;


    /// <summary>
    /// Проект
    /// </summary>
    [Parameter, EditorRequired]
    public required ProjectViewModel ProjectRow { get; set; }

    /// <summary>
    /// Ссылка на 
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ProjectsListComponent ProjectsListComponentRef { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormsPage OwnerFormsPage { get; set; }

    /// <inheritdoc/>
    protected async Task EditProject()
    {
        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, ProjectRow },
             { x => x.ParentFormsPage, OwnerFormsPage },
             { x => x.ParentListProjects, ProjectsListComponentRef },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.Large };
        IDialogReference res = await DialogService.ShowAsync<ProjectEditDialogComponent>("Редактирование проекта", parameters, options);
        await ProjectsListComponentRef.ReloadListProjects();
    }

    /// <inheritdoc/>
    protected async Task DeleteProject()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.SetMarkerDeleteProject(ProjectRow.Id, !ProjectRow.IsDisabled);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await ProjectsListComponentRef.ReloadListProjects();
    }

    /// <inheritdoc/>
    protected async Task SetMainProjectHandle()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.SetProjectAsMain(ProjectRow.Id, OwnerFormsPage.CurrentUser.UserId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
        {
            await OwnerFormsPage.ReadCurrentMainProject();
        }
    }
}
