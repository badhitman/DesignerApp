////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Articles;

public partial class ArticleEditComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// ArticleId
    /// </summary>
    [Parameter]
    public int ArticleId { get; set; }


    string images_upload_url = default!;
    Dictionary<string, object> editorConf = default!;

    string? _textValue;
    string? TextValue
    {
        get => _textValue;
        set
        {
            bool nu = _textValue != value;
            _textValue = value;
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        images_upload_url = $"{GlobalStaticConstants.TinyMCEditorUploadImage}{GlobalStaticConstants.Routes.ARTICLE_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.BODY_CONTROLLER_NAME}?{nameof(StorageMetadataModel.PrefixPropertyName)}={GlobalStaticConstants.Routes.IMAGE_ACTION_NAME}&{nameof(StorageMetadataModel.OwnerPrimaryKey)}={ArticleId}";
        editorConf = GlobalStaticConstants.TinyMCEditorConf(images_upload_url);
    }
}