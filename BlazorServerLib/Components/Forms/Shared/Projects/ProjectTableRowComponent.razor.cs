////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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
    public required ProjectsListComponent ParentProjectsList { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstrucnorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    bool IsMyProject => CurrentUser.UserId.Equals(ProjectRow.OwnerUserId);

    /// <inheritdoc/>
    protected async Task EditProject()
    {
        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, ProjectRow },
             { x => x.ParentFormsPage, ParentFormsPage },
             { x => x.ParentListProjects, ParentProjectsList },
             { x => x.CurrentUser, CurrentUser },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraExtraLarge };
        IDialogReference res = await DialogService.ShowAsync<ProjectEditDialogComponent>("Редактирование проекта", parameters, options);
        await ParentProjectsList.ReloadListProjects();
    }

    /// <inheritdoc/>
    protected async Task DeleteProject()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.SetMarkerDeleteProject(ProjectRow.Id, !ProjectRow.IsDisabled);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await ParentProjectsList.ReloadListProjects();
        ParentProjectsList.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected async Task SetMainProjectHandle()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.SetProjectAsMain(ProjectRow.Id, CurrentUser.UserId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
    }
}
