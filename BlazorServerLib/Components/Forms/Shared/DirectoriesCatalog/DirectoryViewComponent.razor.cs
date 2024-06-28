﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;

namespace BlazorWebLib.Components.Forms.Shared.DirectoriesCatalog;

/// <summary>
/// Directory view
/// </summary>
public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;


    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstrucnorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    /// <inheritdoc/>
    protected DirectoryElementsListViewComponent elementsListOfDirectoryView_ref = default!;

    /// <inheritdoc/>
    protected DirectoryNavComponent? directoryNav_ref;

    SystemOwnedNameModel createNewElementForDict = SystemOwnedNameModel.BuildEmpty(0);

    /// <inheritdoc/>
    protected async void AddElementIntoDirectory()
    {
        if (createNewElementForDict.OwnerId < 1)
            throw new Exception("Не выбран справочник/список");

        if (directoryNav_ref is not null)
        {
            directoryNav_ref.IsBusyProgress = true;
            directoryNav_ref.StateHasChangedCall();
        }

        IsBusyProgress = true;
        TResponseStrictModel<int> rest = await FormsRepo.CreateElementForDirectory(createNewElementForDict);
        IsBusyProgress = false;

        if (directoryNav_ref is not null)
        {
            directoryNav_ref.IsBusyProgress = false;
            directoryNav_ref.StateHasChangedCall();
        }

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        createNewElementForDict = SystemOwnedNameModel.BuildEmpty(createNewElementForDict.OwnerId);
        if (rest.Success())
            await elementsListOfDirectoryView_ref.ReloadElements(directoryNav_ref?.SelectedDirectoryId, true);
        else
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
        }
    }

    async void SelectedDirectoryChangeAction(int selectedDirectoryId)
    {
        createNewElementForDict = SystemOwnedNameModel.BuildEmpty(selectedDirectoryId);
        await elementsListOfDirectoryView_ref.ReloadElements(selectedDirectoryId, true);
        StateHasChanged();
    }
}