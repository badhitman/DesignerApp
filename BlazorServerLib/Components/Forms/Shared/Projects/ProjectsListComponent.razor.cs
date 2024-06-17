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
    public required FormsPage OwnerFormsPage { get; set; }

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
        ProjectsOfUser = await FormsRepo.GetProjects(OwnerFormsPage.CurrentUser.UserId);
        IsBusyProgress = false;
    }

    async Task CreateProject()
    {
        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, new ProjectViewModel() { Name = "Новый проект", SystemName = "NewProject" } },
             { x => x.ParentFormsPage, OwnerFormsPage },
             { x => x.ParentListProjects, this },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraExtraLarge };
        IDialogReference res = await DialogService.ShowAsync<ProjectEditDialogComponent>("Создание проекта", parameters, options);

        await ReloadListProjects();
    }
}