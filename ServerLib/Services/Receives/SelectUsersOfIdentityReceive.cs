////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// SelectUsersOfIdentityReceive
/// </summary>
public class SelectUsersOfIdentityReceive(IDbContextFactory<IdentityAppDbContext> identityDbFactory)
    : IResponseReceive<TPaginationRequestModel<SimpleBaseRequestModel>?, TPaginationResponseModel<UserInfoModel?>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SelectUsersOfIdentityReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserInfoModel?>?>> ResponseHandleAction(TPaginationRequestModel<SimpleBaseRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        if (req.PageSize < 10)
            req.PageSize = 10;

        TResponseModel<TPaginationResponseModel<UserInfoModel?>?> res = new();
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        IQueryable<ApplicationUser> q = identityContext.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = q.Where(x => (x.NormalizedEmail != null && x.NormalizedEmail.Contains(req.Payload.SearchQuery)) ||
            (x.NormalizedUserName != null && x.NormalizedUserName.Contains(req.Payload.SearchQuery)) ||
            (x.NormalizedFirstNameUpper != null && x.NormalizedFirstNameUpper.Contains(req.Payload.SearchQuery)) ||
            (x.NormalizedLastNameUpper != null && x.NormalizedLastNameUpper.Contains(req.Payload.SearchQuery)));
        }

        res.Response = new()
        {
            TotalRowsCount = await q.CountAsync(),
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            Response = [..await q
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new UserInfoModel()
            {
                UserId = x.Id,
                AccessFailedCount = x.AccessFailedCount,
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                GivenName = x.FirstName,
                LockoutEnabled = x.LockoutEnabled,
                LockoutEnd = x.LockoutEnd,
                PhoneNumber = x.PhoneNumber,
                Surname = x.LastName,
                TelegramId = x.ChatTelegramId,
                UserName = x.UserName,
            })
            .ToListAsync()]
        };

        return res;
    }
}