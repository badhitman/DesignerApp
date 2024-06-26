////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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
        TResponseModel<EntryDictModel[]> rest = await FormsRepo.FindSessionsQuestionnairesByFormFieldName(new() { FormId = Form.Id, FieldName = FieldName });
        IsBusyProgress = false;
        
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка 8BDC72AC-AAE3-4EB0-93D3-F510D2324A78 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.Response is not null)
            ShowReferralsHandler(rest.Response);
    }
}