using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory Navigation
/// </summary>
public partial class DirectoryNavComponent : ComponentBase
{
    /// <summary>
    /// Create directory handler
    /// </summary>
    [Parameter, EditorRequired]
    public Action<string> CreateDirectoryHandler { get; set; } = default!;

    /// <summary>
    /// Delete selected directory handler
    /// </summary>
    [Parameter, EditorRequired]
    public Action DeleteSelectedDirectoryHandler { get; set; } = default!;

    /// <summary>
    /// Rename selected directory handler
    /// </summary>
    [Parameter, EditorRequired]
    public Action<bool> RenameSelectedDirectoryHandler { get; set; } = default!;

    /// <summary>
    /// Save rename selected directory handler
    /// </summary>
    [Parameter, EditorRequired]
    public Action SaveRenameSelectedDirectoryHandler { get; set; } = default!;

    /// <summary>
    /// Can delete current directory
    /// </summary>
    [Parameter, EditorRequired]
    public bool CanDeleteCurrentDirectory { get; set; }

    /// <summary>
    /// Имя создаваемого словаря
    /// </summary>
    protected string? NameNewDict;

    /// <summary>
    /// Directory navigation state
    /// </summary>
    protected DirectoryNavStatesEnum DirectoryNavState = DirectoryNavStatesEnum.None;

    /// <summary>
    /// State has changed action
    /// </summary>
    public void StateHasChangedAction(bool? can_delete_current_directory = null)
    {
        if (can_delete_current_directory.HasValue)
            CanDeleteCurrentDirectory = can_delete_current_directory.Value;

        StateHasChanged();
    }

    /// <summary>
    /// Установить состояние навигации по каталогу
    /// </summary>
    public void SetDirectoryNavState(DirectoryNavStatesEnum dir_NavState)
    {
        DirectoryNavState = dir_NavState;
        StateHasChanged();
    }
}