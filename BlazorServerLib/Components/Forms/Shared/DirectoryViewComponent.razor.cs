using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using SharedLib;
using BlazorLib;
using MudBlazor;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Directory view
/// </summary>
public partial class DirectoryViewComponent : BlazorBusyComponentBaseModel
{
    /*[Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;*/

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

#pragma warning disable IDE0044 // Добавить модификатор только для чтения
#pragma warning disable CS0649 // Полю нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию null.
    DirectoryElementsListViewComponent? DirectoryElementsListView_ref;
    DirectoryNavComponent? directoryNav_ref;
#pragma warning restore CS0649 // Полю нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию null.
#pragma warning restore IDE0044 // Добавить модификатор только для чтения

    async void SelectedDirectoryChangeAction(int selectedDirectoryId)
    {
        if (DirectoryElementsListView_ref is null)
            throw new Exception("Компонент перечня элементов справочника/списка не инициализирован");

        await DirectoryElementsListView_ref.ReloadElements(selectedDirectoryId, true);
    }

    /*
    /// <inheritdoc/>
    protected override async Task OnInitializedAsync() => await ReloadDirectories();

    

   

    

    /// <inheritdoc/>
    protected void DeleteSelectedDirectoryAction()
    {
        //IsBusyProgress = true;
        //_ = InvokeAsync(async () =>
        //{
        //    ResponseBaseModel rest = await FormsRepo.DeleteDirectory(SelectedDirectoryId);

        //    SnackbarRepo.ShowMessagesResponse(rest.Messages);

        //    await ReloadDirectories();
        //    StateHasChanged();
        //});
    }

    /// <inheritdoc/>
    protected async void CreateDirectoryAction((string Name, string SystemName) dir)
    {
        //if (ParentFormsPage.MainProject is null)
        //{
        //    SnackbarRepo.Add("Не выбран текущий/основной проект", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
        //    return;
        //}

        //IsBusyProgress = true;
        //TResponseStrictModel<int> rest = await FormsRepo.UpdateOrCreateDirectory(new EntryConstructedModel() { Name = dir.Name, SystemName = dir.SystemName, ProjectId = ParentFormsPage.MainProject.Id });
        //SnackbarRepo.ShowMessagesResponse(rest.Messages);
        //if (!rest.Success())
        //    return;

        //SelectedDirectoryId = rest.Response;
        //await ReloadDirectories();
        //StateHasChanged();
    }

    
    */
}