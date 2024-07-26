////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using SharedLib;

namespace ServerLib;

/// <summary>
/// Journal Universal
/// </summary>
public class JournalUniversalService(IDbContextFactory<MainDbAppContext> mainDbFactory, IUsersProfilesService usersProfilesRepo) : IJournalUniversalService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentScheme(string document_name, int? projectId, bool includeTabs)
    {
        TResponseModel<DocumentSchemeConstructorModelDB[]?> res = new();

        TResponseModel<UserInfoModel?> current_user = await usersProfilesRepo.FindByIdAsync();
        if (!current_user.Success())
        {
            res.Messages = current_user.Messages;
            return res;
        }

        if (current_user.Response is null)
        {
            res.AddError("Пользователь сессии не найден");
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<DocumentSchemeConstructorModelDB> pre_q = from scheme in context_forms.DocumentSchemes
                                                             join pt in context_forms.Projects on scheme.ProjectId equals pt.Id
                                                             where pt.OwnerUserId == current_user.Response.UserId || context_forms.MembersOfProjects.Any(x => x.ProjectId == pt.Id && x.UserId == current_user.Response.UserId)
                                                             select scheme;

        IQueryable<DocumentSchemeConstructorModelDB> q = pre_q.Where(f => f.Name == document_name);

        if (projectId.HasValue)
            q = q.Where(x => x.ProjectId == projectId.Value);

        res.Response = includeTabs
            ? await q.Include(x => x.Tabs).ToArrayAsync()
            : await q.ToArrayAsync();

        if (res.Response.Length == 0 && document_name.Length > 1 && int.TryParse(document_name, out int doc_id))
        {
            q = pre_q.Where(f => f.Id == doc_id);

            res.Response = includeTabs
                ? await q.Include(x => x.Tabs).ToArrayAsync()
                : await q.ToArrayAsync();

            res.Response = await context_forms
                .DocumentSchemes
                .Where(x => x.Id == doc_id)
                .ToArrayAsync();
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryAltModel[]?>> GetColumnsForJournal(string journal_name, int? projectId)
    {
        TResponseModel<EntryAltModel[]?> res = new();

#if DEBUG
        if (journal_name.Equals("test", StringComparison.OrdinalIgnoreCase))
        {
            res.Response = [
            new EntryAltModel() { Id = "Name",          Name = "Имя" },
            new EntryAltModel() { Id = "Position",      Name = "Дол-ть" },
            new EntryAltModel() { Id = "YearsEmployed", Name = "Стаж" },
            new EntryAltModel() { Id = "Salary",        Name = "Оклад" },
            new EntryAltModel() { Id = "Rating",        Name = "Ур-нь" }];
            return res;
        }
#endif

        TResponseModel<DocumentSchemeConstructorModelDB[]?> find_doc = await FindDocumentScheme(journal_name, projectId, true);
        if (!find_doc.Success())
        {
            res.Messages = find_doc.Messages;
            return res;
        }

        if (find_doc.Response is null || find_doc.Response.Length == 0)
        {
            res.AddError($"Документ '{journal_name}' не найден в базе данных");
            return res;
        }

        if (find_doc.Response.Length > 1)
        {
            res.AddWarning($"Найдено несколько документов '{journal_name}'. {string.Join(", ", find_doc.Response.Select(x => $"<a href='/documents-journal/{x.Id}?ProjectId={x.ProjectId}'>{x.Name}</a>"))}");
            return res;
        }

        res.Response = find_doc.Response.First()
            .Tabs!
            .Select(x => new EntryAltModel() { Id = x.Id.ToString(), Name = x.Name })
            .ToArray();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>[]?>> SelectJournalPart(TPaginationRequestModel<string> req, int? projectId)
    {
        TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>[]?> res = new();

        TResponseModel<DocumentSchemeConstructorModelDB[]?> find_doc = await FindDocumentScheme(req.Request, projectId, false);
        if (!find_doc.Success() || find_doc.Response is null || find_doc.Response.Length == 0 || find_doc.Response.Length > 1)
            return res;

        DocumentSchemeConstructorModelDB doc_db = find_doc.Response.First();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<SessionOfDocumentDataModelDB> q = context_forms
            .Sessions
            .Where(x => x.OwnerId == doc_db.Id);
        res.TotalRowsCount = await q.CountAsync();

        q = q
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        SessionOfDocumentDataModelDB[] sessions_db = await q
            .ToArrayAsync();

        return res;
    }
}