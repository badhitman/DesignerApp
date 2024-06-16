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
    /// Текущий пользователь
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <summary>
    /// Ссылка на 
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ProjectsComponent FormsListComponentRef { get; set; }

    /// <summary>
    /// Основной/используемый проект
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ProjectViewModel MainProject { get; set; }

    /// <inheritdoc/>
    protected async Task EditProject()
    {
        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, ProjectRow },
             { x => x.CurrentUser, CurrentUser },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge };
        await DialogService.ShowAsync<ProjectEditDialogComponent>("Редактирование проекта", parameters, options);
        await FormsListComponentRef.ReloadListProjects();
    }

    /// <inheritdoc/>
    protected async Task DeleteProject()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.SetMarkerDeleteProject(ProjectRow.Id, !ProjectRow.IsDeleted);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await FormsListComponentRef.ReloadListProjects();
    }

    /// <inheritdoc/>
    protected async Task SetMainProjectHandle()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.SetProjectAsMain(ProjectRow.Id, CurrentUser.UserId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        //if (!res.Success())
        //    SetMainProjectAction(ProjectRow);
    }
}
