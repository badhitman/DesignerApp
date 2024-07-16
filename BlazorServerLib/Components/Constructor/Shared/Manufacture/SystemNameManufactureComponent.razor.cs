﻿using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;
using Newtonsoft.Json;

namespace BlazorWebLib.Components.Constructor.Shared.Manufacture;

/// <summary>
/// SystemNameManufactureComponent
/// </summary>
public partial class SystemNameManufactureComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IManufactureService ManufactureRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required TreeItemData<EntryTagModel> Item { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ManufactureComponent ManufactureParentView { get; set; }

    /// <summary>
    /// Родительская страница конструктора
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }


    string Information
    {
        get
        {
            if (!string.IsNullOrEmpty(itemSystemName))
            {
                string? cn = ManufactureComponent.CheckName(itemSystemName);
                if (!string.IsNullOrEmpty(cn))
                {
                    return cn;
                }
            }

            return string.IsNullOrEmpty(ItemModel.Information) ? "" : ItemModel.Information;
        }
    }
    TreeItemDataModel ItemModel = default!;

    string? itemSystemName;

    /// <inheritdoc/>
    protected string DomID => $"{Item.Value!.Tag}-{Item.Value!.Id}";

    bool IsEdit => (itemSystemName ?? "") != (ItemModel.SystemName ?? "");

    /// <inheritdoc/>
    protected MarkupString InformationMS => (MarkupString)Information;

    async Task SaveSystemName()
    {
        if (Item.Value?.Tag is null)
            throw new Exception();

        IsBusyProgress = true;
        ResponseBaseModel res = await ManufactureRepo
            .SetOrDeleteSystemName(new UpdateSystemNameModel()
            {
                TypeDataId = Item.Value.Id,
                SystemName = itemSystemName,
                TypeDataName = Item.Value.Tag,
                Qualification = ItemModel.Qualification,
                ManufactureId = ManufactureParentView.Manufacture.Id,
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Success())
        {
            await ParentFormsPage.GetSystemNames();
            ItemModel.SystemName = itemSystemName;
            if (!string.IsNullOrWhiteSpace(ItemModel.SystemName))
            {
                int i = ParentFormsPage.SystemNamesManufacture.FindIndex(x => x.TypeDataId == ItemModel.Value!.Id && x.TypeDataName == ItemModel.Value!.Tag && x.Qualification == ItemModel.Qualification);

                if (i < 0)
                    throw new Exception($"[{nameof(ItemModel.SystemName)}:{ItemModel.SystemName}]\n{JsonConvert.SerializeObject(ParentFormsPage.SystemNamesManufacture)}");

                ParentFormsPage.SystemNamesManufacture[i].SystemName = itemSystemName;
            }

            ManufactureParentView.StateHasChangedCall();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ItemModel = (TreeItemDataModel)Item;
        ArgumentNullException.ThrowIfNull(Item.Value?.Tag);

        itemSystemName = ItemModel.SystemName;
    }
}