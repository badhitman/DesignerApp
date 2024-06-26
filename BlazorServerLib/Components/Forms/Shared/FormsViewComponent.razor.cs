////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Forms view
/// </summary>
public partial class FormsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IDialogService DialogServiceRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;


    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }

    /// <summary>
    /// Таблица
    /// </summary>
    protected MudTable<ConstructorFormModelDB>? table;

    /// <summary>
    /// Строка поиска
    /// </summary>
    protected string? searchString = null;
    ConstructorFormsPaginationResponseModel rest_data = new();

    /// <summary>
    /// Открыть форму
    /// </summary>
    protected async Task OpenForm(ConstructorFormModelDB form)
    {
        IsBusyProgress = true;
        TResponseModel<ConstructorFormModelDB> rest = await FormsRepo.GetForm(form.Id);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка BDED4783-A604-4347-A344-1B66064CDDE8 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        StateHasChanged();
        DialogParameters<EditFormDialogComponent> parameters = new()
        {
            { x => x.Form, rest.Response }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await DialogServiceRepo.Show<EditFormDialogComponent>($"Редактирование формы #{rest.Response?.Id}", parameters, options).Result;

        if (table is not null)
            await table.ReloadServerData();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
        => await RestJson();


    TableState? _table_state;
    async Task RestJson()
    {
        if (_table_state is null)
            return;

        if (ParentFormsPage.MainProject is null)
            throw new Exception("Проект не выбран.");

        IsBusyProgress = true;
        rest_data = await FormsRepo.SelectForms(SimplePaginationRequestModel.Build(searchString, _table_state.PageSize, _table_state.Page), ParentFormsPage.MainProject.Id);
        IsBusyProgress = false;
    }

    /// <summary>
    /// Открыть диалог создания формы
    /// </summary>
    protected async Task OpenDialogCreateForm()
    {
        if (ParentFormsPage.MainProject is null)
        {
            SnackbarRepo.Add("Не выбран основной/текущий проект", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        DialogParameters<EditFormDialogComponent> parameters = new()
        {
            { x => x.Form, ConstructorFormModelDB.BuildEmpty(ParentFormsPage.MainProject.Id) }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await DialogServiceRepo.Show<EditFormDialogComponent>("Создание новой формы", parameters, options).Result;

        if (table is not null)
            await table.ReloadServerData();
    }

    /// <summary>
    /// Загрузка данных форм
    /// </summary>
    protected async Task<TableData<ConstructorFormModelDB>> ServerReload(TableState state)
    {
        _table_state = state;
        await RestJson();
        return new TableData<ConstructorFormModelDB>() { TotalItems = rest_data.TotalRowsCount, Items = rest_data.Elements };
    }

    /// <inheritdoc/>
    protected private async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }
}