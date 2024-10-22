////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared;

/// <summary>
/// Sessions values of field view
/// </summary>
public partial class SessionsValuesOfFieldViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <summary>
    /// Имя поля
    /// </summary>
    [Parameter, EditorRequired]
    public required string FieldName { get; set; }

    /// <summary>
    /// Show referrals -  handler action
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<EntryDictModel[]> ShowReferralsHandler { get; set; }

    /// <summary>
    /// Найти использование полей (заполненные данными), связанные с данным документом/сессией
    /// </summary>
    public async Task FindFields()
    {
        await SetBusy();
        TResponseModel<EntryDictModel[]> rest = await ConstructorRepo.FindSessionsDocumentsByFormFieldName(new() { FormId = Form.Id, FieldName = FieldName });
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