////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components;

/// <summary>
/// TinyMCEComponent
/// </summary>
public partial class TinyMCEComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    NavigationManager NavRepo { get; set; } = default!;


    /// <summary>
    /// KeyStorage
    /// </summary>
    [Parameter, EditorRequired]
    public required StorageMetadataModel KeyStorage { get; set; }


    string images_upload_url = default!;
    string? ScriptSrc = "/lib/tinymce/tinymce.min.js";

    Dictionary<string, object> editorConf = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        images_upload_url = $"/TinyMCEditor/UploadImage/{KeyStorage.ApplicationName}/{KeyStorage.Name}/{KeyStorage.PrefixPropertyName}/{KeyStorage.OwnerPrimaryKey}";

        editorConf = new()
        {
            { "menubar", true },
            { "plugins", "autolink media link image code emoticons table" },
            { "toolbar", "undo redo | styleselect styles | forecolor | bold italic underline | table | alignleft aligncenter alignright alignjustify | outdent indent | link image paste | code" },
            { "images_upload_credentials", true },
            { "paste_data_images", true },
            { "width", "100%" },
            { "automatic_uploads", true },
            { "file_picker_types", "file image media" },
            { "images_reuse_filename", true },
            { "images_upload_url", images_upload_url } };
    }
}