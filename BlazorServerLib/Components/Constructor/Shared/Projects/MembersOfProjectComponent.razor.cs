////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;
using System.Net.Mail;

namespace BlazorWebLib.Components.Constructor.Shared.Projects;

/// <summary>
/// MembersOfProject
/// </summary>
public partial class MembersOfProjectComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UserProfilesManage { get; set; } = default!;


    /// <summary>
    /// Project Id
    /// </summary>
    [Parameter, EditorRequired]
    public required ProjectViewModel ProjectView { get; set; }


    /// <summary>
    /// Ссылка на 
    /// </summary>
    [Parameter, EditorRequired]
    public required ProjectsListComponent ProjectsList { get; set; }


    string? emailForAddMember;

    /// <inheritdoc/>
    public async Task AddMember()
    {
        if (!MailAddress.TryCreate(emailForAddMember, out _))
            throw new Exception($"Email не корректный '{emailForAddMember}'");

        await SetBusy();
        UserInfoModel? user_info = await UserProfilesManage.FindByEmailAsync(emailForAddMember);

        if (user_info is null)
            SnackbarRepo.Add($"Пользователь с Email '{emailForAddMember}' не найден", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
        else
        {
            ResponseBaseModel adding_member = await ConstructorRepo.AddMembersToProject(new() { ProjectId = ProjectView.Id, UsersIds = [user_info.UserId] });
            SnackbarRepo.ShowMessagesResponse(adding_member.Messages);
        }
        IsBusyProgress = false;
        emailForAddMember = null;
        TResponseModel<EntryAltModel[]> members_rest = await ConstructorRepo.GetMembersOfProject(ProjectView.Id);
        ProjectView.Members = new(members_rest.Response ?? throw new Exception());

        await ProjectsList.ReloadListProjects();
        ProjectsList.StateHasChangedCall();
    }

    async Task Closed(MudChip<EntryAltModel> chip)
    {
        if (chip.Value is null)
            throw new Exception();

        await SetBusy();
        ResponseBaseModel res = await ConstructorRepo.DeleteMembersFromProject(new() { ProjectId = ProjectView.Id, UsersIds = [chip.Value.Id] });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        TResponseModel<EntryAltModel[]> rest_members = await ConstructorRepo.GetMembersOfProject(ProjectView.Id);
        ProjectView.Members = new(rest_members.Response ?? throw new Exception());

        await ProjectsList.ReloadListProjects();
        ProjectsList.StateHasChangedCall();
    }
}