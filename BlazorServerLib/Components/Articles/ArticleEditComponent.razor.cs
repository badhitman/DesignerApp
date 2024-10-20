﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Articles;

/// <summary>
/// Редактирование статьи
/// </summary>
public partial class ArticleEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService artRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;

    [Inject]
    NavigationManager NavRepo { get; set; } = default!;


    /// <summary>
    /// Article Id
    /// </summary>
    [Parameter]
    public int ArticleId { get; set; }


    ArticleModelDB orignArticle = default!;
    ArticleModelDB editArticle = default!;

    bool IsEdited => !orignArticle.Equals(editArticle);

    UserInfoMainModel user = default!;
    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;


    async Task SaveArticle()
    {
        IsBusyProgress = true;
        TResponseModel<int?> res = await artRepo.ArticleCreateOrUpdate(editArticle);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (editArticle.Id < 1 && res.Response.HasValue && res.Response.Value > 0)
            NavRepo.NavigateTo($"/articles/edit-card/{res.Response}");
        else
            await LoadArticleData();
    }

    async Task LoadArticleData()
    {
        IsBusyProgress = true;
        TResponseModel<ArticleModelDB[]> res = await artRepo.ArticlesRead([ArticleId]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        if (res.Response is null)
            throw new Exception();

        orignArticle = res.Response.Single();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        images_upload_url = $"{GlobalStaticConstants.TinyMCEditorUploadImage}{GlobalStaticConstants.Routes.ARTICLE_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.BODY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={GlobalStaticConstants.Routes.IMAGE_ACTION_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={ArticleId}";
        editorConf = GlobalStaticConstants.TinyMCEditorConf(images_upload_url);

        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        if (ArticleId > 0)
            await LoadArticleData();
        else
            orignArticle = new()
            {
                AuthorIdentityId = user.UserId,
                Name = "",
            };

        editArticle = GlobalTools.CreateDeepCopy(orignArticle) ?? throw new Exception();
    }
}