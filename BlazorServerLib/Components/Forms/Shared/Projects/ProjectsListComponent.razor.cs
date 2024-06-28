////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <inheritdoc/>
public partial class ProjectsListComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstrucnorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <summary>
    /// Проекты пользователя
    /// </summary>
    public ProjectViewModel[] ProjectsOfUser { get; private set; } = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadListProjects();
    }

    /// <summary>
    /// Загрузка перечня проектов
    /// </summary>
    public async Task ReloadListProjects()
    {
        IsBusyProgress = true;
        ProjectsOfUser = await FormsRepo.GetProjects(CurrentUser.UserId);
        IsBusyProgress = false;
    }

    /// <summary>
    /// Создать проект
    /// </summary>
    protected async Task CreateProject()
    {
        int i = 1;
        string name_new_project = $"Новый проект {i}", system_name_project = $"NewProject{i}";
        while (ProjectsOfUser.Any(x => x.Name.Equals(name_new_project, StringComparison.OrdinalIgnoreCase)) || ProjectsOfUser.Any(x => x.SystemName.Equals(system_name_project, StringComparison.OrdinalIgnoreCase)))
        {
            i++;
            name_new_project = $"Новый проект {i}";
            system_name_project = $"NewProject{i}";
        }

        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, new ProjectViewModel() { Name = name_new_project, SystemName = system_name_project, OwnerUserId = CurrentUser.UserId } },
             { x => x.ParentFormsPage, ParentFormsPage },
             { x => x.ParentListProjects, this },
             { x => x.CurrentUser, CurrentUser },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraExtraLarge };
        IDialogReference res = await DialogService.ShowAsync<ProjectEditDialogComponent>("Создание проекта", parameters, options);

        await ReloadListProjects();
    }
}