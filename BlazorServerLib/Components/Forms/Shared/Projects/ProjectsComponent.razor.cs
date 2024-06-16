using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <inheritdoc/>
public partial class ProjectsComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;

    /// <summary>
    /// Текущий пользователь
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }

    /// <summary>
    /// Проекты пользователя
    /// </summary>
    public ProjectViewModel[] ProjectsOfUser { get; protected set; } = default!;

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

    void CreateProject()
    {
        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, new ProjectViewModel() { Name = "Новый проект", SystemName = "NewProject" } },
             { x => x.CurrentUser, CurrentUser },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraLarge };
        DialogService.Show<ProjectEditDialogComponent>("Создание проекта", parameters, options);
    }
}