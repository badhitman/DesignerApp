using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;
using System.Net.Mail;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <summary>
/// MembersOfProject
/// </summary>
public partial class MembersOfProjectComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    [Inject]
    IUsersProfilesService UserProfilesManage { get; set; } = default!;

    /// <summary>
    /// Project Id
    /// </summary>
    [Parameter, EditorRequired]
    public required ProjectViewModel ProjectView { get; set; }

    string? emailForAddMember;

    /// <inheritdoc/>
    public async Task AddMember()
    {
        if (!MailAddress.TryCreate(emailForAddMember, out _))
            throw new Exception($"Email не корректный '{emailForAddMember}'");

        IsBusyProgress = true;
        UserInfoModel? user_info = await UserProfilesManage.FindByEmailAsync(emailForAddMember);

        if (user_info is null)
            SnackbarRepo.Add($"Пользователь с Email '{emailForAddMember}' не найден", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
        else
        {
            ResponseBaseModel adding_member = await FormsRepo.AddMemberToProject(ProjectView.Id, user_info.UserId);
            SnackbarRepo.ShowMessagesResponse(adding_member.Messages);
        }
        IsBusyProgress = false;

        ProjectView.Members = new(await FormsRepo.GetMembersOfProject(ProjectView.Id));
    }

    /// <inheritdoc/>
    public async Task RemoveMember(string user_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel res = await FormsRepo.DeleteMemberFromProject(ProjectView.Id, user_id);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        ProjectView.Members = new(await FormsRepo.GetMembersOfProject(ProjectView.Id));
    }
}