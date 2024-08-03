////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using Newtonsoft.Json;
using IdentityLib;
using SharedLib;
using DbcLib;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ServerLib;

/// <summary>
/// Constructor служба
/// </summary>
public partial class ConstructorService(
    IDbContextFactory<MainDbAppContext> mainDbFactory,
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    IUsersProfilesService usersProfilesRepo,
    ILogger<ConstructorService> logger,
    IOptions<ServerConstructorConfigModel> _conf) : IConstructorService
{
    static readonly Random r = new();

    #region public    

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocumentData(string guid_session, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(guid_session, out Guid guid_parsed) || guid_parsed == Guid.Empty)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Токен сессии имеет не корректный формат" }] };

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<SessionOfDocumentDataModelDB> q = context_forms
            .Sessions
            .Include(x => x.DataSessionValues)

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Tabs!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.Fields) // поля

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Tabs!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.FieldsDirectoriesLinks) // поля

            .Include(x => x.Project)

            .AsSplitQuery();

        TResponseModel<SessionOfDocumentDataModelDB> res = new()
        {
            Response = await q
            .FirstOrDefaultAsync(x => x.SessionToken == guid_session, cancellationToken: cancellationToken)
        };
        string msg;
        if (res.Response is null)
        {
            msg = $"Токен '{guid_session}' не найден или просрочен.";
            res.AddError(msg);
            logger.LogError($"{msg} res.SessionDocument is null. error 61362F88-21C8-431A-9038-475B4C52B759");
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetDoneSessionDocumentData(string token_session, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        SessionOfDocumentDataModelDB? sq = await context_forms.Sessions.FirstOrDefaultAsync(x => x.SessionToken == token_session, cancellationToken: cancellationToken);
        if (sq is null)
            return ResponseBaseModel.CreateError($"Сессия [{token_session}] не найдена в БД. ошибка 638FB569-BB5E-43C2-9263-2DCB76F7D88E");

        if (sq.SessionStatus >= SessionsStatusesEnum.Sended)
            return ResponseBaseModel.CreateSuccess("Сессия уже отмечена как выполненная и не требует вмешательства");

        string msg = $"Сессия опросника/анкетирования `{token_session}` отправлена на проверку";
        ResponseBaseModel res = new();
        if (!string.IsNullOrWhiteSpace(sq.EmailsNotifications))
        {
            try
            {
                res.AddSuccess($"Наблюдатели оповещены:{sq.EmailsNotifications}");
            }
            catch (Exception ex)
            {
                logger.LogError($"error : {ex.Message}\n: {ex.StackTrace}");
                res.AddWarning($"Не удалось отправить уведомление наблюдателям [{sq.EmailsNotifications}]. Возникло исключение D2D8892C-C018-4069-A978-560A90E7A882: {ex.Message}");
            }
        }

        TResponseModel<UserInfoModel?> author_user = await usersProfilesRepo.FindByIdAsync(sq.AuthorUser);
        if (author_user.Response is null)
        {
            msg = $"Автор ссылки (пользователь #{sq.AuthorUser}) не найден в БД.\n{author_user.Message()}";
            res.AddWarning(msg);
            logger.LogError(msg);
            return res;
        }

        res.AddSuccess($"Сессия [{token_session}] успешно отправлена на проверку:{author_user.Response.UserName}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> SetValueFieldSessionDocumentData(SetValueFieldDocumentDataModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<SessionOfDocumentDataModelDB> session = await GetSessionDocument(req.SessionId, false, cancellationToken);
        if (!session.Success())
            return session;

        if (session.Response!.Project!.SchemeLastUpdated != req.ProjectVersionStamp)
        {
            session.AddError("Версия схемы документа изменилась. Обновите свою сессию (F5)");
            return session;
        }

        SessionOfDocumentDataModelDB? session_Document = session.Response;
        TResponseModel<SessionOfDocumentDataModelDB> res = new();

        if (session_Document?.Owner?.Tabs is null || session_Document.DataSessionValues is null)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка B3AC5AAF-A786-4190-9C61-A272F174D940");
            return res;
        }

        if (session_Document.SessionStatus >= SessionsStatusesEnum.Sended)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (находиться в статусе {session_Document.SessionStatus}).");
            return res;
        }

        session_Document.LastDocumentUpdateActivity = DateTime.Now;

        TabJoinDocumentSchemeConstructorModelDB? form_join = session_Document.Owner.Tabs.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FieldsDirectoriesLinks is null)
        {
            res.AddError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена. ошибка 2494D4D2-24E1-48D4-BC9C-C27D327D98B8");
            return res;
        }

        FieldFormBaseLowConstructorModel? field_by_name = form_join.Form.AllFields.FirstOrDefault(x => x.Name.Equals(req.NameField, StringComparison.OrdinalIgnoreCase));
        if (field_by_name is null)
        {
            res.AddError($"Поле '{req.NameField}' не найдено в форме #{form_join.Form.Id} '{form_join.Form.Name}'. ошибка 98371573-83A3-41A3-97C2-F8F775BFFD2D");
            return res;
        }
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ValueDataForSessionOfDocumentModelDB? existing_value = session_Document.DataSessionValues.FirstOrDefault(x => x.RowNum == req.GroupByRowNum && x.Name.Equals(req.NameField, StringComparison.OrdinalIgnoreCase) && x.TabJoinDocumentSchemeId == form_join.Id);
        if (existing_value is null)
        {
            if (req.FieldValue is null)
                return await GetSessionDocument(req.SessionId, false, cancellationToken);

            existing_value = ValueDataForSessionOfDocumentModelDB.Build(req, form_join, session_Document);

            existing_value.Owner = null;
            existing_value.TabJoinDocumentScheme = null;

            await context_forms.AddAsync(existing_value, cancellationToken);
        }
        else if (req.FieldValue is null)
            context_forms.Remove(existing_value);
        else
        {
            existing_value.Value = req.FieldValue;
            existing_value.Description = req.Description;
            context_forms.Update(existing_value);
        }

        await context_forms.SaveChangesAsync(cancellationToken);

        return await GetSessionDocument(req.SessionId, false, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>> AddRowToTable(FieldSessionDocumentDataBaseModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<SessionOfDocumentDataModelDB> get_s = await GetSessionDocument(req.SessionId, false, cancellationToken);
        if (!get_s.Success())
            return new TResponseStrictModel<int>() { Messages = get_s.Messages, Response = 0 };
        TResponseStrictModel<int> res = new() { Response = 0 };
        SessionOfDocumentDataModelDB? session = get_s.Response;

        if (session?.Owner?.Tabs is null || session.DataSessionValues is null)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка 14504D03-88B5-4D1B-AFF2-8DB8D4EB757F");
            return res;
        }

        if (session.SessionStatus >= SessionsStatusesEnum.Sended)
            return (TResponseStrictModel<int>)ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (в статусе {session.SessionStatus}).");

        TabJoinDocumentSchemeConstructorModelDB? form_join = session.Owner.Tabs.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FieldsDirectoriesLinks is null || !form_join.IsTable)
        {
            res.AddError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена или повреждена. ошибка 6342356D-0491-45BC-A33D-B95F5D7DCB5F");
            return res;
        }
        IQueryable<ValueDataForSessionOfDocumentModelDB> q = session.DataSessionValues.Where(x => x.TabJoinDocumentSchemeId == form_join.Id && x.RowNum > 0).AsQueryable();
        res.Response = (int)(q.Any() ? (q.Max(x => x.RowNum) + 1) : 1);
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ValueDataForSessionOfDocumentModelDB[] rows_add = form_join.Form.AllFields.Where(ScalarOnly).Select(x => new ValueDataForSessionOfDocumentModelDB()
        {
            RowNum = (uint)res.Response,
            Name = x.Name,
            OwnerId = session.Id,
            TabJoinDocumentSchemeId = form_join.Id
        }).ToArray();
        await context_forms.AddRangeAsync(rows_add, cancellationToken);
        session.LastDocumentUpdateActivity = DateTime.Now;

        await context_forms.SaveChangesAsync(cancellationToken);
        res.AddSuccess($"Добавлена строка в таблицу: №п/п {res.Response}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionDocumentDataByRowNum(ValueFieldSessionDocumentDataBaseModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<SessionOfDocumentDataModelDB> get_s = await GetSessionDocument(req.SessionId, false, cancellationToken);
        if (!get_s.Success())
            return get_s;

        SessionOfDocumentDataModelDB? session = get_s.Response;
        if (session?.Owner?.Tabs is null || session.DataSessionValues is null)
            return ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка 5DF6598B-18FF-4E76-AE33-6CE78ACE5442");

        if (session.SessionStatus >= SessionsStatusesEnum.Sended)
            return ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (статус {session.SessionStatus}).");

        TabJoinDocumentSchemeConstructorModelDB? form_join = session.Owner.Tabs.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FieldsDirectoriesLinks is null || !form_join.IsTable)
            return ResponseBaseModel.CreateError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена или повреждена. ошибка 66A38A11-CD9B-4F9E-8B5C-49E60109442D");

        ValueDataForSessionOfDocumentModelDB[] values_for_delete = session.DataSessionValues.Where(x => x.RowNum == req.GroupByRowNum && x.TabJoinDocumentSchemeId == form_join.Id).ToArray();

        ResponseBaseModel res = new();
        if (req.IsSelf && values_for_delete.Any(x => !string.IsNullOrWhiteSpace(x.Value)))
        {
            res.AddWarning($"В строке есть данные. Строка не может быть удалена.");
            return res;
        }

        if (values_for_delete.Length > 0)
        {
            session.LastDocumentUpdateActivity = DateTime.Now;
            using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
            context_forms.RemoveRange(values_for_delete);
            await context_forms.SaveChangesAsync(cancellationToken);
            res.AddSuccess($"Строка №{req.GroupByRowNum} удалена ({values_for_delete.Length} значений ячеек)");

            get_s = await GetSessionDocument(req.SessionId, false, cancellationToken);
            uint i = 0;
            List<ValueDataForSessionOfDocumentModelDB> values_re_sort = [];
            session.DataSessionValues
                .Where(x => x.RowNum > 0)
                .GroupBy(x => x.RowNum)
                .ToList()
                .ForEach(x =>
                {
                    i++;
                    if (i != x.Key)
                        values_re_sort.AddRange(x.ToList().Select(y => { y.RowNum = i; return y; }));
                })
                ;
            if (values_re_sort.Count > 0)
            {
                res.AddInfo($"Нормализация нумерации строк: затронуто {values_re_sort.Count} элементов");
                context_forms.UpdateRange(values_re_sort);
                await context_forms.SaveChangesAsync(cancellationToken);
            }
        }
        else
            res.AddError($"Строки №{req.GroupByRowNum} нет в таблице");

        return res;
    }

    static bool ScalarOnly(FieldFormBaseLowConstructorModel x) => !(x is FieldFormConstructorModelDB _f && _f.TypeField == TypesFieldsFormsEnum.ProgramCalculationDouble);

    static string? EditorsGenerate(SessionOfDocumentDataModelDB session, string editor)
    {
        if (session.Editors?.StartsWith(editor) == true || session.Editors?.EndsWith(editor) == true || session.Editors?.Contains($", {editor},") == true)
            return session.Editors;
        editor = editor.Trim();
        if (string.IsNullOrWhiteSpace(session.Editors))
            session.Editors = editor;
        else
            session.Editors = $"{session.Editors}, {editor}";

        return session.Editors;
    }

    #endregion

    /////////////// Контекст работы конструктора: работы в системе над какими-либо сущностями всегда принадлежат какому-либо проекту/контексту.
    // При переключении контекста (текущий/основной проект) становятся доступны только работы по этому проекту
    // В проект можно добавлять участников, что бы те могли работать вместе с владельцем => вносить изменения в конструкторе данного проекта/контекста
    // Если проект отключить (есть у него такой статус: IsDisabled), то работы с проектом блокируются для всех участников, кроме владельца
    #region проекты
    /// <inheritdoc/>
    public async Task<ProjectViewModel[]> GetProjects(string user_id, string? name_filter = null)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        IQueryable<ProjectConstructorModelDB> q = context_forms
            .Projects
            .Where(x => x.OwnerUserId == user_id || context_forms.MembersOfProjects.Any(y => y.ProjectId == x.Id && y.UserId == user_id))
            .Include(x => x.Members)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name_filter))
            q = q.Where(x => EF.Functions.Like(x.Name.ToUpper(), $"%{name_filter.ToUpper()}%"));

        ProjectConstructorModelDB[] raw_data = await q.ToArrayAsync();

        string[] usersIds = raw_data
            .Where(x => x.Members is not null)
            .SelectMany(x => x.Members!)
            .Select(x => x.UserId)
            .Union(raw_data.Select(x => x.OwnerUserId))
            .Distinct()
            .ToArray();

        EntryAltModel[]? usersIdentity = null;

        if (usersIds.Length != 0)
        {
            using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
            usersIdentity = await identityContext
                .Users
                .Where(x => usersIds.Contains(x.Id))
                .Select(x => new EntryAltModel { Id = x.Id, Name = x.UserName })
                .ToArrayAsync();
        }

        List<EntryAltModel>? ReadMembersData(List<MemberOfProjectConstructorModelDB>? members)
        {
            if (members is null || usersIdentity is null)
                return null;

            return usersIdentity
                .Where(identityUser => members.Any(memberOfProject => memberOfProject.UserId == identityUser.Id))
                .ToList();
        }

        Func<ProjectConstructorModelDB, ProjectViewModel> cast_expression = (project) => new ProjectViewModel()
        {
            OwnerUserId = project.OwnerUserId,
            Name = project.Name,
            Description = project.Description,
            Id = project.Id,
            IsDisabled = project.IsDisabled,
            SchemeLastUpdated = project.SchemeLastUpdated,
            Members = ReadMembersData(project.Members),
        };

        return raw_data.Select(cast_expression).ToArray();
    }

    /// <inheritdoc/>
    public async Task<ProjectConstructorModelDB?> ReadProject(int project_id)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        return await context_forms
            .Projects

            .Include(x => x.Forms!)
            .ThenInclude(x => x.Fields)

            .Include(x => x.Forms!)
            .ThenInclude(x => x.FieldsDirectoriesLinks)

            .Include(x => x.Directories!)
            .ThenInclude(x => x.Elements)

            .Include(x => x.Documents!)
            .ThenInclude(x => x.Tabs!)
            .ThenInclude(x => x.JoinsForms)

            //.Include(x => x.Documents!)
            //.ThenInclude(x=>x.)

            .FirstOrDefaultAsync(x => x.Id == project_id);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CreateProject(ProjectViewModel project, string user_id)
    {
        TResponseModel<int> res = new();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(project);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();

        ApplicationUser? userDb = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == user_id);
        if (userDb is null)
        {
            res.AddError($"Пользователь #{user_id} не найден в БД");
            return res;
        }
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ProjectConstructorModelDB? projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.OwnerUserId == userDb.Id && (x.Name == project.Name));

        if (projectDb is not null)
        {
            res.AddError($"Проект должен иметь уникальное имя и код. Похожий проект есть в БД: #{projectDb.Id} '{projectDb.Name}'");
            return res;
        }

        projectDb = new()
        {
            Name = project.Name,
            OwnerUserId = userDb.Id,
            Description = project.Description,
            IsDisabled = project.IsDisabled,
        };

        await context_forms.AddAsync(projectDb);
        await context_forms.SaveChangesAsync();

        res.Response = projectDb.Id;
        res.AddSuccess("Проект создан");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetMarkerDeleteProject(int project_id, bool is_deleted)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        ProjectConstructorModelDB? project = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id == project_id);

        if (project is null)
            return ResponseBaseModel.CreateError($"Проект #{project_id} не найден в БД");

        project.IsDisabled = is_deleted;
        context_forms.Update(project);
        await context_forms.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Проект '{project.Name}' {(is_deleted ? "выключен" : "включён")}");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateProject(ProjectViewModel project)
    {
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(project);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        ProjectConstructorModelDB? projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id != project.Id && x.Name.ToUpper() == project.Name.ToUpper());

        if (projectDb is not null)
            return ResponseBaseModel.CreateError($"Проект должен иметь уникальное имя и код. Похожий проект есть в БД: #{projectDb.Id} '{projectDb.Name}'");

        projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id == project.Id);

        if (projectDb is null)
            return ResponseBaseModel.CreateError($"Проект #{project.Id} не найден в БД");

        if (project.Name == projectDb.Name && project.Description == projectDb.Description)
            return ResponseBaseModel.CreateInfo("Объект не изменён");

        projectDb.Reload(project);

        context_forms.Update(projectDb);
        await context_forms.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess("Проект обновлён");
    }

    /// <inheritdoc/>
    public async Task<EntryAltModel[]> GetMembersOfProject(int project_id)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        string[] members_users_ids = await context_forms
            .MembersOfProjects
            .Where(x => x.ProjectId == project_id)
            .Select(x => x.UserId)
            .ToArrayAsync();

        if (members_users_ids.Length == 0)
            return [];

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        return await identityContext
            .Users
            .Where(x => members_users_ids.Contains(x.Id))
            .Select(x => new EntryAltModel() { Id = x.Id, Name = x.UserName })
            .ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddMemberToProject(int project_id, string member_user_id)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userDb = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == member_user_id);

        if (userDb is null)
            return ResponseBaseModel.CreateError($"Пользователь #{member_user_id} не найден в БД");

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        MemberOfProjectConstructorModelDB? memberDb = await context_forms
            .MembersOfProjects
            .FirstOrDefaultAsync(x => x.ProjectId == project_id && x.UserId == userDb.Id);

        if (memberDb is not null)
            return ResponseBaseModel.CreateInfo("Пользователь уже является участником проекта");

        ProjectConstructorModelDB? projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id == project_id);

        if (projectDb is null)
            return ResponseBaseModel.CreateError($"Проект #{project_id} не найден в БД");

        ApplicationUser? ownerProject = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == projectDb.OwnerUserId);

        if (ownerProject is null)
            return ResponseBaseModel.CreateError($"Владелец проекта #{projectDb.OwnerUserId} не найден в БД");

        if (ownerProject.Email?.Equals(userDb.Email) == true)
            return ResponseBaseModel.CreateInfo($"Пользователь {userDb.Email} является владельцем проекта, поэтому не может быть добавлен как участник.");

        memberDb = new()
        {
            Project = projectDb,
            ProjectId = projectDb.Id,
            UserId = userDb.Id
        };

        await context_forms.AddAsync(memberDb);
        await context_forms.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Пользователь/участник {userDb.UserName} добавлен к проекту '{projectDb.Name}'");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteMemberFromProject(int project_id, string member_user_id)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userDb = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == member_user_id);

        if (userDb is null)
            return ResponseBaseModel.CreateError($"Пользователь #{member_user_id} не найден в БД");

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        MemberOfProjectConstructorModelDB? memberDb = await context_forms
            .MembersOfProjects
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.ProjectId == project_id && x.UserId == userDb.Id);
        if (memberDb is null)
            return ResponseBaseModel.CreateInfo("Пользователь не является участником проекта. Удаление не требуется");

        context_forms.Remove(memberDb);

        ProjectUseConstructorModelDb? main_project_for_member = await context_forms
            .ProjectsUse
            .FirstOrDefaultAsync(x => x.ProjectId == project_id && x.UserId == member_user_id);

        if (main_project_for_member is not null)
            context_forms.Remove(main_project_for_member);

        await context_forms.SaveChangesAsync();
        return ResponseBaseModel.CreateSuccess($"Пользователь {userDb.UserName} успешно исключён из проекта '{memberDb.Project!.Name}'");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetProjectAsMain(int project_id, string user_id)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userDb = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == user_id);

        if (userDb is null)
            return ResponseBaseModel.CreateError($"Пользователь #{user_id} не найден в БД");

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ProjectConstructorModelDB? projectDb = await context_forms.Projects.FirstOrDefaultAsync(x => x.Id == project_id);
        if (projectDb is null)
            return ResponseBaseModel.CreateError($"Проект #{project_id} не найден в БД");

        ProjectUseConstructorModelDb? mainProjectDb = await context_forms.ProjectsUse.FirstOrDefaultAsync(x => x.UserId == user_id);
        if (mainProjectDb is null)
        {
            mainProjectDb = new ProjectUseConstructorModelDb() { UserId = user_id, ProjectId = project_id, Project = projectDb };
            await context_forms.AddAsync(mainProjectDb);
        }
        else
        {
            mainProjectDb.Project = projectDb;
            mainProjectDb.ProjectId = project_id;
            context_forms.Update(mainProjectDb);
        }
        await context_forms.SaveChangesAsync();
        return ResponseBaseModel.CreateSuccess($"Проект '{projectDb.Name}' успешно установлен в роли основного/используемого");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<MainProjectViewModel>> GetCurrentMainProject(string user_id)
    {
        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userDb = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == user_id);

        TResponseModel<MainProjectViewModel> res = new();
        if (userDb is null)
        {
            res.AddError($"Пользователь #{user_id} не найден в БД");
            return res;
        }

