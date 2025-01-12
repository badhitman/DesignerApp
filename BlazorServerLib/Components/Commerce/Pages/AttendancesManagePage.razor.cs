////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Commerce.Pages;

/// <summary>
/// AttendancesManagePage
/// </summary>
public partial class AttendancesManagePage : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// Commerce
    /// </summary>
    [Inject]
    protected ICommerceTransmission CommerceRepo { get; set; } = default!;


    /// <summary>
    /// Обработчик добавления оффера
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<OfferActionModel> AddingOfferHandler { get; set; }

    /// <summary>
    /// Обработчик выбора Оффера
    /// </summary>
    [Parameter]
    public Action<OfferModelDB?>? SelectOfferHandler { get; set; }


    static readonly EntryAltModel[] showMarkersRoles = [new() { Id = GlobalStaticConstants.Roles.AttendancesExecutor, Name = "Исполнитель", },
    new() { Id = GlobalStaticConstants.Roles.CommerceClient, Name = "Покупатель", }];
}