////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk.console;

/// <summary>
/// ConsoleSegmentColumnComponent
/// </summary>
public partial class ConsoleSegmentColumnComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IHelpdeskRemoteTransmissionService HelpdeskRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// StepIssue
    /// </summary>
    [Parameter, EditorRequired]
    public HelpdeskIssueStepsEnum StepIssue { get; set; }

    /// <summary>
    /// IsLarge
    /// </summary>
    [Parameter, EditorRequired]
    public bool IsLarge { get; set; }

    /// <summary>
    /// UserFilter
    /// </summary>
    [Parameter]
    public string? UserFilter { get; set; }


    string? _searchQuery;
    string? SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            pageNum = 0;
            Issues.Clear();
            InvokeAsync(LoadData);
        }
    }

    static MarkupString MyMarkup(string descr_issue) =>
        new(descr_issue);

    readonly List<IssueHelpdeskModel> Issues = [];
    int totalCount;
    int pageNum = 0;

    async Task LoadData()
    {
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<IssueHelpdeskModel>?> res = await HelpdeskRepo.ConsoleIssuesSelect(new TPaginationRequestModel<ConsoleIssuesRequestModel>
        {
            PageNum = pageNum,
            PageSize = 5,
            SortingDirection = VerticalDirectionsEnum.Down,
            Payload = new()
            {
                Status = StepIssue,
                SearchQuery = _searchQuery,
                FilterUserId = UserFilter,
                ProjectId = 0,
            }
        });

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);

        if (res.Response?.Response is not null && res.Response.Response.Count != 0)
        {
            totalCount = res.Response.TotalRowsCount;
            Issues.AddRange(res.Response.Response);
            pageNum++;
        }
    }

    string? _luf;
    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(!firstRender && _luf != UserFilter)
        {
            pageNum = 0;
            Issues.Clear();
            _luf = UserFilter;
            await LoadData();
            StateHasChanged();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }
}