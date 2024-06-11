using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Sessions values of field view
/// </summary>
public partial class SessionsValuesOfFieldViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }

    /// <summary>
    /// Имя поля
    /// </summary>
    [Parameter, EditorRequired]
    public required string FieldName { get; set; }

    /// <summary>
    /// Show referrals -  handler action
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<IEnumerable<EntryDictModel>> ShowReferralsHandler { get; set; }

    /// <summary>
    /// Найти использование полей (заполненные данными), связанные с данным документом/сессией
    /// </summary>
    public async Task FindFields()
    {
        IsBusyProgress = true;
        EntriesDictResponseModel rest = await FormsRepo.FindSessionsQuestionnairesByFormFieldName(new() { FormId = Form.Id, FieldName = FieldName });
        IsBusyProgress = false;
        
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{8425BABE-0EAF-44CC-925D-DBB5824EB1F3}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.Elements is not null)
            ShowReferralsHandler(rest.Elements);
    }
}