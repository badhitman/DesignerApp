////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// OrdersAttendancesListComponent
/// </summary>
public partial class OrdersAttendancesListComponent(ICommerceRemoteTransmissionService CommerceRepo) : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// OrdersAttendances
    /// </summary>
    [Parameter, EditorRequired]
    public required OrderAttendanceModelDB[] OrdersAttendances { get; set; }

    int? _initDeleteOrder;

    bool CanDeleteRecord(OrderAttendanceModelDB rec) => CurrentUserSession?.IsAdmin == true || rec.AuthorIdentityUserId == CurrentUserSession?.UserId;

    async Task DeleteRecord(OrderAttendanceModelDB rec)
    {
        if (CurrentUserSession is null)
            return;

        if (_initDeleteOrder != rec.Id)
        {
            _initDeleteOrder = rec.Id;
            return;
        }

        await SetBusy();
        ResponseBaseModel res = await CommerceRepo.AttendanceRecordsDelete(new() { Payload = rec.Id, SenderActionUserId = CurrentUserSession.UserId });
        await SetBusy(false);
    }
}
