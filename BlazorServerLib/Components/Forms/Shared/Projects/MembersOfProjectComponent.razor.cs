using BlazorLib;
using Microsoft.AspNetCore.Components;

namespace BlazorWebLib.Components.Forms.Shared.Projects;

/// <summary>
/// MembersOfProject
/// </summary>
public partial class MembersOfProjectComponent : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Project Id
    /// </summary>
    [Parameter, EditorRequired]
    public required int ProjectId { get; set; }
}