////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;

namespace BlazorWebLib.Components;

/// <summary>
/// UserSelectInputComponent
/// </summary>
public partial class UserSelectInputComponent : LazySelectorComponent<UserInfoModel>
{
    [Inject]
    IWebRemoteTransmissionService WebRepo { get; set; } = default!;


    /// <summary>
    /// Selected chat
    /// </summary>
    [Parameter]
    public string? SelectedUser { get; set; }


    bool IsEditMode;

    /// <inheritdoc/>
    public override async Task LoadPartData()
    {
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<UserInfoModel>?> rest = await WebRepo
            .SelectUsersOfIdentity(new()
            {
                Payload = new() { SearchQuery = SelectedUser },
                PageNum = PageNum,
                PageSize = page_size,
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success() && rest.Response?.Response is not null)
        {
            TotalRowsCount = rest.Response.TotalRowsCount;
            LoadedData.AddRange(rest.Response.Response);

            if (PageNum == 0)
                LoadedData.Insert(0, new() { UserId = "", UserName = "Не выбран" });

            PageNum++;
        }
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedUser))
        {
            SelectedObject = new()
            {
                UserId = "",
                UserName = "Не выбрано",
            };
            SelectHandleAction(SelectedObject);
            return;
        }

        IsBusyProgress = true;
        TResponseModel<UserInfoModel[]?> rest = await WebRepo.GetUsersIdentity([SelectedUser]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null || rest.Response.Length == 0)
        {
            SnackbarRepo.Error($"Не найден запрашиваемый пользователь #{SelectedUser}");
            return;
        }
        SelectedObject = rest.Response.Single();
        _selectedValueText = SelectedObject.ToString();
        SelectHandleAction(SelectedObject);
    }
}