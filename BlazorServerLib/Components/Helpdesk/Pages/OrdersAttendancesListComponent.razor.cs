////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.Pages;

/// <summary>
/// OrdersAttendancesListComponent
/// </summary>
public partial class OrdersAttendancesListComponent(ICommerceTransmission CommerceRepo) : BlazorBusyComponentBaseAuthModel
{
    /// <summary>
    /// OrdersAttendances
    /// </summary>
    [Parameter, EditorRequired]
    public required RecordsAttendanceModelDB[] OrdersAttendances { get; set; }

    /// <summary>
    /// HelpdeskIssueId
    /// </summary>
    [Parameter, EditorRequired]
    public int HelpdeskIssueId { get; set; }

    /// <summary>
    /// UpdateRecords
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<RecordsAttendanceModelDB[]> UpdateRecords { get; set; }

    int? _initDeleteOrder;

    bool CanDeleteRecord(RecordsAttendanceModelDB rec) => CurrentUserSession?.IsAdmin == true || rec.AuthorIdentityUserId == CurrentUserSession?.UserId;

    async Task DeleteRecord(RecordsAttendanceModelDB rec)
    {
        if (CurrentUserSession is null)
            return;

        if (_initDeleteOrder != rec.Id)
        {
            _initDeleteOrder = rec.Id;
            return;
        }

        await SetBusy();
        ResponseBaseModel resDel = await CommerceRepo.AttendanceRecordsDelete(new() { Payload = rec.Id, SenderActionUserId = CurrentUserSession.UserId });
        SnackbarRepo.ShowMessagesResponse(resDel.Messages);

        TResponseModel<RecordsAttendanceModelDB[]> resReload = await CommerceRepo.OrdersAttendancesByIssues(new() { IssueIds = [HelpdeskIssueId] });
        SnackbarRepo.ShowMessagesResponse(resReload.Messages);

        if (resReload.Success() && resReload.Response is not null)
        {
            OrdersAttendances = resReload.Response;
            UpdateRecords(OrdersAttendances);
        }

        await SetBusy(false);
    }
}
