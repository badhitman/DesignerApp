﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;

namespace ServerLib;

/// <summary>
/// Journal Constructor
/// </summary>
public partial class JournalConstructorService(IDbContextFactory<MainDbAppContext> mainDbFactory, IUsersProfilesService usersProfilesRepo) : IJournalUniversalService
{
    /// <inheritdoc/>
    public async Task<DocumentFitModel> GetDocumentMetadata(string document_name_or_id, int? projectId = null)
    {
        TResponseModel<DocumentFitModel> res = new();

        TResponseModel<DocumentSchemeConstructorModelDB[]?> find_doc = await FindDocumentSchemes(document_name_or_id, projectId);
        if (!find_doc.Success())
            throw new Exception();

        if (find_doc.Response is null || find_doc.Response.Length == 0)
            throw new Exception();

        if (find_doc.Response.Length > 1)
            throw new Exception();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();


        ManufactureSystemNameModelDB[] system_names = await (from sys_name in context_forms.SystemNamesManufactures
                                                             join man in context_forms.Manufactures.Where(x => x.ProjectId == find_doc.Response.Single().ProjectId) on sys_name.ManufactureId equals man.Id
                                                             select sys_name).ToArrayAsync();

        return IJournalUniversalService.DocumentConvert(find_doc.Response[0], [.. system_names.Select(x => new SystemNameEntryModel() { TypeDataId = x.TypeDataId, TypeDataName = x.TypeDataName, Qualification = x.Qualification, SystemName = x.SystemName })]);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryAltModel[]?>> GetColumnsForJournal(string journal_name_or_id, int? projectId)
    {
        TResponseModel<EntryAltModel[]?> res = new();

        TResponseModel<DocumentSchemeConstructorModelDB[]?> find_doc = await FindDocumentSchemes(journal_name_or_id, projectId);
        if (!find_doc.Success())
        {
            res.Messages = find_doc.Messages;
            return res;
        }

        if (find_doc.Response is null || find_doc.Response.Length == 0)
        {
            res.AddError($"Документ '{journal_name_or_id}' не найден в базе данных");
            return res;
        }

        if (find_doc.Response.Length > 1)
        {
            res.AddWarning($"Найдено несколько документов '{journal_name_or_id}'. {string.Join(", ", find_doc.Response.Select(x => $"<a href='/documents-journal/{x.Id}?ProjectId={x.ProjectId}'>{x.Name}</a>"))};");
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

        KeyValuePair<int, Dictionary<string, object>> SessionConvert(SessionOfDocumentDataModelDB _session)
        {
            string AboutTab(TabOfDocumentSchemeConstructorModelDB _tab)
            {
                if (_tab.JoinsForms is null || _tab.JoinsForms.Count == 0)
                    return "форм на вкладке нет";
                else if (_tab.JoinsForms.Count == 1)
                    return AboutForm(_tab.JoinsForms[0].Form!);
                else
                    return $"{_tab.JoinsForms.Count} форм.";
            }

            string AboutForm(FormConstructorModelDB _form)
            {
                if (_form.AllFields.Length == 0)
                    return "без полей";

                return $"{_form.AllFields.Length} полей";
            }

            if (doc_db.Tabs is null || doc_db.Tabs.Count == 0)
                return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, new() { { doc_db.Name, "документ не сконфигурирован" } });
            else if (doc_db.Tabs.Count == 1)
            {
                TabOfDocumentSchemeConstructorModelDB _tab = doc_db.Tabs[0];
                if (_tab.JoinsForms is null || _tab.JoinsForms.Count == 0)
                    return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, new() { { _tab.Name, AboutTab(_tab) } });
                else if (_tab.JoinsForms.Count == 1)
                {
                    FormToTabJoinConstructorModelDB _join = _tab.JoinsForms[0];
                    FormConstructorModelDB _form = _join.Form ?? throw new Exception();
                    FieldFormBaseLowConstructorModel[] _fields = _form.AllFields;
                    if (_fields.Length == 0)
                        return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, new() { { _form.Name, AboutForm(_form) } });
                    else
                    {
                        Dictionary<string, object> _fields_raw = [];
                        if (_session.DataSessionValues is null || _session.DataSessionValues.Count == 0)
                        {
                            foreach (FieldFormBaseLowConstructorModel _f in _fields)
                                _fields_raw.Add(_f.Name, "");

                            return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, _fields_raw);
                        }

                        ValueDataForSessionOfDocumentModelDB[] data = _session
                            .DataSessionValues
                            .Where(x => x.JoinFormToTabId == _join.Id)
                            .ToArray();

                        foreach (FieldFormBaseLowConstructorModel f in _fields)
                            _fields_raw.Add(f.Name, data.FirstOrDefault()?.Value ?? "");

                        return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, _fields_raw);
                    }
                }
                else
                {
                    Dictionary<string, object> _raw_forms = [];
                    _tab.JoinsForms.ForEach(x =>
                    {
                        _raw_forms.Add(x.Form!.Name, AboutForm(x.Form));
                    });

                    return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, _raw_forms);
                }
            }
            else
            {
                Dictionary<string, object> columns_tabs = [];
                doc_db.Tabs.ForEach(x => { columns_tabs.Add(x.Name, AboutTab(x)); });
                return new KeyValuePair<int, Dictionary<string, object>>(_session.Id, columns_tabs);
            }
        }

        res.Response = sessions_db
            .Select(SessionConvert)
            .ToList();

        if (!string.IsNullOrWhiteSpace(req.SearchString))
            res.Response = res.Response
                .Where(x => x.Value.Any(y => y.Value.ToString()?.Contains(req.SearchString, StringComparison.OrdinalIgnoreCase) == true))
                .ToList();

        res.TotalRowsCount = res.Response.Count;

        if (!string.IsNullOrWhiteSpace(req.SortBy))
        {
            res.Response = req.SortingDirection == VerticalDirectionsEnum.Up
                ? [.. res.Response.OrderBy(x => x.Value[req.SortBy])]
                : [.. res.Response.OrderByDescending(x => x.Value[req.SortBy])];
        }

        res.Response = res.Response
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize)
            .ToList();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentSchemes(string document_name_or_id, int? projectId)
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

        res.Response = int.TryParse(document_name_or_id, out int doc_id)
            ? await IncQuery(pre_q.Where(f => f.Id == doc_id)).ToArrayAsync()
            : await IncQuery(pre_q.Where(f => f.Name == document_name_or_id)).ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<EntryAltTagModel[]> GetMyDocumentsSchemas()
    {
        TResponseModel<UserInfoModel?> current_user = await usersProfilesRepo.FindByIdAsync();
        if (!current_user.Success() || current_user.Response is null)
            return [];

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<EntryAltTagModel> pre_q = from scheme in context_forms.DocumentSchemes
                                             join pt in context_forms.Projects on scheme.ProjectId equals pt.Id
                                             where pt.OwnerUserId == current_user.Response.UserId || context_forms.MembersOfProjects.Any(x => x.ProjectId == pt.Id && x.UserId == current_user.Response.UserId)
                                             select new EntryAltTagModel() { Id = scheme.Id.ToString(), Name = scheme.Name, Tag = pt.Name };

        return await pre_q.OrderBy(x => x.Name).ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<ValueDataForSessionOfDocumentModelDB[]> ReadSessionTabValues(int tabId, int sessionId)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<ValueDataForSessionOfDocumentModelDB> q = from val in context_forms.ValuesSessions.Where(x => x.OwnerId == sessionId)
                join tj in context_forms.TabsJoinsForms.Where(x => x.TabId == tabId) on val.JoinFormToTabId equals tj.Id
                select val;

        return await q.Include(x => x.JoinFormToTab).ToArrayAsync();
    }
}