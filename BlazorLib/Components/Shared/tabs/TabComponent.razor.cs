﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SharedLib;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// Tab component
/// </summary>
public partial class TabComponent : BlazorBusyComponentBaseModel, ITab
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required TabSetComponent ContainerTabSet { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required string SystemName { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required string Title { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Tooltip { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool IsDisabled { get; set; } = false;

    /// <inheritdoc/>
    [Parameter]
    public Action<TabComponent>? OnClickHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    /// <summary>
    /// HoldTab
    /// </summary>
    public bool HoldTab { get; set; }

    private string? TitleCssClass =>
        ContainerTabSet?.ActiveTab == this ? "active" : IsDisabled ? "disabled" : null;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ContainerTabSet?.AddTab(this);
    }

    void ActivateTabHandler(MouseEventArgs args)
    {
        HoldTab = false;
        if (OnClickHandle is not null)
            OnClickHandle(this);

        if (!HoldTab)
            ActivateTab();
    }

    /// <inheritdoc/>
    public void ActivateTab()
    {
        ContainerTabSet.SetActiveTab(this, true);
        if (!ContainerTabSet.IsSilent)
        {
            Uri _u = new(NavigationManager.Uri);
            _u = new(_u.AppendQueryParameter(ExtBlazor.ActiveTabName, SystemName));
            NavigationManager.NavigateTo(_u.ToString());
        }
    }
}