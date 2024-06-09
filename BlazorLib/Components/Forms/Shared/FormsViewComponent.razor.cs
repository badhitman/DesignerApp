using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class FormsViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<FormsViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected IDialogService _dialog_service { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    protected MudTable<ConstructorFormModelDB>? table;

    protected string? searchString = null;
    protected ConstructorFormsPaginationResponseModel rest_data = new();

    protected async Task OpenForm(ConstructorFormModelDB form)
    {
        IsBusyProgress = true;
        FormResponseModel rest = await _forms.GetForm(form.Id);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{FB0BAB08-CEAA-4153-B786-E0EA7EB79FAF}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        StateHasChanged();
        DialogParameters<EditFormDialogComponent> parameters = new();
        parameters.Add(x => x.Form, rest.Form);
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await _dialog_service.Show<EditFormDialogComponent>($"Редактирование формы #{rest.Form?.Id}", parameters, options).Result;

        if (table is not null)
            await table.ReloadServerData();
    }

    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        await RestJson();
    }

    protected TableState? _table_state;
    protected async Task RestJson(int page_num = 0)
    {
        IsBusyProgress = true;
        rest_data = await _forms.SelectForms(new() { PageNum = _table_state?.Page ?? 0, SimpleRequest = searchString, PageSize = _table_state?.PageSize ?? 10 });
        IsBusyProgress = false;
    }

    protected async Task OpenDialogCreateForm()
    {
        DialogParameters<EditFormDialogComponent> parameters = new();
        parameters.Add(x => x.Form, (ConstructorFormModelDB)EntryDescriptionModel.Build(""));
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await _dialog_service.Show<EditFormDialogComponent>("Создание новой формы", parameters, options).Result;

        if (table is not null)
            await table.ReloadServerData();
    }

    protected async Task<TableData<ConstructorFormModelDB>> ServerReload(TableState state)
    {
        _table_state = state;
        await RestJson();
        return new TableData<ConstructorFormModelDB>() { TotalItems = rest_data.TotalRowsCount, Items = rest_data.Elements };
    }

    protected private async Task OnSearch(string text)
    {
        searchString = text;
        if (table is not null)
            await table.ReloadServerData();
    }
}