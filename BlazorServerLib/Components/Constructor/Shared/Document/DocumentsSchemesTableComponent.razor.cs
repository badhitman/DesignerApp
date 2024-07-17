////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Document;

/// <summary>
/// Documents view
/// </summary>
public partial class DocumentsSchemesTableComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IDialogService DialogServiceRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IConstructorService ConstructorRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required UserInfoModel CurrentUser { get; set; }


    MudTable<DocumentSchemeConstructorModelDB>? table;

    /// <inheritdoc/>
    protected string? searchString;
    TPaginationResponseModel<DocumentSchemeConstructorModelDB> data = new() { Response = [] };

    /// <inheritdoc/>
    protected static MarkupString Descr(string? html) => (MarkupString)(html ?? "");

    /// <inheritdoc/>
    protected async Task DeleteDocument(int questionnaire_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await ConstructorRepo.DeleteDocumentScheme(questionnaire_id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка F1AADB25-31FF-4305-90A9-4B71184434CC Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (table is not null)
            await table.ReloadServerData();
    }

    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    protected async Task<TableData<DocumentSchemeConstructorModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        SimplePaginationRequestModel req = new();
        IsBusyProgress = true;
        data = await ConstructorRepo.RequestDocumentsSchemes(req, ParentFormsPage.MainProject.Id, token);
        IsBusyProgress = false;

        if (data.Response is null)
        {
            SnackbarRepo.Add($"rest.Content.Documents is null. error 62D3109B-7349-48E8-932B-762D5B0EA585", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return new TableData<DocumentSchemeConstructorModelDB>() { TotalItems = data.TotalRowsCount, Items = data.Response };
        }

        return new TableData<DocumentSchemeConstructorModelDB>() { TotalItems = data.TotalRowsCount, Items = data.Response };
    }

    /// <inheritdoc/>
    protected async Task DocumentOpenDialog(DocumentSchemeConstructorModelDB? document_scheme = null)
    {
        if (ParentFormsPage.MainProject is null)
            throw new Exception("Не выбран основной/используемый проект");

        document_scheme ??= DocumentSchemeConstructorModelDB.BuildEmpty(ParentFormsPage.MainProject.Id);
        DialogParameters<EditDocumentSchemeDialogComponent> parameters = new()
        {
            { x => x.DocumentScheme, document_scheme },
            { x => x.ParentFormsPage, ParentFormsPage },
            { x => x.CurrentUser, CurrentUser },
        };

        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult? result = await DialogServiceRepo.Show<EditDocumentSchemeDialogComponent>(document_scheme.Id < 1 ? "Создание новой анкеты/опроса" : $"Редактирование анкеты/опроса #{document_scheme.Id}", parameters, options).Result;
        if (table is not null)
            await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }
}