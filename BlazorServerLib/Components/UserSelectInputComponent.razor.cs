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


    /// <inheritdoc/>
    public override async Task LoadPartData()
    {
        await SetBusy();
        TPaginationResponseModel<UserInfoModel> rest = await WebRepo
            .SelectUsersOfIdentity(new()
            {
                Payload = new() { SearchQuery = _selectedValueText },
                PageNum = PageNum,
                PageSize = page_size,
            });
        IsBusyProgress = false;
        
        if (rest.Response is not null)
        {
            TotalRowsCount = rest.TotalRowsCount;
            LoadedData.AddRange(rest.Response);

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

        await SetBusy();
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