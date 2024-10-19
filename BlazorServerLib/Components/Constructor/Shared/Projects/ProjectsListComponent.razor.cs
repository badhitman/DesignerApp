////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebLib.Components.Constructor.Shared.Projects;

/// <inheritdoc/>
public partial class ProjectsListComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;

    [Inject]
    IDialogService DialogService { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    UserInfoMainModel CurrentUser { get; set; } = default!;


    /// <summary>
    /// Проекты пользователя
    /// </summary>
    public ProjectViewModel[] ProjectsOfUser { get; private set; } = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        await ReloadListProjects();
    }

    /// <summary>
    /// Загрузка перечня проектов
    /// </summary>
    public async Task ReloadListProjects()
    {
        IsBusyProgress = true;
        await Task.Delay(1);
        TResponseModel<ProjectViewModel[]> res_pr = await ConstructorRepo.GetProjectsForUser(new() { UserId = CurrentUser.UserId });
        ProjectsOfUser = res_pr.Response ?? throw new Exception();
        IsBusyProgress = false;
    }

    /// <summary>
    /// Создать проект
    /// </summary>
    protected async Task CreateProject()
    {
        int i = 1;
        string name_new_project = $"Новый проект {i}";
        while (ProjectsOfUser.Any(x => x.Name.Equals(name_new_project, StringComparison.OrdinalIgnoreCase)))
        {
            i++;
            name_new_project = $"Новый проект {i}";
        }

        DialogParameters<ProjectEditDialogComponent> parameters = new()
        {
             { x => x.ProjectForEdit, new ProjectViewModel() { Name = name_new_project, OwnerUserId = CurrentUser.UserId } },
             { x => x.ParentFormsPage, ParentFormsPage },
             { x => x.ParentListProjects, this },
             { x => x.CurrentUser, CurrentUser },
        };
        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraExtraLarge };
        IDialogReference res = await DialogService.ShowAsync<ProjectEditDialogComponent>("Создание проекта", parameters, options);

        await ReloadListProjects();
    }
}