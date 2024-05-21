////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace DbTablesLib;

/// <summary>
/// Доступ к данным Telegram MainDbAppContext _db_context
/// </summary>
public class TelegramUsersTable(IDbContextFactory<MainDbAppContext> mainContextFactory, ILogger<TelegramUsersTable> _logger) : ITelegramTable
{
    /// <inheritdoc/>
    public async Task AddAsync(TelegramUserModelDb user)
    {
        MainDbAppContext _db_context = await mainContextFactory.CreateDbContextAsync();
        //
        user.NormalizedFirstName = user.FirstName.ToUpper();
        user.NormalizedLastName = user.LastName?.ToUpper();
        user.NormalizedName = user.Name?.ToUpper();
        //
        await _db_context.AddAsync(user);
        await _db_context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(TelegramUserModelDb user)
    {
        MainDbAppContext _db_context = await mainContextFactory.CreateDbContextAsync();
        //
        user.NormalizedFirstName = user.FirstName.ToUpper();
        user.NormalizedLastName = user.LastName?.ToUpper();
        user.NormalizedName = user.Name?.ToUpper();
        //
        _db_context.Update(user);
        await _db_context.SaveChangesAsync();
    }


    /// <inheritdoc/>
    public async Task<TelegramUserModelDb[]> FindUsersDataAsync(FindRequestModel req)
    {
        MainDbAppContext mainContext = await mainContextFactory.CreateDbContextAsync();
        //
        var q = mainContext.TelegramUsers
           .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.FindQuery))
            q = q.Where(x =>
            x.FirstName.ToLower().Contains(req.FindQuery.ToLower()) ||
            (x.Name != null && x.Name.ToLower().Contains(req.FindQuery.ToLower())) ||
            (x.LastName != null && x.LastName.ToLower().Contains(req.FindQuery.ToLower())));

        int total = q.Count();
        q = q.Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        TelegramUserModelDb[] users_tg = await q.ToArrayAsync();
        if (users_tg.Length == 0)
            return [];

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        var users_identity_data = await identityContext.Users
            .Where(x => users_tg.Select(y => y.TelegramId).Any(z => z == x.TelegramId))
            .Select(x => new { x.Id, x.Email, x.TelegramId })
            .ToArrayAsync();

        Func<TelegramUserModelDb, TelegramUserViewModel> identity_get = (TelegramUserModelDb ctx) =>
        {
            var id_data = users_identity_data.FirstOrDefault(x => x.TelegramId == ctx.TelegramId);
            return new TelegramUserViewModel()
            {
                FirstName = ctx.FirstName,
                LastName = ctx.LastName,
                CreatedAt = ctx.CreatedAt,
                Email = id_data?.Email,
                UserId = id_data?.Id,
                Id = ctx.Id,
                IsBot = ctx.IsBot,
                IsDeleted = ctx.IsDeleted,
                TelegramId = ctx.TelegramId,
                Name = ctx.Name
            };
        };

        return new TelegramUsersPaginationModel()
        {
            TelegramUsers = users_tg.Select(x => identity_get(x)).ToList(),
            TotalRowsCount = total,
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection
        };
    }
    /*
       /// <inheritdoc/>
       public async Task<UserLiteModel?> GetUserDataAsync(int id)
       {
           return await _db_context.Users.Include(x => x.Metadata).Include(x => x.Profile)
               .Select(x => new UserLiteModel()
               {
                   About = x.Profile!.About,
                   AccessLevelUser = x.Metadata!.AccessLevelUser,
                   ConfirmationType = x.Metadata.ConfirmationType,
                   CreatedAt = x.CreatedAt,
                   Email = x.Metadata.Email,
                   Id = x.Id,
                   Login = x.Profile.Login,
                   Name = x.Name
               })
               .FirstOrDefaultAsync(x => x.Id == id);
       }

       /// <inheritdoc/>
       public async Task<UserLiteModel?> GetUserDataAsync(string login)
       {
           return await _db_context.Users.Include(x => x.Metadata).Include(x => x.Profile)
               .Select(x => new UserLiteModel()
               {
                   About = x.Profile!.About,
                   AccessLevelUser = x.Metadata!.AccessLevelUser,
                   ConfirmationType = x.Metadata.ConfirmationType,
                   CreatedAt = x.CreatedAt,
                   Email = x.Metadata.Email,
                   Id = x.Id,
                   Login = x.Profile.Login,
                   Name = x.Name
               })
               .FirstOrDefaultAsync(x => x.Login == login); ;
       }

       /// <inheritdoc/>
       public async Task<bool> PasswordEqualByUserIdAsync(int user_id, string password_hash)
       {
           return await _db_context.Users.Include(x => x.Password)
               .AnyAsync(x => x.Id == user_id && x.Password!.Hash == password_hash);
       }

       /// <inheritdoc/>
       public async Task PasswordUpdateByUserIdAsync(int user_id, string password_hash)
       {
           TelegramUserModelDb? upd_user = await FirstOrDefaultAsync(user_id, inc_password: true, inc_metadata: false, inc_profile: false);

           if (upd_user is null)
               return;

           upd_user.Password!.Hash = password_hash;
           await UpdateAsync(upd_user, true);

           await SaveChangesAsync();
       }*/
}