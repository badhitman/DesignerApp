////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// RubricNodeComponent
/// </summary>
public partial class RubricNodeComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required TreeItemData<RubricIssueHelpdeskBaseModelDB> Item { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required RubricsManageComponent HelpdeskParentView { get; set; }



    string Information
    {
        get
        {
            return "fix this";
            //if (!string.IsNullOrEmpty(itemSystemName))
            //{
            //    string? cn = HelpdeskComponent.CheckName(itemSystemName);
            //    if (!string.IsNullOrEmpty(cn))
            //        return cn;
            //}

            //return string.IsNullOrEmpty(ItemModel.Information) ? "" : ItemModel.Information;
        }
    }
    TreeItemDataRubricModel ItemModel = default!;

    string? itemSystemName;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value?.Id}";

    bool IsEdit => false;

    /// <inheritdoc/>
    protected MarkupString InformationMS => (MarkupString)Information;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = (TreeItemDataRubricModel)Item;
        //ArgumentNullException.ThrowIfNull(Item.Value?.Tag);

        //itemSystemName = ItemModel.SystemName;
    }
}