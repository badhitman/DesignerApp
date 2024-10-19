////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Projects;

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
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


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
    public required ConstructorPage ParentFormsPage { get; set; }


    UserInfoMainModel CurrentUser { get; set; } = default!;


    bool IsMyProject => CurrentUser.UserId.Equals(ProjectRow.OwnerUserId);

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        CurrentUser = state.User.ReadCurrentUserInfo() ?? throw new Exception();
        await base.OnInitializedAsync();
    }

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
        ResponseBaseModel res = await ConstructorRepo.SetMarkerDeleteProject(new() { ProjectId = ProjectRow.Id, Marker = !ProjectRow.IsDisabled });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await ParentProjectsList.ReloadListProjects();
        ParentProjectsList.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected async Task SetMainProjectHandle()
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await ConstructorRepo.SetProjectAsMain(new() { ProjectId = ProjectRow.Id, UserId = CurrentUser.UserId });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
    }
}