#if DEMO
        bool _seed_call = false;
#endif

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ProjectConstructorModelDB? project = null;
        ProjectUseConstructorModelDb? project_use = null;
        if (!await context_forms.Projects.AnyAsync(x => x.OwnerUserId == user_id) && !await context_forms.MembersOfProjects.AnyAsync(x => x.UserId == user_id))
        {
            project = new() { Name = "По умолчанию", OwnerUserId = user_id };
            await context_forms.AddAsync(project);
            await context_forms.SaveChangesAsync();
#if DEMO
            _seed_call = true;
#endif
            project_use = new() { UserId = project.OwnerUserId, ProjectId = project.Id };
            await context_forms.AddAsync(project_use);
            await context_forms.SaveChangesAsync();
        }

        res.Response = project is not null
            ? MainProjectViewModel.Build(project)
            : await context_forms.ProjectsUse.Where(x => x.UserId == user_id)
            .Include(x => x.Project)
            .Select(x => new MainProjectViewModel() { Id = x.Project!.Id, Name = x.Project.Name, Description = x.Project.Description, IsDisabled = x.Project.IsDisabled, OwnerUserId = x.Project.OwnerUserId })
            .FirstOrDefaultAsync();

        if (res.Response is null)
        {
#if DEMO
            _seed_call = true;
#endif
            IQueryable<ProjectConstructorModelDB> members_query = context_forms
                .MembersOfProjects
                .Include(x => x.Project)
                .Select(x => x.Project!);

            project = await context_forms
                .Projects
                .Where(x => x.OwnerUserId == user_id)
                .Union(members_query)
                .FirstAsync();

            project_use = new() { UserId = user_id, ProjectId = project.Id };
            await context_forms.AddAsync(project_use);
            await context_forms.SaveChangesAsync();

            res.Response = MainProjectViewModel.Build(project);
        }
#if DEMO
        if (_seed_call)
        {
            DirectoryConstructorModelDB? _dir_seed_1, _dir_seed_2 = await context_forms.Directories.FirstOrDefaultAsync(x => x.ProjectId == project!.Id);

            if (_dir_seed_2 is null)
            {
                _dir_seed_1 = new()
                {
                    Name = "Preference",
                    ProjectId = project!.Id,
                    Elements = [new() { Name = "One", SortIndex = 1 }, new() { Name = "Two", SortIndex = 2 }, new() { Name = "Three", SortIndex = 3 }]
                };

                _dir_seed_2 = new()
                {
                    Name = "Булево (логическое)",
                    ProjectId = project!.Id,
                    Elements = [new() { Name = "Да", SortIndex = 1 }, new() { Name = "Нет", SortIndex = 2 }]
                };

                await context_forms.AddRangeAsync(_dir_seed_1, _dir_seed_2);
                await context_forms.SaveChangesAsync();
                FormConstructorModelDB? _form_seed_2, _form_seed_1 = await context_forms.Forms.FirstOrDefaultAsync(x => x.ProjectId == project!.Id);
                if (_form_seed_1 is null)
                {
                    _form_seed_1 = new()
                    {
                        Name = "Bootstrap form (demo 1)",
                        ProjectId = project!.Id,
                        Description = "<p>seed data (1) for debug</p>",
                        FieldsDirectoriesLinks = [new FieldFormAkaDirectoryConstructorModelDB() { Name = "Test Directory", DirectoryId = _dir_seed_2.Id, SortIndex = 2 }],
                        Fields = [new FieldFormConstructorModelDB() { Name = "Test text", SortIndex = 1, TypeField = TypesFieldsFormsEnum.Text, Css = "col-12" }],
                    };

                    _form_seed_2 = new()
                    {
                        Name = "Bootstrap form (demo 2)",
                        ProjectId = project!.Id,
                        Description = "<p>seed data (2) for debug</p>",
                        Css = "row g-3",
                        FieldsDirectoriesLinks = [new FieldFormAkaDirectoryConstructorModelDB() { Name = "State", DirectoryId = _dir_seed_1.Id, SortIndex = 6, Css = "col-md-4" }],
                        Fields = [
                            new FieldFormConstructorModelDB() { Name = "Email", SortIndex = 1, TypeField = TypesFieldsFormsEnum.Text, Css = "col-md-6" },
                            new FieldFormConstructorModelDB() { Name = "Password", SortIndex = 2, TypeField = TypesFieldsFormsEnum.Password, Css = "col-md-6" },
                            new FieldFormConstructorModelDB() { Name = "Address", SortIndex = 3, TypeField = TypesFieldsFormsEnum.Text, Css = "col-12", MetadataValueType = JsonConvert.SerializeObject(new Dictionary<MetadataExtensionsFormFieldsEnum, object>{ { MetadataExtensionsFormFieldsEnum.Placeholder, "1234 Main St" } }) },
                            new FieldFormConstructorModelDB() { Name = "Address 2", SortIndex = 4, TypeField = TypesFieldsFormsEnum.Text, Css = "col-12", MetadataValueType = JsonConvert.SerializeObject(new Dictionary<MetadataExtensionsFormFieldsEnum, object>{ { MetadataExtensionsFormFieldsEnum.Placeholder, "Apartment, studio, or floor" } }) },
                            new FieldFormConstructorModelDB() { Name = "City", SortIndex = 5, TypeField = TypesFieldsFormsEnum.Text, Css = "col-md-6" },
                            new FieldFormConstructorModelDB() { Name = "Zip", SortIndex = 7, TypeField = TypesFieldsFormsEnum.Text, Css = "col-md-2" },
                            new FieldFormConstructorModelDB() { Name = "Check me out", SortIndex = 8, TypeField = TypesFieldsFormsEnum.Bool, Css = "col-12" },
                            ],
                    };

                    await context_forms.AddRangeAsync(_form_seed_1, _form_seed_2);
                    await context_forms.SaveChangesAsync();

                    DocumentSchemeConstructorModelDB? _document_scheme_seed = await context_forms.DocumentSchemes.FirstOrDefaultAsync(x => x.ProjectId == project!.Id);

                    if (_document_scheme_seed is null)
                    {
                        _document_scheme_seed = new()
                        {
                            Name = "Demo document",
                            ProjectId = project!.Id,
                            Tabs = [new (){
                                Name = "Demo tab 1",
                                SortIndex = 1,
                                JoinsForms = [new ()
                                {
                                    Name = "join form #1",
                                    FormId = _form_seed_1.Id,
                                    SortIndex = 1
                                },new() {
                                    Name = "join form #2",
                                    FormId = _form_seed_2.Id,
                                    SortIndex = 2
                                }]
                            },new (){
                                Name = "Demo tab 2",
                                SortIndex = 2,
                                JoinsForms = [new ()
                                {
                                    Name = "join form #3",
                                    FormId = _form_seed_1.Id,
                                    SortIndex = 1,
                                    IsTable = true
                                }]
                            }]
                        };

                        await context_forms.AddAsync(_document_scheme_seed);
                        await context_forms.SaveChangesAsync();
                    }

                    SessionOfDocumentDataModelDB? _session_seed = await context_forms.Sessions.FirstOrDefaultAsync(x => x.ProjectId == project!.Id && x.AuthorUser == userDb.Id);

                    if (_session_seed is null)
                    {
                        _session_seed = new()
                        {
                            ProjectId = project!.Id,
                            AuthorUser = userDb.Id,
                            Name = "Debug session",
                            NormalizeUpperName = "DEBUG SESSION",
                            DeadlineDate = DateTime.Now.AddDays(1),
                            OwnerId = _document_scheme_seed.Id,
                            SessionStatus = SessionsStatusesEnum.InProgress,
                            SessionToken = Guid.NewGuid().ToString()
                        };

                        await context_forms.AddAsync(_session_seed);
                        await context_forms.SaveChangesAsync();
                    }
                }
            }
        }
