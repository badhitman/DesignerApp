////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Form;

/// <summary>
/// Forms view
/// </summary>
public partial class FormsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IDialogService DialogServiceRepo { get; set; } = default!;

    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }


    /// <summary>
    /// имя типа данных: формы
    /// </summary>
    const string type_name_form_of_tab = nameof(FormConstructorModelDB);

    /// <summary>
    /// Таблица
    /// </summary>
    protected MudTable<FormConstructorModelDB>? table;

    /// <summary>
    /// Строка поиска
    /// </summary>
    protected string? searchString = null;

    TPaginationResponseModel<FormConstructorModelDB>? rest_data;

    /// <summary>
    /// Открыть форму
    /// </summary>
    protected async Task OpenForm(FormConstructorModelDB form)
    {
        await SetBusy();
        TResponseModel<FormConstructorModelDB> rest = await ConstructorRepo.GetForm(form.Id);
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
            { x => x.Form, rest.Response },
            { x => x.ParentFormsPage, ParentFormsPage }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult? result = await DialogServiceRepo.Show<EditFormDialogComponent>($"Редактирование формы #{rest.Response?.Id}", parameters, options).Result;

        if (table is not null)
            await table.ReloadServerData();
    }

    ///// <inheritdoc/>
    //protected override async Task OnInitializedAsync()
    //{
    //    await RestJson();
    //}


    TableState? _table_state;
    async Task RestJson()
    {
        //if (_table_state is null)
        //    return;

        if (ParentFormsPage.MainProject is null)
            throw new Exception("Проект не выбран.");

        await SetBusy();
        
        TResponseModel<TPaginationResponseModel<FormConstructorModelDB>> res = await ConstructorRepo.SelectForms(new() { Request = SimplePaginationRequestModel.Build(searchString, _table_state?.PageSize ?? 10, _table_state?.Page ?? 0), ProjectId = ParentFormsPage.MainProject.Id });
        rest_data = res.Response;
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
            { x => x.Form, FormConstructorModelDB.BuildEmpty(ParentFormsPage.MainProject.Id) },
            { x => x.ParentFormsPage, ParentFormsPage }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult? result = await DialogServiceRepo.Show<EditFormDialogComponent>("Создание новой формы", parameters, options).Result;

        if (table is not null)
            await table.ReloadServerData();
    }

    /// <summary>
    /// Загрузка данных форм
    /// </summary>
    protected async Task<TableData<FormConstructorModelDB>> ServerReload(TableState state, CancellationToken token)
    {
        bool _init = rest_data is null;
        if (_init)
            await RestJson();

        _table_state = state;

        if (!_init)
            await RestJson();

        return new TableData<FormConstructorModelDB>() { TotalItems = rest_data!.TotalRowsCount, Items = rest_data.Response };
    }

    /// <inheritdoc/>
    protected private async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }
}