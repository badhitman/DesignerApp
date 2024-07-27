////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;

namespace ServerLib;

/// <summary>
/// Journal Constructor
/// </summary>
public class JournalConstructorService(IDbContextFactory<MainDbAppContext> mainDbFactory, IUsersProfilesService usersProfilesRepo) : IJournalUniversalService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentSchemes(string document_name, int? projectId)
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

        if (projectId.HasValue)
            pre_q = pre_q.Where(x => x.ProjectId == projectId.Value);

        static IIncludableQueryable<DocumentSchemeConstructorModelDB, List<ElementOfDirectoryConstructorModelDB>> IncQuery(IQueryable<DocumentSchemeConstructorModelDB> mq)
        {
            return mq
            .Include(x => x.Tabs!)
            .ThenInclude(x => x.JoinsForms!)
            .ThenInclude(x => x.Form!)
            .ThenInclude(x => x.Fields)
            .Include(x => x.Tabs!)
            .ThenInclude(x => x.JoinsForms!)
            .ThenInclude(x => x.Form!)
            .ThenInclude(x => x.FieldsDirectoriesLinks!)
            .ThenInclude(x => x.Directory!)
            .ThenInclude(x => x.Elements!);
        }

        res.Response = int.TryParse(document_name, out int doc_id)
            ? await IncQuery(pre_q.Where(f => f.Id == doc_id)).ToArrayAsync()
            : await IncQuery(pre_q.Where(f => f.Name == document_name)).ToArrayAsync();

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

        TResponseModel<DocumentSchemeConstructorModelDB[]?> find_doc = await FindDocumentSchemes(journal_name, projectId);
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
            res.AddWarning($"Найдено несколько документов '{journal_name}'. {string.Join(", ", find_doc.Response.Select(x => $"<a href='/documents-journal/{x.Id}?ProjectId={x.ProjectId}'>{x.Name}</a>"))};");
            return res;
        }

        res.Response = ExtractColumns(find_doc.Response);

        return res;
    }

    static EntryAltModel[] ExtractColumns(DocumentSchemeConstructorModelDB[] documents)
    {
        DocumentSchemeConstructorModelDB? _doc = documents.FirstOrDefault();
        if (_doc?.Tabs is null || _doc.Tabs.Count == 0)
            return [];

        if (_doc.Tabs.Count > 1)
            return _doc.Tabs
                .Select(x => new EntryAltModel() { Id = x.Id.ToString(), Name = x.Name })
                .ToArray();

        TabOfDocumentSchemeConstructorModelDB _tab = _doc.Tabs.Single();
        if (_tab.JoinsForms is null || _tab.JoinsForms.Count == 0)
            return [new EntryAltModel() { Id = _tab.Id.ToString(), Name = _tab.Name }];

        if (_tab.JoinsForms.Count > 1)
            return _tab.JoinsForms
                .Select(x => new EntryAltModel() { Id = x.Id.ToString(), Name = x.Name })
                .ToArray();

        FormConstructorModelDB _form = _tab.JoinsForms.Single().Form ?? throw new Exception();

        if (_form.AllFields.Length == 0)
            return [new EntryAltModel() { Id = _form.Id.ToString(), Name = _form.Name }];

        return _form.AllFields
            .Select(x => new EntryAltModel() { Id = x.Id.ToString(), Name = x.Name })
            .ToArray();
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>>> SelectJournalPart(SelectJournalPartRequestModel req, int? projectId)
    {
        TPaginationResponseModel<KeyValuePair<int, Dictionary<string, object>>> res = new();

#if DEBUG
        List<KeyValuePair<int, Dictionary<string, object>>> documents = [
            new(1, new()
            {
                { "Name", "Sam" }, { "Position", "CPA" }, { "YearsEmployed", 23 }, { "Salary", 87_000 }, { "Rating", 4 }
            }),
            new(2, new()
            {
                { "Name", "Alicia" }, { "Position", "Product Manager" }, { "YearsEmployed", 11 }, { "Salary", 143_000 }, { "Rating", 5 }
            }),
            new(3, new()
            {
                { "Name", "Ira" }, { "Position", "Developer" }, { "YearsEmployed", 4 }, { "Salary", 92_000 }, { "Rating", 3 }
            }),
            new(4, new()
            {
                { "Name", "John" }, { "Position", "IT Director" }, { "YearsEmployed", 17 }, { "Salary", 229_000 }, { "Rating", 4 }
            })
        ];
        res.Response = documents;
#endif

        TResponseModel<DocumentSchemeConstructorModelDB[]?> find_doc = await FindDocumentSchemes(req.DocumentNameOrId, projectId);
        if (!find_doc.Success() || find_doc.Response is null || find_doc.Response.Length == 0 || find_doc.Response.Length > 1)
            return res;

        DocumentSchemeConstructorModelDB doc_db = find_doc.Response.Single();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<SessionOfDocumentDataModelDB> q = context_forms
            .Sessions
            .Where(x => x.OwnerId == doc_db.Id);

        res.TotalRowsCount = await q.CountAsync();

        q = q
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        SessionOfDocumentDataModelDB[] sessions_db = await q
            .Include(x => x.DataSessionValues)
            .ToArrayAsync();

        static KeyValuePair<int, Dictionary<string, object>> ConvertTab(TabOfDocumentSchemeConstructorModelDB _tab)
        {
            Dictionary<string, object> rows_data = [];
            rows_data.Add(_tab.Name, _tab.Name);
            KeyValuePair<int, Dictionary<string, object>> res = new(_tab.Id, rows_data);

            return res;
        }

        if (doc_db.Tabs?.Count > 1)
        {
            res.Response = doc_db.Tabs
                .Select(ConvertTab)
                .ToList();

            return res;
        }

        return res;
    }
}