#endif

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CanEditProject(int project_id, string? user_id = null)
    {
        if (project_id < 1)
            return ResponseBaseModel.CreateError("Не указан проект");

        TResponseModel<UserInfoModel?> call_user = await usersProfilesRepo.FindByIdAsync(user_id);

        if (!call_user.Success())
            return ResponseBaseModel.Create(call_user.Messages);

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ProjectConstructorModelDB? project = await context_forms.Projects.FirstOrDefaultAsync(x => x.Id == project_id);
        if (project is null)
            return ResponseBaseModel.CreateError($"Проект #{project_id} не найден в БД");

        return project.CanEdit(call_user.Response!)
            ? ResponseBaseModel.CreateSuccess("Проект доступен для редактирования")
            : ResponseBaseModel.CreateError($"Проект недоступен для редактирования #{project.Id} '{project.Name}'"); ;
    }
    #endregion

    /////////////// Перечисления.
    // Простейший тип данных поля формы, который можно в в последствии использовать в конструкторе форм при добавлении/редактировании полей
    // Примечание: В генераторе для C# .NET формируются как Enum
    #region справочники/списки
    /// <inheritdoc/>
    public async Task<IEnumerable<EntryNestedModel>> ReadDirectories(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        if (!ids.Any())
            return [];
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DirectoryConstructorModelDB[] res = await context_forms
            .Directories
            .Include(x => x.Elements)
            .Where(x => ids.Contains(x.Id)).
            ToArrayAsync(cancellationToken: cancellationToken);

        return res.Select(x => new EntryNestedModel() { Id = x.Id, Name = x.Name, Childs = x.Elements!.Select(y => new EntryModel() { Id = y.Id, Name = y.Name }) });
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<EntryModel[]>> GetDirectories(int project_id, string? name_filter = null, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<EntryModel> query = context_forms
            .Directories
            .Where(x => x.ProjectId == project_id)
            .Select(x => new EntryModel() { Id = x.Id, Name = x.Name })
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name_filter))
            query = query.Where(x => EF.Functions.Like(x.Name.ToUpper(), $"%{name_filter.ToUpper()}%"));

        return new TResponseStrictModel<EntryModel[]>() { Response = await query.ToArrayAsync(cancellationToken: cancellationToken) };
    }

    /// <inheritdoc/>
    public async Task<EntryDescriptionModel> GetDirectory(int enumeration_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        return await context_forms
            .Directories
            .Where(x => x.Id == enumeration_id)
            .Select(x => new EntryDescriptionModel() { Name = x.Name, Id = x.Id, Description = x.Description })
            .FirstAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>> UpdateOrCreateDirectory(EntryConstructedModel _dir, CancellationToken cancellationToken = default)
    {
        _dir.Name = MyRegexSpices().Replace(_dir.Name.Trim(), " ");
        TResponseStrictModel<int> res = new() { Response = 0 };
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(_dir);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        string msg;

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DirectoryConstructorModelDB? directory_db = await context_forms
            .Directories
            .FirstOrDefaultAsync(x => x.Id != _dir.Id && x.ProjectId == _dir.ProjectId && x.Name == _dir.Name, cancellationToken: cancellationToken);

        if (directory_db is not null)
        {
            msg = $"Справочник '{directory_db.Name} уже существует `#{directory_db.Id}`";
            logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        if (_dir.Id < 1)
        {
            ResponseBaseModel check_project = await CanEditProject(_dir.ProjectId);
            if (!check_project.Success())
            {
                res.AddRangeMessages(check_project.Messages);
                return res;
            }

            directory_db = DirectoryConstructorModelDB.Build(_dir);
            await context_forms.AddAsync(directory_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken: cancellationToken);
            res.AddSuccess($"Справочник успешно создан #{res.Response}");
        }
        else
        {
            directory_db = await context_forms
                .Directories
                .FirstOrDefaultAsync(x => x.Id == _dir.Id, cancellationToken: cancellationToken);

            if (directory_db is null)
            {
                msg = $"Справочник #{_dir.Id} не найден в БД";
                logger.LogError(msg);
                res.AddError(msg);
                return res;
            }

            ResponseBaseModel check_project = await CanEditProject(directory_db.ProjectId);
            if (!check_project.Success())
            {
                res.AddRangeMessages(check_project.Messages);
                return res;
            }

            if (directory_db.Name.Equals(_dir.Name) && directory_db.Description == _dir.Description)
            {
                res.AddInfo("Справочник не требует изменения");
            }
            else
            {
                msg = $"Справочник #{_dir.Id} обновлён";
                directory_db.Name = _dir.Name;
                directory_db.Description = _dir.Description;
                context_forms.Update(directory_db);
                await context_forms.SaveChangesAsync(cancellationToken);
                logger.LogInformation(msg);
                res.AddSuccess(msg);
            }
        }

        await context_forms
             .Projects
             .Where(u => u.Id == _dir.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        res.Response = directory_db.Id;
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DirectoryConstructorModelDB? directory_db = await context_forms
            .Directories
            .FirstOrDefaultAsync(x => x.Id == directory_id, cancellationToken: cancellationToken);

        if (directory_db is null)
            return ResponseBaseModel.CreateError($"Список/справочник #{directory_id} не найден в БД");

        ResponseBaseModel check_project = await CanEditProject(directory_db.ProjectId);
        if (!check_project.Success())
            return check_project;

        context_forms.Remove(directory_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == directory_db.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Список/справочник #{directory_id} успешно удалён из БД");
    }
    #endregion
    #region элементы справочникв/списков
    /// <inheritdoc/>
    public async Task<EntryDescriptionModel> GetElementOfDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        return await context_forms
            .ElementsOfDirectories
            .Where(x => x.Id == element_id)
            .Select(x => new EntryDescriptionModel() { Name = x.Name, Id = x.Id, Description = x.Description })
            .FirstAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<List<EntryModel>>> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        TResponseModel<List<EntryModel>> res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DirectoryConstructorModelDB? dir = await context_forms.Directories
            .Include(x => x.Elements)
            .FirstOrDefaultAsync(x => x.Id == directory_id, cancellationToken: cancellationToken);

        if (dir is null)
        {
            res.AddError($"Справочник #{directory_id} не найден в БД");
            return res;
        }

        res.Response = dir.Elements?
            .OrderBy(x => x.SortIndex)
            .Select(x => new EntryModel() { Name = x.Name, Id = x.Id })
            .ToList();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>> CreateElementForDirectory(OwnedNameModel element, CancellationToken cancellationToken = default)
    {
        element.Name = MyRegexSpices().Replace(element.Name, " ").Trim();
        TResponseStrictModel<int> res = new() { Response = 0 };

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(element);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DirectoryConstructorModelDB? directory_db = await context_forms.Directories
            .Include(x => x.Elements)
            .FirstOrDefaultAsync(x => x.Id == element.OwnerId, cancellationToken: cancellationToken);

        if (directory_db is null)
        {
            res.AddError($"Справочник #{element.OwnerId} не найден в БД");
            return res;
        }

        ResponseBaseModel check_project = await CanEditProject(directory_db.ProjectId);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        ElementOfDirectoryConstructorModelDB? dictionary_element_db = await context_forms.ElementsOfDirectories.FirstOrDefaultAsync(x => x.ParentId == element.OwnerId && (x.Name.ToUpper() == element.Name.ToUpper()), cancellationToken: cancellationToken);

        if (dictionary_element_db is not null)
        {
            res.AddError("Такой элемент справочника уже существует");
            return res;
        }

        int[] current_indexes = await context_forms
            .ElementsOfDirectories
            .Where(x => x.ParentId == element.OwnerId)
            .Select(x => x.SortIndex)
            .ToArrayAsync(cancellationToken: cancellationToken);

        int current_index = current_indexes.Length == 0
            ? 0
            : current_indexes.Max();

        dictionary_element_db = new() { Name = element.Name, ParentId = directory_db.Id, Parent = directory_db, SortIndex = current_index + 1 };

        try
        {
            await context_forms.AddAsync(dictionary_element_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            res.AddError($"exception A708CE72-AB9D-430D-8CB0-40C522F2D91A: {ex.Message}");
            return res;
        }

        res.Response = dictionary_element_db.Id;
        res.AddSuccess($"Элемент справочника успешно создан #{res.Response}");

        await context_forms
             .Projects
             .Where(u => u.Id == directory_db.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateElementOfDirectory(EntryDescriptionModel element, CancellationToken cancellationToken = default)
    {
        element.Name = MyRegexSpices().Replace(element.Name, " ").Trim();
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(element);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ElementOfDirectoryConstructorModelDB? element_db = await context_forms
            .ElementsOfDirectories
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == element.Id, cancellationToken: cancellationToken);

        if (element_db?.Parent is null)
        {
            res.AddError($"Элемент справочника #{element.Id} не найден в БД");
            return res;
        }

        ResponseBaseModel check_project = await CanEditProject(element_db.Parent.ProjectId);
        if (!check_project.Success())
            return check_project;

        if (element_db.Name.Equals(element.Name) && element_db.Description?.Equals(element.Description) == true)
        {
            res.AddInfo("Изменений нет - обновления не требуется");
            return res;
        }

        IQueryable<ElementOfDirectoryConstructorModelDB> duplicate_check_query = context_forms
            .ElementsOfDirectories
            .Where(x => x.Id != element.Id && x.ParentId == element_db.ParentId && (x.Name.ToUpper() == element.Name.ToUpper()));

        if (await duplicate_check_query.AnyAsync(cancellationToken: cancellationToken))
        {
            res.AddError("Элемент с таким именем уже существует в справочнике");
            return res;
        }

        element_db.Name = element.Name;
        element_db.Description = element.Description;

        context_forms.Update(element_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        res.AddSuccess("Элемент справочника успешно обновлён");

        await context_forms
             .Projects
             .Where(u => u.Id == element_db.Parent.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteElementFromDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ElementOfDirectoryConstructorModelDB? element_db = await context_forms
            .ElementsOfDirectories
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);

        if (element_db?.Parent is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");

        ResponseBaseModel check_project = await CanEditProject(element_db.Parent.ProjectId);
        if (!check_project.Success())
            return check_project;

        context_forms.Remove(element_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == element_db.Parent.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Элемент справочника #{element_id} успешно удалён из БД");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpMoveElementOfDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ElementOfDirectoryConstructorModelDB? element_db = await context_forms
            .ElementsOfDirectories
            .Include(x => x.Parent)
            .ThenInclude(x => x!.Elements)
            .FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);

        if (element_db?.Parent?.Elements is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");

        ResponseBaseModel check_project = await CanEditProject(element_db.Parent.ProjectId);
        if (!check_project.Success())
            return check_project;

        element_db.Parent.Elements = [.. element_db.Parent.Elements.OrderBy(x => x.SortIndex)];

        int i = element_db.Parent!.Elements!.FindIndex(x => x.Id == element_id);
        if (i == 0)
            return ResponseBaseModel.CreateWarning($"Элемент '{element_db.Name}' не может быть выше. Он в крайнем положении");

        element_db.Parent!.Elements![i].SortIndex--;
        element_db.Parent!.Elements![i - 1].SortIndex++;

        context_forms.UpdateRange(new ElementOfDirectoryConstructorModelDB[] { element_db.Parent!.Elements![i], element_db.Parent!.Elements![i - 1] });
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == element_db.Parent.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess("Элемент сдвинут");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DownMoveElementOfDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ElementOfDirectoryConstructorModelDB? element_db = await context_forms
            .ElementsOfDirectories
            .Include(x => x.Parent)
            .ThenInclude(x => x!.Elements)
            .FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);

        if (element_db?.Parent?.Elements is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");

        ResponseBaseModel check_project = await CanEditProject(element_db.Parent.ProjectId);
        if (!check_project.Success())
            return check_project;

        element_db.Parent.Elements = [.. element_db.Parent.Elements.OrderBy(x => x.SortIndex)];

        int i = element_db.Parent!.Elements!.FindIndex(x => x.Id == element_id);
        if (i == element_db.Parent!.Elements!.Count - 1)
            return ResponseBaseModel.CreateWarning($"Элемент '{element_db.Name}' не может быть ниже. Он в крайнем положении");

        element_db.Parent!.Elements![i].SortIndex++;
        element_db.Parent!.Elements![i + 1].SortIndex--;

        context_forms.UpdateRange(new ElementOfDirectoryConstructorModelDB[] { element_db.Parent!.Elements![i], element_db.Parent!.Elements![i + 1] });
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == element_db.Parent.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess("Элемент сдвинут");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CheckAndNormalizeSortIndexForElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        List<ElementOfDirectoryConstructorModelDB> elements = await context_forms
            .ElementsOfDirectories
            .Where(x => x.ParentId == directory_id)
            .ToListAsync(cancellationToken: cancellationToken);

        if (elements.Count == 0)
            return ResponseBaseModel.CreateWarning("Элементов в справочнике нет");

        bool need_update = false;
        int i = 1;
        elements.ForEach(x =>
        {
            need_update = need_update || x.SortIndex != i;
            x.SortIndex = i;
            i++;
        });

        if (need_update)
        {
            context_forms.UpdateRange(elements);
            await context_forms.SaveChangesAsync(cancellationToken);
            return ResponseBaseModel.CreateSuccess("Сортировка обновлена");
        }

        return ResponseBaseModel.CreateInfo("Обновления не потребовались. Сортировка в порядке.");
    }
    #endregion

    /////////////// Формы для редактирования/добавления бизнес-сущностей внутри итогового документа.
    // Базовая бизнес-сущность описывающая каркас/строение данных. Можно сравнить с таблицей БД со своим набором полей/колонок
    // К тому же сразу настраивается web-форма для редактирования объекта данного типа. Возможность устанавливать css стили формам и полям (с умыслом использования возможностей Bootstrap)
    // Тип данных для полей форм может быть любой из перечня доступных: перечисление (созданное вами же), строка, число, булево, дата и т.д.
    #region формы
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<FormConstructorModelDB>> SelectForms(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TPaginationResponseModel<FormConstructorModelDB> res = new(req);

        IQueryable<FormConstructorModelDB> q;

        q = context_forms.Forms.Where(x => x.ProjectId == projectId).OrderBy(x => x.Name);

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
        {
            q = (from _form in q
                 join _field in context_forms.Fields on _form.Id equals _field.OwnerId into ps_field
                 from field in ps_field.DefaultIfEmpty()
                 where
                 EF.Functions.Like(_form.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") ||
                 EF.Functions.Like(field.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%")
                 group _form by _form into g
                 select g.Key)
             .OrderBy(x => x.Name)
            .AsQueryable();
        }

        res.TotalRowsCount = await q.CountAsync(cancellationToken: cancellationToken);
        q = q.OrderBy(x => x.Id).Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        int[] ids = await q.Select(x => x.Id).ToArrayAsync(cancellationToken: cancellationToken);

        res.Response = ids.Length != 0
            ? await context_forms.Forms.Where(x => ids.Contains(x.Id)).Include(x => x.Fields).Include(x => x.FieldsDirectoriesLinks).ToListAsync(cancellationToken: cancellationToken)
            : [];
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> GetForm(int form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<FormConstructorModelDB> res = new()
        {
            Response = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FieldsDirectoriesLinks!)
            .ThenInclude(x => x.Directory)
            .FirstOrDefaultAsync(x => x.Id == form_id, cancellationToken: cancellationToken)
        };

        if (res.Response is null)
            res.AddError($"Форма #{form_id} не найдена в БД");
        else
        {
            if (res.Response.Fields is not null)
            {
                string msg;
                List<FieldFormConstructorModelDB> _upd = [];
                Dictionary<MetadataExtensionsFormFieldsEnum, object?> mdvs;
                foreach (FieldFormConstructorModelDB f in res.Response.Fields)
                {
                    if (f.MetadataValueType?.Equals("{}") == true)
                    {
                        f.MetadataValueType = default;
                        if (!_upd.Any(x => x.Id == f.Id))
                            _upd.Add(f);
                    }
                    else
                    {
                        mdvs = f.MetadataValueTypeGet();
                        foreach (KeyValuePair<MetadataExtensionsFormFieldsEnum, object?> md in mdvs)
                        {
                            if (string.IsNullOrWhiteSpace(md.Value?.ToString()))
                            {
                                f.UnsetValueOfMetadata(md.Key);
                                msg = $"Удалена пустая опция {md.Key} из поля ['{f.Name}' #{f.Id}] формы ['{f.Owner?.Name}' #{f.Owner?.Id}]";
                                res.AddInfo(msg);
                                logger.LogInformation(msg);
                                if (!_upd.Any(x => x.Id == f.Id))
                                    _upd.Add(f);
                            }
                        }
                    }

                }
                if (_upd.Count != 0)
                {
                    context_forms.UpdateRange(_upd);
                    await context_forms.SaveChangesAsync(cancellationToken);
                }
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FormUpdateOrCreate(FormBaseConstructorModel form, CancellationToken cancellationToken = default)
    {
        TResponseModel<FormConstructorModelDB> res = new();

        form.Name = MyRegexSpices().Replace(form.Name, " ").Trim();
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(form);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
#pragma warning disable CA1862 // Используйте перегрузки метода "StringComparison" для сравнения строк без учета регистра
        FormConstructorModelDB? form_db = await context_forms
            .Forms
            .FirstOrDefaultAsync(x => x.Id != form.Id && x.ProjectId == form.ProjectId && x.Name.ToUpper() == form.Name.ToUpper(), cancellationToken: cancellationToken);
#pragma warning restore CA1862 // Используйте перегрузки метода "StringComparison" для сравнения строк без учета регистра

        string msg;
        if (form_db is not null)
        {
            msg = $"Такая форма уже существует: #{form_db.Id} '{form_db.Name}'";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        ResponseBaseModel check_project;
        if (form.Id < 1)
        {
            check_project = await CanEditProject(form.ProjectId);
            if (!check_project.Success())
            {
                res.AddRangeMessages(check_project.Messages);
                return res;
            }

            form_db = FormConstructorModelDB.Build(form);
            await context_forms.AddAsync(form_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Форма создана #{form_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.Response = form_db;
            return res;
        }

        form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FieldsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == form.Id, cancellationToken: cancellationToken);

        if (form_db is null)
        {
            msg = $"Форма #{form.Id} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        check_project = await CanEditProject(form_db.ProjectId);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        if (form_db.Name.Equals(form.Name) && form_db.Description == form.Description && form_db.Css == form.Css && form_db.AddRowButtonTitle == form.AddRowButtonTitle)
        {
            msg = $"Форма #{form.Id} не требует изменений в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
        }
        else
        {
            form_db.Name = form.Name;
            form_db.Description = form.Description;
            form_db.Css = form.Css;
            form_db.AddRowButtonTitle = form.AddRowButtonTitle;
            context_forms.Update(form_db);
            msg = $"Поле (простой тип) формы #{form.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        res.Response = form_db;
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormDelete(int form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        FormConstructorModelDB? form_db = await context_forms
            .Forms
            .FirstOrDefaultAsync(x => x.Id == form_id, cancellationToken: cancellationToken);

        if (form_db is null)
            return ResponseBaseModel.CreateError($"Форма #{form_id} не найдена в БД");

        ResponseBaseModel check_project = await CanEditProject(form_db.ProjectId);
        if (!check_project.Success())
            return check_project;

        await context_forms
             .Projects
             .Where(u => u.Id == form_db.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateError("Метод не реализован");
    }
    #endregion
    #region поля форм
    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FieldFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<FormConstructorModelDB> res = new();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        FieldFormConstructorModelDB? field_db = await context_forms
            .Fields
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Fields)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.FieldsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_id, cancellationToken: cancellationToken);

        if (field_db?.Owner?.Fields is null || field_db?.Owner?.FieldsDirectoriesLinks is null)
        {
            res.AddError($"Поле формы (простого типа) #{field_id} не найден в БД. ошибка D4B94965-1C93-478E-AC8A-8F75C0D6455E");
            return res;
        }

        ResponseBaseModel check_project = await CanEditProject(field_db.Owner.ProjectId);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        if ((field_db.SortIndex <= 1 && direct == VerticalDirectionsEnum.Up) || (field_db.SortIndex >= field_db.Owner!.AllFields.Count() && direct == VerticalDirectionsEnum.Down))
        {
            res.AddWarning($"Поле формы (простого типа) #{field_id} не может быть перемещено: оно уже в крайнем положении. ошибка D46E662C-F643-467E-9EDC-528B0674C66A");
            return res;
        }

        int next_index = field_db.SortIndex + (direct == VerticalDirectionsEnum.Up ? -1 : 1);

        if (field_db.Owner.Fields!.Any(x => x.SortIndex == next_index))
        {
            FieldFormConstructorModelDB _fns = field_db.Owner.Fields!.First(x => x.SortIndex == next_index);
            res.AddInfo($"Поля формы меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' (простой тип) && #{field_db.Id} i:{field_db.SortIndex} '{field_db.Name}' (простой тип)");

            _fns.SortIndex = field_db.SortIndex;
            field_db.SortIndex = next_index;
            context_forms.Update(field_db);
            context_forms.Update(_fns);
        }
        else if (field_db.Owner.FieldsDirectoriesLinks!.Any(x => x.SortIndex == next_index))
        {
            FieldFormAkaDirectoryConstructorModelDB _fnsd = field_db.Owner.FieldsDirectoriesLinks!.First(x => x.SortIndex == next_index);
            res.AddInfo($"Поля формы меняются индексами сортировки: #{_fnsd.Id} i:{_fnsd.SortIndex} '{_fnsd.Name}' (тип: справочник) && #{field_db.Id} i:{field_db.SortIndex} '{field_db.Name}' (простой тип)");

            _fnsd.SortIndex = field_db.SortIndex;
            field_db.SortIndex = next_index;
            context_forms.Update(field_db);
            context_forms.Update(_fnsd);
        }
        else
            res.AddError("Не удалось выполнить перемещение. ошибка 24D882A6-8A0E-44B6-BFAD-4916A7372582");

        if (res.Success())
        {
            await context_forms.SaveChangesAsync(cancellationToken);
            res.Response = await context_forms
                .Forms
                .Include(x => x.Fields)
                .Include(x => x.FieldsDirectoriesLinks)
                .FirstAsync(x => x.Id == field_db.OwnerId, cancellationToken: cancellationToken);
        }

        await context_forms
             .Projects
             .Where(u => u.Id == field_db.Owner.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> FieldDirectoryFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<FormConstructorModelDB> res = new();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        FieldFormAkaDirectoryConstructorModelDB? field_db = await context_forms
            .LinksDirectoriesToForms
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Fields)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.FieldsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_id, cancellationToken: cancellationToken);

        if (field_db?.Owner is null)
        {
            res.AddError($"Поле формы (тип: справочник) #{field_id} не найден в БД. ошибка 39253612-8DD9-40B3-80AA-CF6589288E06");
            return res;
        }

        ResponseBaseModel check_project = await CanEditProject(field_db.Owner.ProjectId);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        if ((field_db.SortIndex <= 1 && direct == VerticalDirectionsEnum.Up) || (field_db.SortIndex >= field_db.Owner!.AllFields.Count() && direct == VerticalDirectionsEnum.Down))
        {
            res.AddWarning($"Поле формы (тип: справочник) #{field_id} не может быть перемещено: оно уже в крайнем положении. ошибка 4DA195B0-F0B1-43AB-96F5-F282CB74FFF5");
            return res;
        }

        int next_index = field_db.SortIndex + (direct == VerticalDirectionsEnum.Up ? -1 : 1);

        if (field_db.Owner.Fields!.Any(x => x.SortIndex == next_index))
        {
            FieldFormConstructorModelDB _fns = field_db.Owner.Fields!.First(x => x.SortIndex == next_index);
            res.AddInfo($"Поля формы меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' (простой тип) && #{field_db.Id} i:{field_db.SortIndex} '{field_db.Name}' (тип: справочник)");

            _fns.SortIndex = field_db.SortIndex;
            field_db.SortIndex = next_index;
            context_forms.Update(field_db);
            context_forms.Update(_fns);
        }
        else if (field_db.Owner.FieldsDirectoriesLinks!.Any(x => x.SortIndex == next_index))
        {
            FieldFormAkaDirectoryConstructorModelDB _fnsd = field_db.Owner.FieldsDirectoriesLinks!.First(x => x.SortIndex == next_index);
            res.AddInfo($"Поля формы меняются индексами сортировки: #{_fnsd.Id} i:{_fnsd.SortIndex} '{_fnsd.Name}' (тип: справочник) && #{field_db.Id} i:{field_db.SortIndex} '{field_db.Name}' (тип: справочник)");

            _fnsd.SortIndex = field_db.SortIndex;
            field_db.SortIndex = next_index;
            context_forms.Update(field_db);
            context_forms.Update(_fnsd);
        }
        else
            res.AddError("Не удалось выполнить перемещение. ошибка 4DC24323-5F46-4EC5-AD6B-3C57B7400883");

        if (res.Success())
        {
            await context_forms.SaveChangesAsync(cancellationToken);
            res.Response = await context_forms
                .Forms
                .Include(x => x.Fields)
                .Include(x => x.FieldsDirectoriesLinks)
                .FirstAsync(x => x.Id == field_db.OwnerId, cancellationToken: cancellationToken);
        }

        await context_forms
             .Projects
             .Where(u => u.Id == field_db.Owner.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldUpdateOrCreate(FieldFormBaseConstructorModel form_field, CancellationToken cancellationToken = default)
    {
        form_field.Name = MyRegexSpices().Replace(form_field.Name, " ").Trim();
        form_field.MetadataValueType = form_field.MetadataValueType?.Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(form_field);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        FormConstructorModelDB? form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FieldsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == form_field.OwnerId, cancellationToken: cancellationToken);

        string msg;
        if (form_db?.Fields is null || form_db.FieldsDirectoriesLinks is null)
        {
            msg = $"Форма #{form_field.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (form_field.Id > 0 && !form_db.Fields.Any(x => x.Id == form_field.Id))
            return ResponseBaseModel.CreateError($"Поле (#{form_field.Id} простого типа) для формы #{form_db.Id} не найдено в БД, либо поле принадлежит другой форме или относится к иному виду поля");

        ResponseBaseModel check_project = await CanEditProject(form_db.ProjectId);
        if (!check_project.Success())
            return check_project;

        FieldFormBaseLowConstructorModel? duplicate_field = form_db.AllFields.FirstOrDefault(x => (x.GetType() == typeof(FieldFormAkaDirectoryConstructorModelDB) && x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase)) || (x.GetType() == typeof(FieldFormConstructorModelDB)) && x.Id != form_field.Id && x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase));
        if (duplicate_field is not null)
            return ResponseBaseModel.CreateError($"Поле с таким именем уже существует: '{duplicate_field.Name}'");

        FieldFormConstructorModelDB? form_field_db;
        if (form_field.Id < 1)
        {
            int _sort_index = form_db.FieldsDirectoriesLinks.Count != 0
                ? form_db.FieldsDirectoriesLinks.Max(x => x.SortIndex)
                : 0;

            _sort_index = Math.Max(_sort_index, form_db.Fields.Count != 0 ? form_db.Fields.Max(x => x.SortIndex) : 0);

            form_field_db = FieldFormConstructorModelDB.Build(form_field, form_db, _sort_index + 1);

            await context_forms.AddAsync(form_field_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Поле (простого типа) создано #{form_field_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        form_field_db = form_db.Fields.First(x => x.Id == form_field.Id);

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context_forms.Database.BeginTransaction(isolationLevel: System.Data.IsolationLevel.Serializable);

        List<ValueDataForSessionOfDocumentModelDB> values_updates = await (from val_s in context_forms.ValuesSessions.Where(x => x.Name == form_field.Name)
                                                                           join session in context_forms.Sessions on val_s.OwnerId equals session.Id
                                                                           join DocumentScheme in context_forms.DocumentSchemes on session.OwnerId equals DocumentScheme.Id
                                                                           join page in context_forms.TabsOfDocumentsSchemes on DocumentScheme.Id equals page.OwnerId
                                                                           join form_join in context_forms.TabsJoinsForms.Where(x => x.FormId == form_db.Id) on page.Id equals form_join.TabId
                                                                           select val_s)
                                                                     .ToListAsync(cancellationToken: cancellationToken);

        if (form_field_db.TypeField != form_field.TypeField)
        {
            if (values_updates.Count != 0)
            {
                msg = $"Тип поля (простого типа) формы #{form_field.Id} не может быть изменён ([{form_field_db.TypeField}] -> [{form_field.TypeField}]): для этого поля найдены ссылки введённых значений ({values_updates.Count} штук).";
                res.AddError(msg);
                logger.LogError(msg);
                return res;
            }

            if (form_field.TypeField == TypesFieldsFormsEnum.ProgramCalculationDouble && form_field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, null) is null)
            {
                msg = $"Для калькуляции требуются параметры с именами полей. Пример: ['ИмяПоля1', 'ИмяПоля2', 'ИмяПоля3']";
                res.AddError(msg);
                logger.LogError(msg);
                return res;
            }

            msg = $"Тип поля (простого типа) формы #{form_field.Id} изменился: [{form_field_db.TypeField}] -> [{form_field.TypeField}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.TypeField = form_field.TypeField;
        }

        if (form_field_db.Name != form_field.Name)
        {
            msg = $"Название поля (простого типа) формы #{form_field.Id} изменилось: [{form_field_db.Name}] -> [{form_field.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Name = form_field.Name;

            if (values_updates.Count != 0)
            {
                values_updates.ForEach(x => { x.Name = form_field.Name; });
                context_forms.UpdateRange(values_updates);
            }
        }

        if (form_field_db.Description != form_field.Description)
        {
            msg = $"Описание поля (простого типа) формы #{form_field.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Description = form_field.Description;
        }
        if (form_field_db.Hint != form_field.Hint)
        {
            msg = $"Подсказка поля (простого типа) формы #{form_field.Id} изменилась: [{form_field_db.Hint}] -> [{form_field.Hint}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Hint = form_field.Hint;
        }
        if (form_field_db.Css != form_field.Css)
        {
            msg = $"CSS (простого типа) формы #{form_field.Id} изменилась: [{form_field_db.Css}] -> [{form_field.Css}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Css = form_field.Css;
        }

        if (form_field_db.MetadataValueType != form_field.MetadataValueType)
        {
            msg = $"Метаданные поля (простого типа) формы #{form_field.Id} изменились: [{form_field_db.MetadataValueType}] -> [{form_field.MetadataValueType}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.MetadataValueType = form_field.MetadataValueType;
        }

        if (form_field_db.Required != form_field.Required)
        {
            msg = $"Признак [Required] поля (простого типа) формы #{form_field.Id} изменился: [{form_field_db.Required}] -> [{form_field.Required}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Required = form_field.Required;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            context_forms.Update(form_field_db);
            msg = $"Поле (простого типа) формы #{form_field.Id} обновлено в БД";
            try
            {
                await context_forms.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                logger.LogInformation(msg);
            }
            catch (Exception ex)
            {
                msg = $"Ошибка обновления поля #{form_field_db.Id} '{form_field_db.Name}' (форма #{form_db.Id} '{form_db.Name}')";
                res.AddError(msg);
                res.Messages.InjectException(ex);
                logger.LogError(ex, msg);
            }
        }

        await context_forms
             .Projects
             .Where(u => u.Id == form_db.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(FieldFormAkaDirectoryConstructorModelDB field_directory, CancellationToken cancellationToken = default)
    {
        field_directory.Name = MyRegexSpices().Replace(field_directory.Name, " ").Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(field_directory);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        DirectoryConstructorModelDB? directory_db = await context_forms
            .Directories
            .FirstOrDefaultAsync(x => x.Id == field_directory.DirectoryId, cancellationToken: cancellationToken);

        if (directory_db is null)
            return ResponseBaseModel.CreateError($"Не найден справочник/список #{field_directory.DirectoryId}");

        FormConstructorModelDB? form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FieldsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_directory.OwnerId, cancellationToken: cancellationToken);

        string msg;
        if (form_db?.Fields is null || form_db.FieldsDirectoriesLinks is null)
        {
            msg = $"Форма #{field_directory.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (field_directory.Id > 0 && !form_db.FieldsDirectoriesLinks.Any(x => x.Id == field_directory.Id))
            return ResponseBaseModel.CreateError($"Поле (списочного типа #{field_directory.Id}) для формы #{form_db.Id} не найдено в БД, либо принадлежит другой форме или относится к иному виду поля");

        ResponseBaseModel check_project = await CanEditProject(form_db.ProjectId);
        if (!check_project.Success())
            return check_project;

        FieldFormBaseLowConstructorModel? duplicate_field = form_db.AllFields.FirstOrDefault(x => (x.GetType() == typeof(FieldFormAkaDirectoryConstructorModelDB) && x.Id != field_directory.Id && x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase)) || (x.GetType() == typeof(FieldFormConstructorModelDB)) && x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase));
        if (duplicate_field is not null)
            return ResponseBaseModel.CreateError($"Поле с таким именем уже существует: '{duplicate_field.Name}'");

        FieldFormAkaDirectoryConstructorModelDB? form_field_db;
        if (field_directory.Id < 1)
        {
            int _sort_index = form_db.FieldsDirectoriesLinks.Count != 0 ? form_db.FieldsDirectoriesLinks.Max(x => x.SortIndex) : 0;
            _sort_index = Math.Max(_sort_index, form_db.Fields.Count != 0 ? form_db.Fields.Max(x => x.SortIndex) : 0);

            form_field_db = new()
            {
                IsMultiSelect = field_directory.IsMultiSelect,
                Name = field_directory.Name,
                Css = field_directory.Css,
                OwnerId = field_directory.OwnerId,
                Hint = field_directory.Hint,
                Required = field_directory.Required,
                Owner = form_db,
                Description = field_directory.Description,
                Directory = directory_db,
                DirectoryId = directory_db.Id,
                SortIndex = _sort_index + 1,
            };
            await context_forms.AddAsync(form_field_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Поле (списочного типа) создано #{form_field_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        form_field_db = form_db.FieldsDirectoriesLinks.First(x => x.Id == field_directory.Id);

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context_forms.Database.BeginTransaction(isolationLevel: System.Data.IsolationLevel.Serializable);

        List<ValueDataForSessionOfDocumentModelDB> values_updates = await (from val_s in context_forms.ValuesSessions.Where(x => x.Name == form_field_db.Name)
                                                                           join session in context_forms.Sessions on val_s.OwnerId equals session.Id
                                                                           join DocumentScheme in context_forms.DocumentSchemes on session.OwnerId equals DocumentScheme.Id
                                                                           join page in context_forms.TabsOfDocumentsSchemes on DocumentScheme.Id equals page.OwnerId
                                                                           join form_join in context_forms.TabsJoinsForms.Where(x => x.FormId == form_db.Id) on page.Id equals form_join.TabId
                                                                           select val_s)
                                                                     .ToListAsync(cancellationToken: cancellationToken);

        if (form_field_db.DirectoryId != field_directory.DirectoryId)
        {
            if (values_updates.Count != 0)
            {
                msg = $"Тип поля (списочного типа) формы #{field_directory.Id} не может быть изменён ([{form_field_db.DirectoryId}] -> [{field_directory.DirectoryId}]): найдены ссылки введённых значений ({values_updates.Count} штук) для этого поля";
                res.AddError(msg);
                logger.LogError(msg);
                return res;
            }

            msg = $"Подтип поля (списочного типа) формы #{field_directory.Id} изменился: [{form_field_db.DirectoryId}] -> [{field_directory.DirectoryId}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.DirectoryId = field_directory.DirectoryId;
        }

        if (form_field_db.Name != field_directory.Name)
        {
            if (values_updates.Count != 0)
            {
                values_updates.ForEach(x => { x.Name = field_directory.Name; });
                context_forms.UpdateRange(values_updates);
            }

            msg = $"Название поля (списочного типа) формы #{field_directory.Id} изменилось: [{form_field_db.Name}] -> [{field_directory.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Name = field_directory.Name;
        }
        if (form_field_db.IsMultiSelect != field_directory.IsMultiSelect)
        {
            msg = $"Multi признак поля (списочного типа) формы #{field_directory.Id} изменилось: [{form_field_db.IsMultiSelect}] -> [{field_directory.IsMultiSelect}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.IsMultiSelect = field_directory.IsMultiSelect;
        }
        if (form_field_db.Css != field_directory.Css)
        {
            msg = $"CSS поля (списочного типа) формы #{field_directory.Id} изменилось: [{form_field_db.Css}] -> [{field_directory.Css}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Css = field_directory.Css;
        }
        if (form_field_db.Description != field_directory.Description)
        {
            msg = $"Описание поля (списочного типа) формы #{field_directory.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Description = field_directory.Description;
        }
        if (form_field_db.Hint != field_directory.Hint)
        {
            msg = $"Подсказка поля (списочного типа) формы #{field_directory.Id} изменилась: [{form_field_db.Hint}] -> [{field_directory.Hint}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Hint = field_directory.Hint;
        }
        if (form_field_db.Required != field_directory.Required)
        {
            msg = $"Признак [{nameof(form_field_db.Required)}] поля (списочного типа) формы #{field_directory.Id} изменился: [{form_field_db.Required}] -> [{field_directory.Required}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Required = field_directory.Required;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            msg = $"Поле (списочного типа) формы #{field_directory.Id} обновлено в БД";
            context_forms.Update(form_field_db);

            try
            {
                await context_forms.SaveChangesAsync(cancellationToken);
                transaction.Commit();
                logger.LogInformation(msg);
            }
            catch (Exception ex)
            {
                msg = $"Ошибка обновления поля #{form_field_db.Id} '{form_field_db.Name}' (форма #{form_db.Id} '{form_db.Name}')";
                res.AddError(msg);
                res.Messages.InjectException(ex);
                logger.LogError(ex, msg);
            }
        }

        await context_forms
             .Projects
             .Where(u => u.Id == form_db.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        await CheckAndNormalizeSortIndexFrmFields(form_field_db.OwnerId, cancellationToken);
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDelete(int form_field_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        FieldFormConstructorModelDB? field_db = await context_forms
            .Fields
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == form_field_id, cancellationToken: cancellationToken);

        if (field_db?.Owner is null)
            return ResponseBaseModel.CreateError($"Поле #{form_field_id} (простого типа) формы не найден в БД");

        ResponseBaseModel check_project = await CanEditProject(field_db.Owner.ProjectId);
        if (!check_project.Success())
            return check_project;

        IQueryable<ValueDataForSessionOfDocumentModelDB> values = from _v in context_forms.ValuesSessions.Where(x => x.Name == field_db.Name)
                                                                  join _jf in context_forms.TabsJoinsForms.Where(x => x.FormId == field_db.OwnerId) on _v.TabJoinDocumentSchemeId equals _jf.Id
                                                                  select _v;

        if (await values.AnyAsync(cancellationToken: cancellationToken))
            context_forms.RemoveRange(values);

        context_forms.Remove(field_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == field_db.Owner.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        await CheckAndNormalizeSortIndexFrmFields(field_db.OwnerId, cancellationToken);
        return ResponseBaseModel.CreateSuccess($"Поле '{field_db.Name}' {{простого типа}} удалено из формы");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryDelete(int field_directory_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        FieldFormAkaDirectoryConstructorModelDB? field_db = await context_forms
            .LinksDirectoriesToForms
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == field_directory_id, cancellationToken: cancellationToken);

        if (field_db?.Owner is null)
            return ResponseBaseModel.CreateError($"Поле #{field_directory_id} (тип: справочник) формы не найден в БД");

        ResponseBaseModel check_project = await CanEditProject(field_db.Owner.ProjectId);
        if (!check_project.Success())
            return check_project;

        context_forms.Remove(field_db);

        IQueryable<ValueDataForSessionOfDocumentModelDB> values = from _v in context_forms.ValuesSessions.Where(x => x.Name == field_db.Name)
                                                                  join _jf in context_forms.TabsJoinsForms.Where(x => x.FormId == field_db.OwnerId) on _v.TabJoinDocumentSchemeId equals _jf.Id
                                                                  select _v;

        if (await values.AnyAsync(cancellationToken: cancellationToken))
            context_forms.RemoveRange(values);

        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == field_db.Owner.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess("Поле {справочник/список} удалено из формы");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FormConstructorModelDB>> CheckAndNormalizeSortIndexFrmFields(int form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        IIncludableQueryable<FormConstructorModelDB, DirectoryConstructorModelDB?> _q = context_forms
            .Forms
            .Where(x => x.Id == form_id)
            .Include(x => x.Fields)
            .Include(x => x.FieldsDirectoriesLinks!)
            .ThenInclude(x => x.Directory);

        FormConstructorModelDB form = _q.First();
        TResponseModel<FormConstructorModelDB> res = new();
        int i = 0;
        List<FieldFormAkaDirectoryConstructorModelDB> fields_dir = [];
        List<FieldFormConstructorModelDB> fields_st = [];

        foreach (FieldFormBaseLowConstructorModel fb in form.AllFields)
        {
            i++;
            if (fb is FieldFormAkaDirectoryConstructorModelDB fs)
            {
                if (i != fs.SortIndex)
                {
                    res.AddWarning($"Исправление пересортицы '{fs.Name}': [{fs.SortIndex}] -> [{i}]");
                    fs.SortIndex = i;
                    fields_dir.Add(fs);
                }
            }
            else if (fb is FieldFormConstructorModelDB fd)
            {
                if (i != fd.SortIndex)
                {
                    res.AddWarning($"Исправление пересортицы '{fd.Name}': [{fd.SortIndex}] -> [{i}]");
                    fd.SortIndex = i;
                    fields_st.Add(fd);
                }
            }
        }

        if (fields_st.Count != 0)
            context_forms.UpdateRange(fields_st);
        if (fields_dir.Count != 0)
            context_forms.UpdateRange(fields_dir);

        if (fields_st.Count != 0 || fields_dir.Count != 0)
            await context_forms.SaveChangesAsync(cancellationToken);
        else
            res.AddInfo("Пересортица отсутствует");

        res.Response = await _q.FirstAsync(cancellationToken: cancellationToken);

        return res;
    }
    #endregion

    /////////////// Документ. Описывается/настраивается конечный результат, который будет использоваться.
    // Может содержать одну или несколько вкладок/табов. На каждом табе/вкладке может располагаться одна или больше форм
    // Каждая располагаемая форма может быть помечена как [Табличная]. Т.е. пользователь будет добавлять сколь угодно строк одной и той же формы.
    // Пользователь при добавлении/редактировании строк таблицы будет видеть форму, которую вы настроили для этого, а внутри таба это будет выглядеть как обычная многострочная таблица с колонками, равными полям формы
    #region схема документа
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<DocumentSchemeConstructorModelDB>> RequestDocumentsSchemes(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken)
    {
        if (req.PageSize < 1)
            req.PageSize = 10;

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TPaginationResponseModel<DocumentSchemeConstructorModelDB> res = new(req);

        IQueryable<DocumentSchemeConstructorModelDB> query = context_forms
            .DocumentSchemes
            .Where(x => x.ProjectId == projectId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
        {
            query = (from _quest in query
                     join _page in context_forms.TabsOfDocumentsSchemes on _quest.Id equals _page.OwnerId into j_pages
                     from j_page in j_pages.DefaultIfEmpty()
                     join _join_form in context_forms.TabsJoinsForms on j_page.Id equals _join_form.TabId into j_join_forms
                     from j_join_form in j_join_forms.DefaultIfEmpty()
                     join _form in context_forms.Forms on j_join_form.FormId equals _form.Id into j_forms
                     from j_form in j_forms.DefaultIfEmpty()
                     where EF.Functions.Like(_quest.Name.ToUpper(), $"%{req.SimpleRequest.ToUpper()}%") || EF.Functions.Like(j_form.Name.ToUpper(), $"%{req.SimpleRequest.ToUpper()}%") || EF.Functions.Like(j_page.Name.ToUpper(), $"%{req.SimpleRequest.ToUpper()}%")
                     group _quest by _quest into g
                     select g.Key)
                        .AsQueryable();
        }

        //IQueryable<DocumentShemaModelDB> query =
        //    (from _quest in context_forms.Documents.Where(x => x.ProjectId == projectId)
        //     join _page in context_forms.DocumentsPages on _quest.Id equals _page.OwnerId
        //     join _join_form in context_forms.DocumentsPagesJoinForms on _page.Id equals _join_form.OwnerId
        //     join _form in context_forms.Forms on _join_form.FormId equals _form.Id
        //     where string.IsNullOrWhiteSpace(req.SimpleRequest) || EF.Functions.Like(_form.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") || EF.Functions.Like(_quest.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") || EF.Functions.Like(_page.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%")
        //     group _quest by _quest into g
        //     select g.Key)
        //    .AsQueryable();

        res.TotalRowsCount = await query.CountAsync(cancellationToken: cancellationToken);
        query = query.OrderBy(x => x.Id).Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        int[] ids = await query.Select(x => x.Id).ToArrayAsync(cancellationToken: cancellationToken);
        query = context_forms.DocumentSchemes.Include(x => x.Tabs).Where(x => ids.Contains(x.Id)).Include(x => x.Tabs).OrderBy(x => x.Name);
        res.Response = ids.Length != 0
            ? await query.ToListAsync(cancellationToken: cancellationToken)
            : [];

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> GetDocumentScheme(int document_scheme_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<DocumentSchemeConstructorModelDB> res = new()
        {
            Response = await context_forms
            .DocumentSchemes
            .Include(x => x.Tabs!)
            .ThenInclude(x => x.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == document_scheme_id, cancellationToken: cancellationToken)
        };

        if (res.Response is null)
            res.AddError($"Опрос/анкета #{document_scheme_id} отсутствует в БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> UpdateOrCreateDocumentScheme(EntryConstructedModel documentScheme, CancellationToken cancellationToken = default)
    {
        documentScheme.Name = MyRegexSpices().Replace(documentScheme.Name, " ").Trim();
        TResponseModel<DocumentSchemeConstructorModelDB> res = new();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(documentScheme);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DocumentSchemeConstructorModelDB? questionnaire_db = await context_forms
            .DocumentSchemes
            .FirstOrDefaultAsync(x => x.Id != documentScheme.Id && x.ProjectId == documentScheme.ProjectId && x.Name.ToUpper() == documentScheme.Name.ToUpper(), cancellationToken: cancellationToken);
        string msg;
        if (questionnaire_db is not null)
        {
            msg = $"Опрос/анкета с таким именем уже существует: #{questionnaire_db.Id} '{questionnaire_db.Name}'";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        ResponseBaseModel check_project;
        if (documentScheme.Id < 1)
        {
            check_project = await CanEditProject(documentScheme.ProjectId);
            if (!check_project.Success())
            {
                res.AddRangeMessages(check_project.Messages);
                return res;
            }

            questionnaire_db = DocumentSchemeConstructorModelDB.Build(documentScheme, documentScheme.ProjectId);
            await context_forms.AddAsync(questionnaire_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Опрос/анкета создана #{questionnaire_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.Response = questionnaire_db;
            return res;
        }
        questionnaire_db = await context_forms
            .DocumentSchemes
            .FirstOrDefaultAsync(x => x.Id == documentScheme.Id, cancellationToken: cancellationToken);

        if (questionnaire_db is null)
        {
            msg = $"Опрос/анкета #{documentScheme.Id} отсутствует в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        check_project = await CanEditProject(questionnaire_db.ProjectId);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        if (questionnaire_db.Name == documentScheme.Name && questionnaire_db.Description == documentScheme.Description)
        {
            msg = $"Опрос/анкета #{documentScheme.Id} не требует изменений в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
        }
        else
        {
            questionnaire_db.Name = documentScheme.Name;
            questionnaire_db.Description = documentScheme.Description;
            context_forms.Update(questionnaire_db);
            msg = $"Опрос/анкета #{documentScheme.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        await context_forms
             .Projects
             .Where(u => u.Id == documentScheme.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return await GetDocumentScheme(questionnaire_db.Id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteDocumentScheme(int document_scheme_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DocumentSchemeConstructorModelDB? document_scheme_db = await context_forms
            .DocumentSchemes
            .Include(x => x.Tabs!)
            .ThenInclude(x => x.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == document_scheme_id, cancellationToken: cancellationToken);

        if (document_scheme_db is null)
            return ResponseBaseModel.CreateError($"Опрос/анкета #{document_scheme_id} не найдена в БД");

        ResponseBaseModel check_project = await CanEditProject(document_scheme_db.ProjectId);
        if (!check_project.Success())
            return check_project;

        context_forms.Remove(document_scheme_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == document_scheme_db.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Опрос/анкета #{document_scheme_id} удалена");
    }
    #endregion
    // табы/вкладки схожи по смыслу табов/вкладок в Excel. Т.е. обычная группировка разных рабочих пространств со своим именем 
    #region табы документов    
    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> GetTabOfDocumentScheme(int tab_of_document_scheme_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<TabOfDocumentSchemeConstructorModelDB> res = new()
        {
            Response = await context_forms
           .TabsOfDocumentsSchemes

           .Include(x => x.JoinsForms!)
           .ThenInclude(x => x.Form)
           .ThenInclude(x => x!.Fields)

           .Include(x => x.JoinsForms!)
           .ThenInclude(x => x.Form)
           .ThenInclude(x => x!.FieldsDirectoriesLinks)

           .FirstOrDefaultAsync(x => x.Id == tab_of_document_scheme_id, cancellationToken: cancellationToken)
        };

        if (res.Response is null)
            res.AddError($"Форма #{tab_of_document_scheme_id} не найдена в БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<DocumentSchemeConstructorModelDB>> MoveTabOfDocumentScheme(int tab_of_document_scheme_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<DocumentSchemeConstructorModelDB> res = new();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TabOfDocumentSchemeConstructorModelDB? tab_of_document_scheme_db = await context_forms
            .TabsOfDocumentsSchemes
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Tabs)
            .FirstOrDefaultAsync(x => x.Id == tab_of_document_scheme_id, cancellationToken: cancellationToken);

        if (tab_of_document_scheme_db?.Owner is null)
        {
            res.AddError($"Страница опроса/анкеты #{tab_of_document_scheme_id} отсутствует в БД");
            return res;
        }

        ResponseBaseModel check_project = await CanEditProject(tab_of_document_scheme_db.Owner.ProjectId);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        tab_of_document_scheme_db.Owner!.Tabs = [.. tab_of_document_scheme_db.Owner.Tabs!.OrderBy(x => x.SortIndex)];

        TabOfDocumentSchemeConstructorModelDB? _fns = tab_of_document_scheme_db.Owner.GetOutermostPage(direct, tab_of_document_scheme_db.SortIndex);

        if (_fns is null)
            res.AddError("Не удалось выполнить перемещение. ошибка 7BA66820-1DDF-42B0-AF6D-F8F0C920A40E");
        else
        {
            res.AddInfo($"Страницы опроса/анкеты меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' && #{tab_of_document_scheme_db.Id} i:{tab_of_document_scheme_db.SortIndex} '{tab_of_document_scheme_db.Name}'");
            int next_index = _fns.SortIndex;
            int tmp_id = tab_of_document_scheme_db.SortIndex;
            if (direct == VerticalDirectionsEnum.Down)
            {
                tab_of_document_scheme_db.SortIndex = r.Next(10000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1), 50000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1));
            }
            else
            {
                tab_of_document_scheme_db.SortIndex = r.Next(50000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1), 10000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1));
            }
            context_forms.Update(tab_of_document_scheme_db);
            await context_forms.SaveChangesAsync(cancellationToken);
            _fns.SortIndex = tmp_id;
            context_forms.Update(_fns);
            await context_forms.SaveChangesAsync(cancellationToken);
            tab_of_document_scheme_db.SortIndex = next_index;
            context_forms.Update(tab_of_document_scheme_db);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        if (!res.Success())
            return res;

        res.Response = await context_forms
            .DocumentSchemes
            .Include(x => x.Tabs)
            .FirstAsync(x => x.Id == tab_of_document_scheme_db.OwnerId, cancellationToken: cancellationToken);
        tab_of_document_scheme_db.Owner!.Tabs = tab_of_document_scheme_db.Owner.Tabs!.OrderBy(x => x.SortIndex).ToList();

        int i = 0;
        bool is_upd = false;
        foreach (TabOfDocumentSchemeConstructorModelDB p in res.Response.Tabs!)
        {
            i++;
            is_upd = is_upd || p.SortIndex != i;
            p.SortIndex = i;
        }

        if (is_upd)
        {
            res.AddWarning("Исправлена пересортица");
            context_forms.UpdateRange(res.Response.Tabs!);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        await context_forms
             .Projects
             .Where(u => u.Id == tab_of_document_scheme_db.Owner.ProjectId)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> CreateOrUpdateTabOfDocumentScheme(EntryDescriptionOwnedModel tab_of_document_scheme, CancellationToken cancellationToken = default)
    {
        TResponseModel<TabOfDocumentSchemeConstructorModelDB> res = new();

        tab_of_document_scheme.Name = MyRegexSpices().Replace(tab_of_document_scheme.Name, " ").Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(tab_of_document_scheme);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        DocumentSchemeConstructorModelDB? document_scheme_db = await context_forms
            .DocumentSchemes
            .Include(x => x.Tabs)
            .FirstOrDefaultAsync(x => x.Id == tab_of_document_scheme.OwnerId, cancellationToken: cancellationToken);

        string msg;
        if (document_scheme_db is null)
        {
            msg = $"Анкета/опрос #{tab_of_document_scheme.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        int current_project_id = await
            (from project in context_forms.Projects
             join ds in context_forms.DocumentSchemes on project.Id equals ds.ProjectId
             where ds.Id == document_scheme_db.Id
             select project.Id).FirstAsync(cancellationToken: cancellationToken);

        ResponseBaseModel check_project = await CanEditProject(current_project_id);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        TabOfDocumentSchemeConstructorModelDB? tab_of_document_scheme_db;
        if (tab_of_document_scheme.Id < 1)
        {
            tab_of_document_scheme.Id = 0;
            int _sort_index = document_scheme_db.Tabs!.Count != 0 ? document_scheme_db.Tabs!.Max(x => x.SortIndex) : 0;

            tab_of_document_scheme_db = TabOfDocumentSchemeConstructorModelDB.Build(tab_of_document_scheme, document_scheme_db, _sort_index + 1);

            await context_forms.AddAsync(tab_of_document_scheme_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Страница анкеты/опроса создано #{tab_of_document_scheme_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.Response = tab_of_document_scheme_db;
            return res;
        }

        tab_of_document_scheme_db = document_scheme_db.Tabs!.FirstOrDefault(x => x.Id == tab_of_document_scheme.Id);

        if (tab_of_document_scheme_db is null)
        {
            msg = $"Страница опроса/анкеты #{tab_of_document_scheme.Id} не найдено в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (tab_of_document_scheme_db.Name != tab_of_document_scheme.Name)
        {
            msg = $"Имя страницы опроса/анкеты #{tab_of_document_scheme.Id} изменилось: [{tab_of_document_scheme_db.Name}] -> [{tab_of_document_scheme.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            tab_of_document_scheme_db.Name = tab_of_document_scheme.Name;
        }
        if (tab_of_document_scheme_db.Description != tab_of_document_scheme.Description)
        {
            msg = $"Описание страницы опроса/анкеты #{tab_of_document_scheme.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            tab_of_document_scheme_db.Description = tab_of_document_scheme.Description;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            context_forms.Update(tab_of_document_scheme_db);
            msg = $"Страница опроса/анкеты #{tab_of_document_scheme.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        await context_forms
             .Projects
             .Where(u => u.Id == current_project_id)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        res.Response = tab_of_document_scheme_db;
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteTabOfDocumentScheme(int tab_of_document_scheme_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TabOfDocumentSchemeConstructorModelDB? tab_of_document_scheme_db = await context_forms
            .TabsOfDocumentsSchemes
            .Include(x => x.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == tab_of_document_scheme_id, cancellationToken: cancellationToken);

        if (tab_of_document_scheme_db is null)
            return ResponseBaseModel.CreateError($"Страница опроса/анкеты #{tab_of_document_scheme_id} не найден в БД");

        int current_project_id = await
            (from project in context_forms.Projects
             join ds in context_forms.DocumentSchemes on project.Id equals ds.ProjectId
             where ds.Id == tab_of_document_scheme_db.OwnerId
             select project.Id).FirstAsync(cancellationToken: cancellationToken);

        ResponseBaseModel check_project = await CanEditProject(current_project_id);
        if (!check_project.Success())
            return check_project;

        context_forms.Remove(tab_of_document_scheme_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == current_project_id)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Страница #{tab_of_document_scheme_id} удалена из опроса/анкеты");
    }
    #endregion
    #region структура/схема табов/вкладок схем документов: формы, порядок и настройки поведения    
    /// <inheritdoc/>
    public async Task<TResponseModel<TabJoinDocumentSchemeConstructorModelDB>> GetTabDocumentSchemeJoinForm(int tab_document_scheme_join_form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<TabJoinDocumentSchemeConstructorModelDB> res = new()
        {
            Response = await context_forms
            .TabsJoinsForms
            .Include(x => x.Form)
            .Include(x => x.Tab)
            .ThenInclude(x => x!.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == tab_document_scheme_join_form_id, cancellationToken: cancellationToken)
        };

        if (res.Response is null)
            res.AddError($"Связь #{tab_document_scheme_join_form_id} (форма<->опрос/анкета) не найдена в БД");
        else if (res.Response.Tab?.JoinsForms is not null)
            res.Response.Tab.JoinsForms = res.Response.Tab.JoinsForms.OrderBy(x => x.SortIndex).ToList();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TabOfDocumentSchemeConstructorModelDB>> MoveTabDocumentSchemeJoinForm(int tab_document_scheme_join_form_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<TabOfDocumentSchemeConstructorModelDB> res = new();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TabJoinDocumentSchemeConstructorModelDB? questionnaire_page_join_db = await context_forms
            .TabsJoinsForms
            .Include(x => x.Tab)
            .ThenInclude(x => x!.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == tab_document_scheme_join_form_id, cancellationToken: cancellationToken);

        if (questionnaire_page_join_db?.Tab?.JoinsForms is null)
        {
            res.AddError($"Форма для страницы опроса/анкеты #{tab_document_scheme_join_form_id} отсутствует в БД. ошибка 66CB7541-20AE-4C26-A020-3A9546457C3D");
            return res;
        }

        int current_project_id = await (from project in context_forms.Projects
                                        join ds in context_forms.DocumentSchemes on project.Id equals ds.ProjectId
                                        join tds in context_forms.TabsOfDocumentsSchemes on ds.Id equals tds.OwnerId
                                        where tds.Id == questionnaire_page_join_db.TabId
                                        select project.Id)
                                                          .FirstAsync(cancellationToken: cancellationToken);

        ResponseBaseModel check_project = await CanEditProject(current_project_id);
        if (!check_project.Success())
        {
            res.AddRangeMessages(check_project.Messages);
            return res;
        }

        TabJoinDocumentSchemeConstructorModelDB? _fns = questionnaire_page_join_db.Tab.GetOutermostJoinForm(direct, questionnaire_page_join_db.SortIndex);

        if (_fns is null)
            res.AddError("Не удалось выполнить перемещение. ошибка ED601887-8BB3-4FB7-96C7-1563FD9B1FCD");
        else
        {
            res.AddInfo($"Формы для страницы опроса/анкеты меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' && #{questionnaire_page_join_db.Id} i:{questionnaire_page_join_db.SortIndex} '{questionnaire_page_join_db.Name}'");

            int next_index = _fns.SortIndex;
            int tmp_id = questionnaire_page_join_db.SortIndex;

            questionnaire_page_join_db.SortIndex = direct == VerticalDirectionsEnum.Down
                ? questionnaire_page_join_db.SortIndex = r.Next(10000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1), 50000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1))
                : questionnaire_page_join_db.SortIndex = r.Next(50000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1), 10000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1));

            context_forms.Update(questionnaire_page_join_db);
            await context_forms.SaveChangesAsync(cancellationToken);
            _fns.SortIndex = tmp_id;
            context_forms.Update(_fns);
            await context_forms.SaveChangesAsync(cancellationToken);
            questionnaire_page_join_db.SortIndex = next_index;
            context_forms.Update(questionnaire_page_join_db);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        if (!res.Success())
            return res;

        res.Response = await context_forms
            .TabsOfDocumentsSchemes
            .Include(x => x.JoinsForms)
            .FirstAsync(x => x.Id == questionnaire_page_join_db.TabId, cancellationToken: cancellationToken);

        questionnaire_page_join_db.Tab.JoinsForms = [.. questionnaire_page_join_db.Tab.JoinsForms!.OrderBy(x => x.SortIndex)];

        int i = 0;
        bool is_upd = false;
        foreach (TabJoinDocumentSchemeConstructorModelDB p in res.Response.JoinsForms!)
        {
            i++;
            is_upd = is_upd || p.SortIndex != i;
            p.SortIndex = i;
        }

        if (is_upd)
        {
            res.AddWarning("Исправлена пересортица");
            context_forms.UpdateRange(res.Response.JoinsForms);
            await context_forms.SaveChangesAsync(cancellationToken);

            await context_forms
                 .Projects
                 .Where(u => u.Id == current_project_id)
                 .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateOrUpdateTabDocumentSchemeJoinForm(TabJoinDocumentSchemeConstructorModelDB tab_document_scheme_form, CancellationToken cancellationToken = default)
    {
        tab_document_scheme_form.Name = tab_document_scheme_form.Name is null
            ? null
            : MyRegexSpices().Replace(tab_document_scheme_form.Name, " ").Trim();

        tab_document_scheme_form.Description = tab_document_scheme_form.Description?.Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(tab_document_scheme_form);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TabOfDocumentSchemeConstructorModelDB? tab_of_document_db = await context_forms
            .TabsOfDocumentsSchemes
            .Include(x => x.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == tab_document_scheme_form.TabId, cancellationToken: cancellationToken);

        string msg;
        if (tab_of_document_db?.JoinsForms is null)
        {
            msg = $"Страница анкеты/опроса #{tab_document_scheme_form.TabId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        int current_project_id = await (from project in context_forms.Projects
                                        join ds in context_forms.DocumentSchemes on project.Id equals ds.ProjectId
                                        where ds.Id == tab_of_document_db.OwnerId
                                        select project.Id)
                                                           .FirstAsync(cancellationToken: cancellationToken);

        ResponseBaseModel check_project = await CanEditProject(current_project_id);
        if (!check_project.Success())
            return check_project;

        TabJoinDocumentSchemeConstructorModelDB? questionnaire_page_join_db;
        if (tab_document_scheme_form.Id < 1)
        {
            int _sort_index = tab_of_document_db.JoinsForms.Count != 0 ? tab_of_document_db.JoinsForms.Max(x => x.SortIndex) : 0;
            tab_document_scheme_form.SortIndex = _sort_index + 1;
            await context_forms.AddAsync(tab_document_scheme_form, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Создана связь форма и страницы анкеты/опроса #{tab_of_document_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        questionnaire_page_join_db = tab_of_document_db.JoinsForms.FirstOrDefault(x => x.Id == tab_document_scheme_form.Id);

        if (questionnaire_page_join_db is null)
        {
            msg = $"Связь формы и страницы опроса/анкеты #{tab_document_scheme_form.Id} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (questionnaire_page_join_db.Name != tab_document_scheme_form.Name)
        {
            msg = $"Имя связи формы и страницы опроса/анкеты #{tab_document_scheme_form.Id} изменилось: [{questionnaire_page_join_db.Name}] -> [{tab_document_scheme_form.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.Name = tab_document_scheme_form.Name;
        }
        if (questionnaire_page_join_db.Description != tab_document_scheme_form.Description)
        {
            msg = $"Описание связи формы и страницы опроса/анкеты #{tab_document_scheme_form.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.Description = tab_document_scheme_form.Description;
        }
        if (questionnaire_page_join_db.ShowTitle != tab_document_scheme_form.ShowTitle)
        {
            msg = $"Описание связи формы и страницы опроса/анкеты #{tab_document_scheme_form.Id} изменение '{nameof(questionnaire_page_join_db.ShowTitle)}': [{questionnaire_page_join_db.ShowTitle}] => [{tab_document_scheme_form.ShowTitle}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.ShowTitle = tab_document_scheme_form.ShowTitle;
        }
        if (questionnaire_page_join_db.IsTable != tab_document_scheme_form.IsTable)
        {
            msg = $"Признак [таблица] связи формы и страницы опроса/анкеты #{tab_document_scheme_form.Id} изменение '{nameof(questionnaire_page_join_db.IsTable)}': [{questionnaire_page_join_db.IsTable}] => [{tab_document_scheme_form.IsTable}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.IsTable = tab_document_scheme_form.IsTable;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            context_forms.Update(questionnaire_page_join_db);
            msg = $"Связь формы и страницы опроса/анкеты #{tab_document_scheme_form.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);

            await context_forms
                 .Projects
                 .Where(u => u.Id == current_project_id)
                 .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteTabDocumentSchemeJoinForm(int tab_document_scheme_join_form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TabJoinDocumentSchemeConstructorModelDB? questionnaire_page_db = await context_forms
            .TabsJoinsForms
            .FirstOrDefaultAsync(x => x.Id == tab_document_scheme_join_form_id, cancellationToken: cancellationToken);

        if (questionnaire_page_db is null)
            return ResponseBaseModel.CreateError($"Связь формы и страницы опроса/анкеты #{tab_document_scheme_join_form_id} не найдена в БД");

        int current_project_id = await (from project in context_forms.Projects
                                        join ds in context_forms.DocumentSchemes on project.Id equals ds.ProjectId
                                        join tds in context_forms.TabsOfDocumentsSchemes on ds.Id equals tds.OwnerId
                                        where tds.Id == questionnaire_page_db.TabId
                                        select project.Id)
                                                           .FirstAsync(cancellationToken: cancellationToken);

        ResponseBaseModel check_project = await CanEditProject(current_project_id);
        if (!check_project.Success())
            return check_project;

        context_forms.Remove(questionnaire_page_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        await context_forms
             .Projects
             .Where(u => u.Id == current_project_id)
             .ExecuteUpdateAsync(b => b.SetProperty(u => u.SchemeLastUpdated, DateTime.Now), cancellationToken: cancellationToken);

        return ResponseBaseModel.CreateSuccess("Связь формы и страницы удалена из опроса/анкеты");
    }
    #endregion

    /////////////// Пользовательский/публичный доступ к возможностям заполнения документа данными
    // Если у вас есть готовый к заполнению документ со всеми его табами и настройками, то вы можете создавать уникальные ссылки для заполнения данными
    // Каждая ссылка это всего лишь уникальный GUID к которому привязываются все данные, которые вводят конечные пользователи
    // Пользователи видят ваш документ, но сам документ данные не хранит. Хранение данных происходит в сессиях, которые вы сами выпускаете для любого вашего документа
    #region сессии документов (данные заполнения документов).
    /// <inheritdoc/>
    public async Task<TResponseModel<ValueDataForSessionOfDocumentModelDB[]>> SaveSessionForm(int sessionId, int join, List<ValueDataForSessionOfDocumentModelDB> sessionValues)
    {
        sessionValues = [.. sessionValues.SkipWhile(x => x.Id < 1 && string.IsNullOrWhiteSpace(x.Value))];

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        SessionOfDocumentDataModelDB? sq = await context_forms
            .Sessions
            .Include(x => x.DataSessionValues!)
            .ThenInclude(x => x.TabJoinDocumentScheme)
            .FirstOrDefaultAsync(x => x.Id == sessionId);

        TResponseModel<ValueDataForSessionOfDocumentModelDB[]> res = new();

        if (sq is null)
        {
            res.AddError("Объект документа удалён");
            return res;
        }

        ValueDataForSessionOfDocumentModelDB[] _session_data = sq
            .DataSessionValues!
            .Where(x => x.TabJoinDocumentSchemeId == join)
            .ToArray();

        int[] _ids_del = sessionValues
            .Where(x => x.Id > 0 && string.IsNullOrWhiteSpace(x.Value))
            .Select(x => x.Id)
            .ToArray();

        if (_ids_del.Length != 0)
        {
            context_forms.RemoveRange(context_forms.Sessions.Where(x => _ids_del.Contains(x.Id)));
            await context_forms.SaveChangesAsync();
        }

        ValueDataForSessionOfDocumentModelDB[] values_upd = _session_data
            .Where(x => sessionValues.Any(y => y.Id == x.Id && x.Value != y.Value))
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => { x.Value = sessionValues.First(y => y.Id == x.Id).Value; return x; })
            .ToArray();

        if (values_upd.Length != 0)
        {
            context_forms.UpdateRange(values_upd);
            await context_forms.SaveChangesAsync();
        }

        values_upd = sessionValues.Where(x => x.Id == 0).ToArray();
        if (values_upd.Length != 0)
        {
            await context_forms.AddRangeAsync(values_upd);
            await context_forms.SaveChangesAsync();
        }

        res.AddSuccess("Форма документа сохранена");
        res.Response = await context_forms
            .ValuesSessions
            .Where(x => x.OwnerId == sessionId && x.TabJoinDocumentSchemeId == join)
            .ToArrayAsync();

        if (res.Response.Any(x => string.IsNullOrWhiteSpace(x.Value)))
        {
            _ids_del = [..res
                .Response
                .Where(x => string.IsNullOrWhiteSpace(x.Value))
                .Select(x => x.Id)];

            context_forms.RemoveRange(context_forms.Sessions.Where(x => _ids_del.Contains(x.Id)));
            await context_forms.SaveChangesAsync();
            res.Response = [.. res.Response.SkipWhile(x => string.IsNullOrWhiteSpace(x.Value))];
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetStatusSessionDocument(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        SessionOfDocumentDataModelDB? sq = await context_forms.Sessions.FirstOrDefaultAsync(x => x.Id == id_session, cancellationToken: cancellationToken);
        if (sq is null)
            return ResponseBaseModel.CreateError($"Сессия [{id_session}] не найдена в БД. ошибка A85733AF-56F4-45D2-A16C-729352D1645B");

        if (sq.SessionStatus == status)
            return ResponseBaseModel.CreateSuccess($"Сессия уже переведена в статус [{status}] и не требует обработки");

        sq.SessionStatus = status;

        await context_forms.SaveChangesAsync(cancellationToken);
        ResponseBaseModel res = new();
        string msg = $"Сессия опросника/анкетирования `{sq.SessionToken}` {status}";
        if (!string.IsNullOrWhiteSpace(sq.EmailsNotifications))
        {
            try
            {
                res.AddSuccess($"Наблюдатели оповещены:{sq.EmailsNotifications}");
            }
            catch (Exception ex)
            {
                logger.LogError($"error : {ex.Message}\n: {ex.StackTrace}");
                res.AddWarning($"Не удалось отправить уведомление наблюдателям [{sq.EmailsNotifications}]. Возникло исключение 7F67AA0A-7F5F-499D-8680-73A665106D8E: {ex.Message}");
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> GetSessionDocument(int id_session, bool skip_include = false, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        IQueryable<SessionOfDocumentDataModelDB> q = context_forms
            .Sessions
            .Where(x => x.Id == id_session)
            .AsQueryable();

        IIncludableQueryable<SessionOfDocumentDataModelDB, List<FieldFormAkaDirectoryConstructorModelDB>?> inc = q
            .Include(x => x.Project)
            .Include(x => x.DataSessionValues)

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Tabs!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.Fields) // поля

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Tabs!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.FieldsDirectoriesLinks);

        TResponseModel<SessionOfDocumentDataModelDB> res = new()
        {
            Response = skip_include
            ? await q.FirstOrDefaultAsync(cancellationToken: cancellationToken)
            : await inc.FirstOrDefaultAsync(cancellationToken: cancellationToken)
        };
        string msg;
        if (res.Response is null)
        {
            msg = $"for {nameof(id_session)} = [{id_session}]. SessionDocument is null. error 965BED19-5E30-4AA5-8FBD-1B3EFEFC5B1D";
            logger.LogError(msg);
            res.AddError(msg);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<SessionOfDocumentDataModelDB>> UpdateOrCreateSessionDocument(SessionOfDocumentDataModelDB session_json, CancellationToken cancellationToken = default)
    {
        TResponseModel<SessionOfDocumentDataModelDB> res = new();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(session_json);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        if (!string.IsNullOrWhiteSpace(session_json.EmailsNotifications))
        {
            string[] een = session_json.EmailsNotifications.SplitToList().Where(x => !MailAddress.TryCreate(x, out _)).ToArray();
            if (een.Length != 0)
            {
                res.AddError($"Не корректные адреса получателей. {JsonConvert.SerializeObject(een)}. error AFDDD9DE-F36E-4FB0-9C10-ACDAF48409A8");
                return res;
            }
        }

        if (session_json.SessionStatus == SessionsStatusesEnum.None)
            session_json.SessionToken = null;

        session_json.Name = MyRegexSpices().Replace(session_json.Name.Trim(), " ");
        session_json.NormalizeUpperName = session_json.Name.ToUpper();

        using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
        ApplicationUser? userDb = await identityContext.Users
            .FirstOrDefaultAsync(x => x.Id == session_json.AuthorUser, cancellationToken: cancellationToken);

        if (userDb is null)
        {
            res.AddError($"Пользователь #{session_json.AuthorUser} не найден в БД");
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        Expression<Func<SessionOfDocumentDataModelDB, bool>> expr = x
            => x.Id != session_json.Id &&
            x.OwnerId == session_json.OwnerId &&
            x.ProjectId == session_json.ProjectId &&
            x.NormalizeUpperName == session_json.NormalizeUpperName;

        SessionOfDocumentDataModelDB? session_db = await context_forms
            .Sessions
            .FirstOrDefaultAsync(expr, cancellationToken: cancellationToken);

        if (session_db is not null)
        {
            res.AddError($"Ссылка для выбранного документа с таким именем уже существует в БД! Для одного и того же документа имена должны быть уникальны.");
            return res;
        }

        if (session_json.Id < 1)
        {
            session_json.CreatedAt = DateTime.Now;
            session_json.DeadlineDate = DateTime.Now.AddMinutes(_conf.Value.TimeActualityDocumentSessionMinutes);
            session_json.SessionToken = Guid.NewGuid().ToString();
            session_json.SessionStatus = SessionsStatusesEnum.InProgress;

            await context_forms.AddAsync(session_json, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            return new() { Response = session_json, Messages = [new() { TypeMessage = ResultTypesEnum.Success, Text = $"Создана сессия #{session_json.Id}" }] };
        }
        session_db = await context_forms.Sessions.FirstOrDefaultAsync(x => x.Id == session_json.Id, cancellationToken: cancellationToken);
        string msg;
        if (session_db is null)
        {
            msg = $"Сессия #{session_json.Id} не найдена в БД";
            logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        if (session_db.Name == session_json.Name &&
            session_db.Description == session_json.Description &&
            session_db.EmailsNotifications == session_json.EmailsNotifications &&
            session_db.SessionStatus == session_json.SessionStatus &&
            session_db.ShowDescriptionAsStartPage == session_json.ShowDescriptionAsStartPage &&
            session_db.DeadlineDate == session_json.DeadlineDate &&
            session_db.SessionToken == session_json.SessionToken)
        {
            msg = $"Сессия #{session_json.Id} не требует изменений в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            return res;
        }
        else if (Guid.TryParse(session_json.SessionToken, out Guid _guid_parsed) && _guid_parsed != Guid.Empty && await context_forms.Sessions.AnyAsync(x => x.Id != session_db.Id && x.SessionToken == session_json.SessionToken, cancellationToken: cancellationToken))
        {
            msg = $"Токен сессии {session_json.SessionToken} уже занят";
            logger.LogError(msg);
            res.AddError(msg);
            return res;
        }
        else
        {
            session_db.Name = session_json.Name;
            session_db.Description = session_json.Description;
            session_db.EmailsNotifications = session_json.EmailsNotifications;
            session_db.SessionStatus = session_json.SessionStatus;
            session_db.ShowDescriptionAsStartPage = session_json.ShowDescriptionAsStartPage;
            session_db.DeadlineDate = session_json.DeadlineDate;

            if (Guid.Empty.ToString() == session_json.SessionToken)
                session_db.SessionToken = Guid.NewGuid().ToString();
            else
                session_db.SessionToken = session_json.SessionToken;

            context_forms.Update(session_db);
            msg = $"Сессия #{session_db.Id} обновлена в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }
        TResponseModel<SessionOfDocumentDataModelDB> sr = await GetSessionDocument(session_json.Id, false, cancellationToken);
        if (res.Messages.Count != 0)
            sr.AddRangeMessages(res.Messages);
        return sr;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<SessionOfDocumentDataModelDB>> RequestSessionsDocuments(RequestSessionsDocumentsRequestPaginationModel req, CancellationToken cancellationToken = default)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TPaginationResponseModel<SessionOfDocumentDataModelDB> res = new(req);
        IQueryable<SessionOfDocumentDataModelDB> query = context_forms
            .Sessions
            .Include(x => x.Owner)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (req.DocumentSchemeId > 0)
            query = query.Where(x => x.OwnerId == req.DocumentSchemeId);

        if (req.ProjectId > 0)
            query = query.Where(x => context_forms.DocumentSchemes.Any(y => y.Id == x.OwnerId && y.ProjectId == req.ProjectId));

        if (!string.IsNullOrWhiteSpace(req.FilterUserId))
            query = query.Where(x => x.AuthorUser == req.FilterUserId);

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
        {
            Expression<Func<SessionOfDocumentDataModelDB, bool>> expr = x
                => EF.Functions.Like(x.NormalizeUpperName!, $"%{req.SimpleRequest.ToUpper()}%") ||
                (x.SessionToken != null && x.SessionToken.ToLower() == req.SimpleRequest.ToLower()) ||
                (!string.IsNullOrWhiteSpace(x.EmailsNotifications) && EF.Functions.Like(x.EmailsNotifications.ToLower(), $"%{req.SimpleRequest.ToLower()}%"));

            query = query.Where(expr);
        }
        res.TotalRowsCount = await query.CountAsync(cancellationToken: cancellationToken);
        query = query.Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        res.Response = await query.ToListAsync(cancellationToken: cancellationToken);

        if (res.Response.Count != 0)
        {
            string[] users_ids = res.Response.Select(x => x.AuthorUser).Distinct().ToArray();
            using IdentityAppDbContext identityContext = identityDbFactory.CreateDbContext();
            ApplicationUser[] users_data = await identityContext.Users.Where(x => users_ids.Contains(x.Id)).ToArrayAsync(cancellationToken: cancellationToken);

            res.Response.ForEach(x => { x.AuthorUser = users_data.FirstOrDefault(y => y.Id.Equals(x.AuthorUser))?.UserName ?? x.AuthorUser; });
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDictModel[]>> FindSessionsDocumentsByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<EntryDictModel[]> res = new();
        if (string.IsNullOrWhiteSpace(req.FieldName))
        {
            res.AddError("Не указано имя поля/колонки");
            return res;
        }
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        var q = from _vs in context_forms.ValuesSessions.Where(_vs => _vs.Name == req.FieldName)
                join _s in context_forms.Sessions on _vs.OwnerId equals _s.Id
                join _pjf in context_forms.TabsJoinsForms.Where(x => x.FormId == req.FormId) on _vs.TabJoinDocumentSchemeId equals _pjf.Id
                join _qp in context_forms.TabsOfDocumentsSchemes on _pjf.TabId equals _qp.Id
                select new { Value = _vs, Session = _s, DocumentPageJoinForm = _pjf, DocumentPage = _qp };

        var data_rows = await q.ToArrayAsync(cancellationToken: cancellationToken);

        res.Response = data_rows
            .GroupBy(x => x.Session.Id)
            .Select(x =>
            {
                var element_g = x.First();
                Dictionary<string, object> _d = new()
                {
                    { nameof(Enumerable.Count), x.Count() },
                    { nameof(element_g.Session.CreatedAt), element_g.Session.CreatedAt },
                    { nameof(element_g.Session.AuthorUser), element_g.Session.AuthorUser },
                    { nameof(element_g.Session.SessionStatus), element_g.Session.SessionStatus }
                };

                if (!string.IsNullOrWhiteSpace(element_g.Session.EmailsNotifications))
                    _d.Add(nameof(element_g.Session.EmailsNotifications), element_g.Session.EmailsNotifications);

                if (!string.IsNullOrWhiteSpace(element_g.Session.Editors))
                    _d.Add(nameof(element_g.Session.Editors), element_g.Session.Editors);

                if (!string.IsNullOrWhiteSpace(element_g.Session.SessionToken))
                    _d.Add(nameof(element_g.Session.SessionToken), element_g.Session.SessionToken);
                if (element_g.Session.DeadlineDate is not null)
                    _d.Add(nameof(element_g.Session.DeadlineDate), element_g.Session.DeadlineDate);
                if (element_g.Session.LastDocumentUpdateActivity is not null)
                    _d.Add(nameof(element_g.Session.LastDocumentUpdateActivity), element_g.Session.LastDocumentUpdateActivity);

                return new EntryDictModel()
                {
                    Id = x.Key,
                    Name = element_g.Session.Name,
                    Tag = _d
                };
            }).ToArray();
        res.AddInfo($"Получено ссылок {res.Response.Length}");
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClearValuesForFieldName(FormFieldOfSessionModel req, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        IQueryable<ValueDataForSessionOfDocumentModelDB> q = from _vs in context_forms.ValuesSessions.Where(_vs => _vs.Name == req.FieldName)
                                                             join _s in context_forms.Sessions.Where(x => !req.SessionId.HasValue || x.Id == req.SessionId.Value) on _vs.OwnerId equals _s.Id
                                                             join _pjf in context_forms.TabsJoinsForms.Where(x => x.FormId == req.FormId) on _vs.TabJoinDocumentSchemeId equals _pjf.Id
                                                             select _vs;
        int _i = await q.CountAsync();
        if (_i == 0)
            return ResponseBaseModel.CreateSuccess("Значений нет (удалить нечего)");

        context_forms.RemoveRange(q);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Удалено значений: {_i}");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteSessionDocument(int session_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        SessionOfDocumentDataModelDB? session_db = await context_forms
            .Sessions
            .Include(x => x.DataSessionValues)

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Tabs!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.Fields) // поля

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Tabs!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.FieldsDirectoriesLinks) // поля
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == session_id, cancellationToken: cancellationToken);

        if (session_db is null)
            return ResponseBaseModel.CreateError($"Сессия #{session_id} не найдена в БД");

        context_forms.Remove(session_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        string json_string = JsonConvert.SerializeObject(session_db, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        logger.LogWarning($"удаление сессии: {json_string}");

        return ResponseBaseModel.CreateSuccess($"Сессия #{session_id} успешно удалена из БД");
    }
    #endregion

    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegexSpices();
}