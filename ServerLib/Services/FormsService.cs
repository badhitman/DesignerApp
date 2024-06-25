using DbcLib;
using IdentityLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedLib;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ServerLib;

/// <summary>
/// Forms служба
/// </summary>
public class FormsService(IDbContextFactory<MainDbAppContext> mainDbFactory, IDbContextFactory<IdentityAppDbContext> identityDbFactory, ILogger<FormsService> logger, IOptions<ServerConstructorConfigModel> _conf) : IFormsService
{
    static readonly Random r = new();

    #region public
    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormSessionModelDB>> GetSessionQuestionnaire(string guid_session, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(guid_session, out Guid guid_parsed) || guid_parsed == Guid.Empty)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Токен сессии имеет не корректный формат" }] };

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<ConstructorFormSessionModelDB> q = context_forms
            .Sessions
            .Include(x => x.SessionValues)

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Pages!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.Fields) // поля

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Pages!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.FormsDirectoriesLinks) // поля

            .AsSplitQuery();

        TResponseModel<ConstructorFormSessionModelDB> res = new()
        {
            Response = await q
            .FirstOrDefaultAsync(x => x.SessionToken == guid_session, cancellationToken: cancellationToken)
        };
        string msg;
        if (res.Response is null)
        {
            msg = $"Токен '{guid_session}' не найден или просрочен.";
            res.AddError(msg);
            logger.LogError($"{msg} res.SessionQuestionnaire is null. error 61362F88-21C8-431A-9038-475B4C52B759");
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetDoneSessionQuestionnaire(string token_session, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormSessionModelDB? sq = await context_forms.Sessions.FirstOrDefaultAsync(x => x.SessionToken == token_session, cancellationToken: cancellationToken);
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

        if (MailAddress.TryCreate(sq.CreatorEmail, out _))
        {
            res.AddSuccess($"Сессия [{token_session}] успешно отправлена на проверку:{sq.CreatorEmail}");
        }
        else
        {
            msg = $"Email автора [{sq.CreatorEmail}] имеет не корректный формат. сессия: {token_session}";
            res.AddWarning(msg);
            logger.LogError(msg);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormSessionModelDB>> SetValueFieldSessionQuestionnaire(SetValueFieldSessionQuestionnaireModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormSessionModelDB> session = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
        if (!session.Success())
            return session;

        ConstructorFormSessionModelDB? session_Questionnaire = session.Response;
        TResponseModel<ConstructorFormSessionModelDB> res = new();

        if (session_Questionnaire?.Owner?.Pages is null || session_Questionnaire.SessionValues is null)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка B3AC5AAF-A786-4190-9C61-A272F174D940");
            return res;
        }

        if (session_Questionnaire.SessionStatus >= SessionsStatusesEnum.Sended)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (находиться в статусе {session_Questionnaire.SessionStatus}).");
            return res;
        }

        session_Questionnaire.LastQuestionnaireUpdateActivity = DateTime.Now;

        ConstructorFormQuestionnairePageJoinFormModelDB? form_join = session_Questionnaire.Owner.Pages.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FormsDirectoriesLinks is null)
        {
            res.AddError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена. ошибка 2494D4D2-24E1-48D4-BC9C-C27D327D98B8");
            return res;
        }

        ConstructorFieldFormBaseLowModel? field_by_name = form_join.Form.AllFields.FirstOrDefault(x => x.Name.Equals(req.NameField, StringComparison.OrdinalIgnoreCase));
        if (field_by_name is null)
        {
            res.AddError($"Поле '{req.NameField}' не найдено в форме #{form_join.Form.Id} '{form_join.Form.Name}'. ошибка 98371573-83A3-41A3-97C2-F8F775BFFD2D");
            return res;
        }
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormSessionValueModelDB? existing_value = session_Questionnaire.SessionValues.FirstOrDefault(x => x.GroupByRowNum == req.GroupByRowNum && x.Name.Equals(req.NameField, StringComparison.OrdinalIgnoreCase) && x.QuestionnairePageJoinFormId == form_join.Id);
        if (existing_value is null)
        {
            existing_value = ConstructorFormSessionValueModelDB.Build(req, form_join, session_Questionnaire);
            await context_forms.AddAsync(existing_value, cancellationToken);
        }
        else
        {
            existing_value.Value = req.FieldValue;
            existing_value.Description = req.Description;
            context_forms.Update(existing_value);
        }

        await context_forms.SaveChangesAsync(cancellationToken);

        return await GetSessionQuestionnaire(req.SessionId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>> AddRowToTable(FieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormSessionModelDB> get_s = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
        if (!get_s.Success())
            return new TResponseStrictModel<int>() { Messages = get_s.Messages, Response = 0 };
        TResponseStrictModel<int> res = new() { Response = 0 };
        ConstructorFormSessionModelDB? session = get_s.Response;

        if (session?.Owner?.Pages is null || session.SessionValues is null)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка 14504D03-88B5-4D1B-AFF2-8DB8D4EB757F");
            return res;
        }

        if (session.SessionStatus >= SessionsStatusesEnum.Sended)
            return (TResponseStrictModel<int>)ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (в статусе {session.SessionStatus}).");

        ConstructorFormQuestionnairePageJoinFormModelDB? form_join = session.Owner.Pages.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FormsDirectoriesLinks is null || !form_join.IsTable)
        {
            res.AddError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена или повреждена. ошибка 6342356D-0491-45BC-A33D-B95F5D7DCB5F");
            return res;
        }
        IQueryable<ConstructorFormSessionValueModelDB> q = session.SessionValues.Where(x => x.QuestionnairePageJoinFormId == form_join.Id && x.GroupByRowNum > 0).AsQueryable();
        res.Response = (int)(q.Any() ? (q.Max(x => x.GroupByRowNum) + 1) : 1);
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        await context_forms.AddRangeAsync(form_join.Form.AllFields.Where(ScalarOnly).Select(x => new ConstructorFormSessionValueModelDB()
        {
            GroupByRowNum = (uint)res.Response,
            Name = x.Name,
            Owner = session,
            OwnerId = session.Id,
            QuestionnairePageJoinForm = form_join,
            QuestionnairePageJoinFormId = form_join.Id
        }), cancellationToken);
        session.LastQuestionnaireUpdateActivity = DateTime.Now;

        await context_forms.SaveChangesAsync(cancellationToken);
        res.AddSuccess($"Добавлена строка в таблицу: №п/п {res.Response}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionQuestionnaireByRowNum(ValueFieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormSessionModelDB> get_s = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
        if (!get_s.Success())
            return get_s;

        ConstructorFormSessionModelDB? session = get_s.Response;
        if (session?.Owner?.Pages is null || session.SessionValues is null)
            return ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка 5DF6598B-18FF-4E76-AE33-6CE78ACE5442");

        if (session.SessionStatus >= SessionsStatusesEnum.Sended)
            return ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (статус {session.SessionStatus}).");

        ConstructorFormQuestionnairePageJoinFormModelDB? form_join = session.Owner.Pages.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FormsDirectoriesLinks is null || !form_join.IsTable)
            return ResponseBaseModel.CreateError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена или повреждена. ошибка 66A38A11-CD9B-4F9E-8B5C-49E60109442D");

        ConstructorFormSessionValueModelDB[] values_for_delete = session.SessionValues.Where(x => x.GroupByRowNum == req.GroupByRowNum && x.QuestionnairePageJoinFormId == form_join.Id).ToArray();

        ResponseBaseModel res = new();
        if (req.IsSelf && values_for_delete.Any(x => !string.IsNullOrWhiteSpace(x.Value)))
        {
            res.AddWarning($"В строке есть данные. Строка не может быть удалена.");
            return res;
        }

        if (values_for_delete.Length > 0)
        {
            session.LastQuestionnaireUpdateActivity = DateTime.Now;
            using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
            context_forms.RemoveRange(values_for_delete);
            await context_forms.SaveChangesAsync(cancellationToken);
            res.AddSuccess($"Строка №{req.GroupByRowNum} удалена ({values_for_delete.Length} значений ячеек)");

            get_s = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
            uint i = 0;
            List<ConstructorFormSessionValueModelDB> values_re_sort = [];
            session.SessionValues
                .Where(x => x.GroupByRowNum > 0)
                .GroupBy(x => x.GroupByRowNum)
                .ToList()
                .ForEach(x =>
                {
                    i++;
                    if (i != x.Key)
                        values_re_sort.AddRange(x.ToList().Select(y => { y.GroupByRowNum = i; return y; }));
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

    static bool ScalarOnly(ConstructorFieldFormBaseLowModel x) => !(x is ConstructorFieldFormModelDB _f && _f.TypeField == TypesFieldsFormsEnum.ProgramCalculationDouble);

    static string? EditorsGenerate(ConstructorFormSessionModelDB session, string editor)
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

    /// <summary>
    /// Признак особых привилегий, которыми обладает создатель/владелец сессии и пользователь с ролью Admin
    /// </summary>
    public static bool IsForce(ClaimsPrincipal clp, ConstructorFormSessionModelDB sq)
    {
        string? email = clp.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value;
        return clp.Claims.Any(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && x.Value.Equals("admin", StringComparison.OrdinalIgnoreCase)) || sq.CreatorEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
    }

    /////////////// Контекст работы конструктора: работы в системе над какими-либо сущностями всегда принадлежат какому-либо проекту/контексту.
    // При переключении контекста (текущий/основной проект) становятся доступны только работы по этому проекту
    // В проект можно добавлять участников, что бы те могли работать вместе с владельцем => вносить изменения в конструкторе данного проекта/контекста
    // Если проект отключить (есть у него такой статус: IsDisabled), то работы с проектом блокируются для всех участников, кроме владельца
    #region проекты
    /// <inheritdoc/>
    public async Task<ProjectViewModel[]> GetProjects(string user_id, string? name_filter = null)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        IQueryable<ProjectConstructorModelDb> q = context_forms
            .Projects
            .Where(x => x.OwnerUserId == user_id || context_forms.MembersOfProjects.Any(y => y.ProjectId == x.Id && y.UserId == user_id))
            .Include(x => x.Members)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name_filter))
            q = q.Where(x => EF.Functions.Like(x.Name.ToUpper(), $"%{name_filter.ToUpper()}%") || EF.Functions.Like(x.SystemName.ToUpper(), $"%{name_filter.ToUpper()}%"));

        ProjectConstructorModelDb[] raw_data = await q.ToArrayAsync();

        string[] usersIds = raw_data
            .Where(x => x.Members is not null)
            .SelectMany(x => x.Members!)
            .Select(x => x.UserId)
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

        List<EntryAltModel>? ReadMembersData(List<MemberOfProjectModelDb>? members)
        {
            if (members is null || usersIdentity is null)
                return null;

            return usersIdentity
                .Where(identityUser => members.Any(memberOfProject => memberOfProject.UserId == identityUser.Id))
                .ToList();
        }
        return raw_data.Select(x => new ProjectViewModel() { OwnerUserId = x.OwnerUserId, Name = x.Name, SystemName = x.SystemName, Description = x.Description, Id = x.Id, IsDisabled = x.IsDisabled, Members = ReadMembersData(x.Members) }).ToArray();
    }

    /// <inheritdoc/>
    public async Task<ProjectConstructorModelDb?> ReadProject(int project_id)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        return await context_forms
            .Projects
            .Include(x => x.Members)
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
        ProjectConstructorModelDb? projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.OwnerUserId == userDb.Id && (x.Name == project.Name || x.SystemName == project.SystemName));

        if (projectDb is not null)
        {
            res.AddError($"Проект должен иметь уникальное имя и код. Похожий проект есть в БД: #{projectDb.Id} '{projectDb.Name}' ({projectDb.SystemName})");
            return res;
        }

        projectDb = new()
        {
            Name = project.Name,
            SystemName = project.SystemName,
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

        ProjectConstructorModelDb? project = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id == project_id);

        if (project is null)
            return ResponseBaseModel.CreateError($"Проект #{project_id} не найден в БД");

        project.IsDisabled = is_deleted;
        context_forms.Update(project);
        await context_forms.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Проект '{project.Name}' [{project.SystemName}] {(is_deleted ? "выключен" : "включён")}");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateProject(ProjectViewModel project)
    {
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(project);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        ProjectConstructorModelDb? projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id != project.Id && (x.Name.ToUpper() == project.Name.ToUpper() || x.SystemName.ToUpper() == project.SystemName.ToUpper()));

        if (projectDb is not null)
            return ResponseBaseModel.CreateError($"Проект должен иметь уникальное имя и код. Похожий проект есть в БД: #{projectDb.Id} '{projectDb.Name}' ({projectDb.SystemName})");

        projectDb = await context_forms
            .Projects
            .FirstOrDefaultAsync(x => x.Id == project.Id);

        if (projectDb is null)
            return ResponseBaseModel.CreateError($"Проект #{project.Id} не найден в БД");

        if (project.Name == projectDb.Name && project.SystemName == projectDb.SystemName && project.Description == projectDb.Description)
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
        MemberOfProjectModelDb? memberDb = await context_forms
            .MembersOfProjects
            .FirstOrDefaultAsync(x => x.ProjectId == project_id && x.UserId == userDb.Id);

        if (memberDb is not null)
            return ResponseBaseModel.CreateInfo("Пользователь уже является участником проекта");

        ProjectConstructorModelDb? projectDb = await context_forms
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

        return ResponseBaseModel.CreateSuccess($"Пользователь/участник {userDb.UserName} добавлен к проекту '{projectDb.Name}' ({projectDb.SystemName})");
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
        MemberOfProjectModelDb? memberDb = await context_forms
            .MembersOfProjects
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.ProjectId == project_id && x.UserId == userDb.Id);
        if (memberDb is null)
            return ResponseBaseModel.CreateInfo("Пользователь не является участником проекта. Удаление не требуется");

        context_forms.Remove(memberDb);
        await context_forms.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess($"Пользователь {userDb.UserName} успешно исключён из проекта '{memberDb.Project!.Name}' ({memberDb.Project.SystemName})");
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
        ProjectConstructorModelDb? projectDb = await context_forms.Projects.FirstOrDefaultAsync(x => x.Id == project_id);
        if (projectDb is null)
            return ResponseBaseModel.CreateError($"Проект #{project_id} не найден в БД");

        ProjectUseModelDb? mainProjectDb = await context_forms.ProjectsUse.FirstOrDefaultAsync(x => x.UserId == user_id);
        if (mainProjectDb is null)
        {
            mainProjectDb = new ProjectUseModelDb() { UserId = user_id, ProjectId = project_id, Project = projectDb };
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
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        if (!await context_forms.Projects.AnyAsync(x => x.OwnerUserId == user_id) && !await context_forms.MembersOfProjects.AnyAsync(x => x.UserId == user_id))
        {
            ProjectConstructorModelDb project = new() { Name = "По умолчанию", OwnerUserId = user_id, SystemName = "Default" };
            await context_forms.AddAsync(project);
            await context_forms.SaveChangesAsync();

            ProjectUseModelDb project_use = new() { UserId = user_id, ProjectId = project.Id };
            await context_forms.AddAsync(project_use);
            await context_forms.SaveChangesAsync();

            res.Response = MainProjectViewModel.Build(project);
        }
        else
            res.Response = await context_forms
                .ProjectsUse
                .Where(x => x.UserId == user_id)
                .Include(x => x.Project)
                .Select(x => new MainProjectViewModel() { Name = x.Project!.Name, Description = x.Project.Description, Id = x.Project.Id, IsDisabled = x.Project.IsDisabled })
                .FirstOrDefaultAsync();

        return res;
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
        ConstructorFormDirectoryModelDB[] res = await context_forms.Directories.Include(x => x.Elements).Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken: cancellationToken);

        return res.Select(x => new EntryNestedModel() { Id = x.Id, Name = x.Name, Childs = x.Elements!.Select(y => new EntryModel() { Id = y.Id, Name = y.Name }) });
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<SystemEntryModel[]>> GetDirectories(int project_id, string? name_filter = null, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        IQueryable<SystemEntryModel> query = context_forms
            .Directories
            .Where(x => x.ProjectId == project_id)
            .Select(x => new SystemEntryModel() { Id = x.Id, Name = x.Name, SystemName = x.SystemName })
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name_filter))
            query = query.Where(x => EF.Functions.Like(x.Name.ToUpper(), $"%{name_filter.ToUpper()}%") || EF.Functions.Like(x.SystemName.ToUpper(), $"%{name_filter.ToUpper()}%"));

        return new TResponseStrictModel<SystemEntryModel[]>() { Response = await query.ToArrayAsync(cancellationToken: cancellationToken) };
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>> UpdateOrCreateDirectory(EntryConstructedModel _dir, CancellationToken cancellationToken = default)
    {
        _dir.Name = Regex.Replace(_dir.Name, @"\s+", " ").Trim();
        TResponseStrictModel<int> res = new() { Response = 0 };
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(_dir);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        string msg;

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryModelDB? directory_db = await context_forms
            .Directories
            .FirstOrDefaultAsync(x => x.Id != _dir.Id && x.ProjectId == _dir.ProjectId && (x.Name == _dir.Name || x.SystemName == _dir.SystemName), cancellationToken: cancellationToken);

        if (directory_db is not null)
        {
            msg = $"Справочник '{directory_db.Name}' ({directory_db.SystemName}) уже существует `#{directory_db.Id}`";
            logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        if (_dir.Id < 1)
        {
            directory_db = ConstructorFormDirectoryModelDB.Build(_dir);
            try
            {
                await context_forms.AddAsync(directory_db, cancellationToken);
                await context_forms.SaveChangesAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                res.AddError(ex.Message);
                return res;
            }

            res.AddSuccess($"Справочник успешно создан #{res.Response}");
        }
        else
        {
            directory_db = await context_forms.Directories.FirstOrDefaultAsync(x => x.Id == _dir.Id, cancellationToken: cancellationToken);
            if (directory_db is null)
            {
                msg = $"Справочник #{_dir.Id} не найден в БД";
                logger.LogError(msg);
                res.AddError(msg);
                return res;
            }
            if (directory_db.Name.Equals(_dir.Name) && directory_db.SystemName.Equals(_dir.SystemName))
            {
                res.AddInfo("Справочник не требует изменения");
            }
            else
            {
                msg = $"Справочник #{_dir.Id} переименован: `{directory_db.Name}` -> `{_dir.Name}`";
                directory_db.Name = _dir.Name;
                directory_db.SystemName = _dir.SystemName;
                context_forms.Update(directory_db);
                await context_forms.SaveChangesAsync(cancellationToken);
                logger.LogInformation(msg);
                res.AddSuccess(msg);
            }
        }
        res.Response = directory_db.Id;
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryModelDB? dir_db = await context_forms.Directories.FirstOrDefaultAsync(x => x.Id == directory_id, cancellationToken: cancellationToken);
        if (dir_db is null)
            return ResponseBaseModel.CreateError($"Список/справочник #{directory_id} не найден в БД");

        context_forms.Remove(dir_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Список/справочник #{directory_id} успешно удалён из БД");
    }
    #endregion
    #region элементы справочникв/списков
    /// <inheritdoc/>
    public async Task<TResponseModel<List<SystemEntryModel>>> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        TResponseModel<List<SystemEntryModel>> res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryModelDB? dir = await context_forms.Directories
            .Include(x => x.Elements)
            .FirstOrDefaultAsync(x => x.Id == directory_id, cancellationToken: cancellationToken);

        if (dir is null)
        {
            res.AddError($"Справочник #{directory_id} не найден в БД");
            return res;
        }

        res.Response = dir.Elements?
            .OrderBy(x => x.SortIndex)
            .Select(x => new SystemEntryModel() { Name = x.Name, SystemName = x.SystemName, Id = x.Id })
            .ToList();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseStrictModel<int>> CreateElementForDirectory(SystemOwnedNameModel element, CancellationToken cancellationToken = default)
    {
        element.Name = Regex.Replace(element.Name, @"\s+", " ").Trim();
        TResponseStrictModel<int> res = new() { Response = 0 };

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(element);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryModelDB? dir = await context_forms.Directories
            .Include(x => x.Elements)
            .FirstOrDefaultAsync(x => x.Id == element.OwnerId, cancellationToken: cancellationToken);

        if (dir is null)
        {
            res.AddError($"Справочник #{element.OwnerId} не найден в БД");
            return res;
        }

        ConstructorFormDirectoryElementModelDB? dictionary_element_db = await context_forms.DirectoriesElements.FirstOrDefaultAsync(x => x.ParentId == element.OwnerId && (x.Name.ToUpper() == element.Name.ToUpper() || x.SystemName.ToUpper() == element.SystemName.ToUpper()), cancellationToken: cancellationToken);

        if (dictionary_element_db is not null)
        {
            res.AddError("Такой элемент справочника уже существует");
            return res;
        }

        int[] current_indexes = await context_forms
            .DirectoriesElements
            .Where(x => x.ParentId == element.OwnerId)
            .Select(x => x.SortIndex)
            .ToArrayAsync(cancellationToken: cancellationToken);

        int current_index = current_indexes.Length == 0
            ? 0
            : current_indexes.Max();

        dictionary_element_db = new() { Name = element.Name, SystemName = element.SystemName, ParentId = dir.Id, Parent = dir, SortIndex = current_index + 1 };

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

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateElementOfDirectory(SystemEntryModel element, CancellationToken cancellationToken = default)
    {
        element.Name = Regex.Replace(element.Name, @"\s+", " ").Trim();
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(element);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        ConstructorFormDirectoryElementModelDB? element_db;

        element_db = await context_forms
            .DirectoriesElements
            .FirstOrDefaultAsync(x => x.Id == element.Id, cancellationToken: cancellationToken);

        if (element_db is null)
        {
            res.AddError($"Элемент справочника #{element.Id} не найден в БД");
            return res;
        }

        if (element_db.Name.Equals(element.Name) && element_db.SystemName.Equals(element.SystemName))
        {
            res.AddInfo("Изменений нет - обновления не требуется");
            return res;
        }

        ConstructorFormDirectoryElementModelDB? duplicate_check;
        IQueryable<ConstructorFormDirectoryElementModelDB> duplicate_check_query = context_forms.DirectoriesElements.Where(x => x.Id != element.Id && x.ParentId == element_db.ParentId && (x.Name.ToUpper() == element.Name.ToUpper() || x.SystemName.ToUpper() == element.SystemName.ToUpper()));
#if DEBUG
        duplicate_check = await duplicate_check_query.FirstOrDefaultAsync();
#endif
        if (await duplicate_check_query.AnyAsync())
        {
            res.AddError("Элемент с таким именем уже существует в справочнике");
            return res;
        }

        element_db.Name = element.Name;
        element_db.SystemName = element.SystemName;
        context_forms.Update(element_db);
        await context_forms.SaveChangesAsync(cancellationToken);
        res.AddSuccess("Элемент справочника успешно обновлён");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteElementFromDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryElementModelDB? el = await context_forms.DirectoriesElements.FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);
        if (el is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");
        context_forms.Remove(el);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Элемент справочника #{element_id} успешно удалён из БД");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpMoveElementOfDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryElementModelDB? el = await context_forms
            .DirectoriesElements
            .Include(x => x.Parent)
            .ThenInclude(x => x!.Elements)
            .FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);

        if (el?.Parent?.Elements is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");

        el.Parent.Elements = [.. el.Parent.Elements.OrderBy(x => x.SortIndex)];

        int i = el.Parent!.Elements!.FindIndex(x => x.Id == element_id);
        if (i == 0)
            return ResponseBaseModel.CreateWarning($"Элемент '{el.Name}' не может быть выше. Он в крайнем положении");

        el.Parent!.Elements![i].SortIndex--;
        el.Parent!.Elements![i - 1].SortIndex++;

        context_forms.UpdateRange(new ConstructorFormDirectoryElementModelDB[] { el.Parent!.Elements![i], el.Parent!.Elements![i - 1] });
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess("Элемент сдвинут");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DownMoveElementOfDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryElementModelDB? el = await context_forms
            .DirectoriesElements
            .Include(x => x.Parent)
            .ThenInclude(x => x!.Elements)
            .FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);

        if (el?.Parent?.Elements is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");

        el.Parent.Elements = [.. el.Parent.Elements.OrderBy(x => x.SortIndex)];

        int i = el.Parent!.Elements!.FindIndex(x => x.Id == element_id);
        if (i == el.Parent!.Elements!.Count - 1)
            return ResponseBaseModel.CreateWarning($"Элемент '{el.Name}' не может быть ниже. Он в крайнем положении");

        el.Parent!.Elements![i].SortIndex++;
        el.Parent!.Elements![i + 1].SortIndex--;

        context_forms.UpdateRange(new ConstructorFormDirectoryElementModelDB[] { el.Parent!.Elements![i], el.Parent!.Elements![i + 1] });
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess("Элемент сдвинут");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CheckAndNormalizeSortIndexForElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        List<ConstructorFormDirectoryElementModelDB> elements = await context_forms
            .DirectoriesElements
            .Where(x => x.ParentId == directory_id)
            .ToListAsync();

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
    public async Task<ConstructorFormsPaginationResponseModel> SelectForms(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormsPaginationResponseModel res = new(req);

        IQueryable<ConstructorFormModelDB> q;

        q = context_forms.Forms.Where(x => x.ProjectId == projectId).OrderBy(x => x.Name);

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
        {
            q = (from _form in q
                 join _field in context_forms.Fields on _form.Id equals _field.OwnerId into ps_field
                 from field in ps_field.DefaultIfEmpty()
                 where
                 EF.Functions.Like(_form.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") ||
                 EF.Functions.Like(field.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") ||
                 EF.Functions.Like(_form.SystemName.ToLower(), $"%{req.SimpleRequest.ToLower()}%") ||
                 EF.Functions.Like(field.SystemName.ToLower(), $"%{req.SimpleRequest.ToLower()}%")
                 group _form by _form into g
                 select g.Key)
             .OrderBy(x => x.Name)
            .AsQueryable();
        }

        res.TotalRowsCount = await q.CountAsync(cancellationToken: cancellationToken);
        q = q.OrderBy(x => x.Id).Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        int[] ids = await q.Select(x => x.Id).ToArrayAsync(cancellationToken: cancellationToken);

        res.Elements = ids.Length != 0
            ? await context_forms.Forms.Where(x => ids.Contains(x.Id)).Include(x => x.Fields).Include(x => x.FormsDirectoriesLinks).ToArrayAsync(cancellationToken: cancellationToken)
            : Enumerable.Empty<ConstructorFormModelDB>();
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormModelDB>> GetForm(int form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<ConstructorFormModelDB> res = new()
        {
            Response = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks!)
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
                List<ConstructorFieldFormModelDB> _upd = [];
                Dictionary<MetadataExtensionsFormFieldsEnum, object?> mdvs;
                foreach (ConstructorFieldFormModelDB f in res.Response.Fields)
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
    public async Task<TResponseModel<ConstructorFormModelDB>> FormUpdateOrCreate(ConstructorFormBaseModel form, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormModelDB> res = new();
        form.Name = Regex.Replace(form.Name, @"\s+", " ").Trim();
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(form);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
#pragma warning disable CA1862 // Используйте перегрузки метода "StringComparison" для сравнения строк без учета регистра
        ConstructorFormModelDB? form_db = await context_forms
            .Forms
            .FirstOrDefaultAsync(x => x.Id != form.Id && x.ProjectId == form.ProjectId && (x.Name.ToUpper() == form.Name.ToUpper() || x.SystemName.ToUpper() == form.SystemName.ToUpper()), cancellationToken: cancellationToken);
#pragma warning restore CA1862 // Используйте перегрузки метода "StringComparison" для сравнения строк без учета регистра

        string msg;
        if (form_db is not null)
        {
            msg = $"Такая форма уже существует: #{form_db.Id} '{form_db.Name}' [{form_db.SystemName}]";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        if (form.Id < 1)
        {
            form_db = ConstructorFormModelDB.Build(form);
            await context_forms.AddAsync(form_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Форма создана #{form_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.Response = form_db;
            return res;
        }

        form_db = await context_forms.Forms.FirstOrDefaultAsync(x => x.Id == form.Id, cancellationToken: cancellationToken);

        if (form_db is null)
        {
            msg = $"Форма #{form.Id} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (form_db.SystemName.Equals(form.SystemName) && form_db.Name.Equals(form.Name) && form_db.Description == form.Description && form_db.Css == form.Css && form_db.AddRowButtonTitle == form.AddRowButtonTitle)
        {
            msg = $"Форма #{form.Id} не требует изменений в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
        }
        else
        {
            form_db.SystemName = form.SystemName;
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
        res.Response = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == form_db.Id, cancellationToken: cancellationToken);

        return res;
    }

    /// <inheritdoc/>
    public Task<ResponseBaseModel> FormDelete(int form_id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ResponseBaseModel.CreateSuccess("Метод не реализован"));
    }
    #endregion
    #region поля форм
    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormModelDB>> FieldFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormModelDB> res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFieldFormModelDB? field_db = await context_forms
            .Fields
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Fields)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_id, cancellationToken: cancellationToken);

        if (field_db is null)
        {
            res.AddError($"Поле формы (простого типа) #{field_id} не найден в БД. ошибка D4B94965-1C93-478E-AC8A-8F75C0D6455E");
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
            ConstructorFieldFormModelDB _fns = field_db.Owner.Fields!.First(x => x.SortIndex == next_index);
            res.AddInfo($"Поля формы меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' (простой тип) && #{field_db.Id} i:{field_db.SortIndex} '{field_db.Name}' (простой тип)");

            _fns.SortIndex = field_db.SortIndex;
            field_db.SortIndex = next_index;
            context_forms.Update(field_db);
            context_forms.Update(_fns);
        }
        else if (field_db.Owner.FormsDirectoriesLinks!.Any(x => x.SortIndex == next_index))
        {
            ConstructorFormDirectoryLinkModelDB _fnsd = field_db.Owner.FormsDirectoriesLinks!.First(x => x.SortIndex == next_index);
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
                .Include(x => x.FormsDirectoriesLinks)
                .FirstAsync(x => x.Id == field_db.OwnerId, cancellationToken: cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormModelDB>> FieldDirectoryFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormModelDB> res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryLinkModelDB? field_db = await context_forms
            .FormsDirectoriesLinks
            //.Include(x => x.Directory)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Fields)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_id, cancellationToken: cancellationToken);


        if (field_db is null)
        {
            res.AddError($"Поле формы (тип: справочник) #{field_id} не найден в БД. ошибка 39253612-8DD9-40B3-80AA-CF6589288E06");
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
            ConstructorFieldFormModelDB _fns = field_db.Owner.Fields!.First(x => x.SortIndex == next_index);
            res.AddInfo($"Поля формы меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' (простой тип) && #{field_db.Id} i:{field_db.SortIndex} '{field_db.Name}' (тип: справочник)");

            _fns.SortIndex = field_db.SortIndex;
            field_db.SortIndex = next_index;
            context_forms.Update(field_db);
            context_forms.Update(_fns);
        }
        else if (field_db.Owner.FormsDirectoriesLinks!.Any(x => x.SortIndex == next_index))
        {
            ConstructorFormDirectoryLinkModelDB _fnsd = field_db.Owner.FormsDirectoriesLinks!.First(x => x.SortIndex == next_index);
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
                .Include(x => x.FormsDirectoriesLinks)
                .FirstAsync(x => x.Id == field_db.OwnerId, cancellationToken: cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldUpdateOrCreate(ConstructorFieldFormBaseModel form_field, CancellationToken cancellationToken = default)
    {
        form_field.Name = Regex.Replace(form_field.Name, @"\s+", " ").Trim();
        form_field.MetadataValueType = form_field.MetadataValueType?.Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(form_field);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormModelDB? form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == form_field.OwnerId, cancellationToken: cancellationToken);

        string msg;
        if (form_db?.Fields is null || form_db.FormsDirectoriesLinks is null)
        {
            msg = $"Форма #{form_field.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        ConstructorFieldFormBaseLowModel? duplicate_field = form_db.AllFields.FirstOrDefault(x => (x.GetType() == typeof(ConstructorFormDirectoryLinkModelDB) && (x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase) || x.SystemName.Equals(form_field.SystemName, StringComparison.OrdinalIgnoreCase))) || (x.GetType() == typeof(ConstructorFieldFormModelDB)) && x.Id != form_field.Id && (x.SystemName.Equals(form_field.SystemName, StringComparison.OrdinalIgnoreCase) || x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase)));
        if (duplicate_field is not null)
            return ResponseBaseModel.CreateError($"Поле с таким именем уже существует: '{duplicate_field.Name}' `{duplicate_field.SystemName}`");

        ConstructorFieldFormModelDB? form_field_db;
        if (form_field.Id < 1)
        {
            int _sort_index = form_db.FormsDirectoriesLinks.Count != 0
                ? form_db.FormsDirectoriesLinks.Max(x => x.SortIndex)
                : 0;

            _sort_index = Math.Max(_sort_index, form_db.Fields.Count != 0 ? form_db.Fields.Max(x => x.SortIndex) : 0);

            form_field_db = ConstructorFieldFormModelDB.Build(form_field, form_db, _sort_index + 1);

            await context_forms.AddAsync(form_field_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Поле (простого типа) создано #{form_field_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        form_field_db = form_db.Fields.FirstOrDefault(x => x.Id == form_field.Id);

        if (form_field_db is null)
        {
            msg = $"Поле (простого типа) формы #{form_field.Id} не найдено в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context_forms.Database.BeginTransaction(isolationLevel: System.Data.IsolationLevel.Serializable);

        List<ConstructorFormSessionValueModelDB> values_updates = await (from val_s in context_forms.ValuesSessions.Where(x => x.Name == form_field.Name)
                                                                         join session in context_forms.Sessions on val_s.OwnerId equals session.Id
                                                                         join Questionnaire in context_forms.Questionnaires on session.OwnerId equals Questionnaire.Id
                                                                         join page in context_forms.QuestionnairesPages on Questionnaire.Id equals page.OwnerId
                                                                         join form_join in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == form_db.Id) on page.Id equals form_join.OwnerId
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

        if (form_field_db.SystemName != form_field.SystemName)
        {
            msg = $"Системное имя поля (простого типа) формы #{form_field.Id} изменилось: [{form_field_db.Name}] -> [{form_field.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.SystemName = form_field.SystemName;
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
                msg = $"Ошибка обновления поля #{form_field_db.Id} '{form_field_db.Name}' `{form_field_db.SystemName}` (форма #{form_db.Id} '{form_db.Name}' `{form_db.SystemName}`)";
                res.AddError(msg);
                res.Messages.InjectException(ex);
                logger.LogError(ex, msg);
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(ConstructorFormDirectoryLinkModelDB field_directory, CancellationToken cancellationToken = default)
    {
        field_directory.Name = Regex.Replace(field_directory.Name, @"\s+", " ").Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(field_directory);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryModelDB? dir_db = await context_forms.Directories.FirstOrDefaultAsync(x => x.Id == field_directory.DirectoryId, cancellationToken: cancellationToken);

        if (dir_db is null)
            return ResponseBaseModel.CreateError($"Не найден справочник #{field_directory.DirectoryId}");

        ConstructorFormModelDB? form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_directory.OwnerId, cancellationToken: cancellationToken);
        string msg;
        if (form_db?.Fields is null || form_db.FormsDirectoriesLinks is null)
        {
            msg = $"Форма #{field_directory.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        ConstructorFieldFormBaseLowModel? duplicate_field = form_db.AllFields.FirstOrDefault(x => (x.GetType() == typeof(ConstructorFormDirectoryLinkModelDB) && x.Id != field_directory.Id && (x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase) || x.SystemName.Equals(field_directory.SystemName, StringComparison.OrdinalIgnoreCase))) || (x.GetType() == typeof(ConstructorFieldFormModelDB)) && (x.SystemName.Equals(field_directory.SystemName, StringComparison.OrdinalIgnoreCase) || x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase)));
        if (duplicate_field is not null)
            return ResponseBaseModel.CreateError($"Поле с таким именем уже существует: '{duplicate_field.Name}' `{duplicate_field.SystemName}`");

        ConstructorFormDirectoryLinkModelDB? form_field_db;
        if (field_directory.Id < 1)
        {
            int _sort_index = form_db.FormsDirectoriesLinks.Count != 0 ? form_db.FormsDirectoriesLinks.Max(x => x.SortIndex) : 0;
            _sort_index = Math.Max(_sort_index, form_db.Fields.Count != 0 ? form_db.Fields.Max(x => x.SortIndex) : 0);

            form_field_db = new()
            {
                SystemName = field_directory.SystemName,
                Name = field_directory.Name,
                Css = field_directory.Css,
                OwnerId = field_directory.OwnerId,
                Hint = field_directory.Hint,
                Required = field_directory.Required,
                Owner = form_db,
                Description = field_directory.Description,
                Directory = dir_db,
                DirectoryId = dir_db.Id,
                SortIndex = _sort_index + 1
            };
            await context_forms.AddAsync(form_field_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Поле (списочного типа) создано #{form_field_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        form_field_db = form_db.FormsDirectoriesLinks.FirstOrDefault(x => x.Id == field_directory.Id);
        if (form_field_db is null)
        {
            msg = $"Поле (списочного типа) формы #{field_directory.Id} не найдено в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context_forms.Database.BeginTransaction(isolationLevel: System.Data.IsolationLevel.Serializable);

        List<ConstructorFormSessionValueModelDB> values_updates = await (from val_s in context_forms.ValuesSessions.Where(x => x.Name == field_directory.Name)
                                                                         join session in context_forms.Sessions on val_s.OwnerId equals session.Id
                                                                         join Questionnaire in context_forms.Questionnaires on session.OwnerId equals Questionnaire.Id
                                                                         join page in context_forms.QuestionnairesPages on Questionnaire.Id equals page.OwnerId
                                                                         join form_join in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == form_db.Id) on page.Id equals form_join.OwnerId
                                                                         select val_s)
                                                                     .ToListAsync(cancellationToken: cancellationToken);

        if (form_field_db.DirectoryId != field_directory.DirectoryId)
        {
            if (values_updates.Count != 0)
            {
                msg = $"Тип поля (списочного типа) формы #{field_directory.Id} не может быть изменён ([{form_field_db.DirectoryId}] -> [{field_directory.DirectoryId}]): найдены ссылки введёных значений ({values_updates.Count} штук) для этого поля";
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

        if (form_field_db.SystemName != field_directory.SystemName)
        {
            msg = $"Системное имя поля (списочного типа) формы #{field_directory.Id} изменилось: [{form_field_db.SystemName}] -> [{field_directory.SystemName}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.SystemName = field_directory.SystemName;
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
                msg = $"Ошибка обновления поля #{form_field_db.Id} '{form_field_db.Name}' `{form_field_db.SystemName}` (форма #{form_db.Id} '{form_db.Name}' `{form_db.SystemName}`)";
                res.AddError(msg);
                res.Messages.InjectException(ex);
                logger.LogError(ex, msg);
            }
        }
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDelete(int form_field_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFieldFormModelDB? field_db = await context_forms.Fields.FirstOrDefaultAsync(x => x.Id == form_field_id, cancellationToken: cancellationToken);
        if (field_db is null)
            return ResponseBaseModel.CreateError($"Поле #{form_field_id} (простого типа) формы не найден в БД");

        IQueryable<ConstructorFormSessionValueModelDB> values = from _v in context_forms.ValuesSessions.Where(x => x.Name == field_db.Name)
                                                                join _jf in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == field_db.OwnerId) on _v.QuestionnairePageJoinFormId equals _jf.Id
                                                                select _v;

        if (await values.AnyAsync(cancellationToken: cancellationToken))
            context_forms.RemoveRange(values);

        context_forms.Remove(field_db);

        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Поле '{field_db.Name}' {{простого типа}} удалено из формы");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryDelete(int field_directory_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormDirectoryLinkModelDB? field_db = await context_forms.FormsDirectoriesLinks.FirstOrDefaultAsync(x => x.Id == field_directory_id, cancellationToken: cancellationToken);
        if (field_db is null)
            return ResponseBaseModel.CreateError($"Поле #{field_directory_id} (тип: справочник) формы не найден в БД");
        context_forms.Remove(field_db);

        IQueryable<ConstructorFormSessionValueModelDB> values = from _v in context_forms.ValuesSessions.Where(x => x.Name == field_db.Name)
                                                                join _jf in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == field_db.OwnerId) on _v.QuestionnairePageJoinFormId equals _jf.Id
                                                                select _v;

        if (await values.AnyAsync(cancellationToken: cancellationToken))
            context_forms.RemoveRange(values);

        await context_forms.SaveChangesAsync(cancellationToken);
        return ResponseBaseModel.CreateSuccess("Поле {справочник/список} удалено из формы");
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormModelDB>> CheckAndNormalizeSortIndexFrmFields(ConstructorFormModelDB form, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormModelDB> res = new();
        int i = 0;
        List<ConstructorFormDirectoryLinkModelDB> fields_dir = [];
        List<ConstructorFieldFormModelDB> fields_st = [];

        foreach (ConstructorFieldFormBaseLowModel fb in form.AllFields)
        {
            i++;
            if (fb is ConstructorFormDirectoryLinkModelDB fs)
            {
                if (i != fs.SortIndex)
                {
                    res.AddWarning($"Исправление пересортицы '{fs.Name}': [{fs.SortIndex}] -> [{i}]");
                    fs.SortIndex = i;
                    fields_dir.Add(fs);
                }
            }
            else if (fb is ConstructorFieldFormModelDB fd)
            {
                if (i != fd.SortIndex)
                {
                    res.AddWarning($"Исправление пересортицы '{fd.Name}': [{fd.SortIndex}] -> [{i}]");
                    fd.SortIndex = i;
                    fields_st.Add(fd);
                }
            }
        }
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        if (fields_st.Count != 0)
            context_forms.UpdateRange(fields_st);
        if (fields_dir.Count != 0)
            context_forms.UpdateRange(fields_dir);

        if (fields_st.Count != 0 || fields_dir.Count != 0)
            await context_forms.SaveChangesAsync(cancellationToken);
        else
            res.AddInfo("Пересортица отсутствует");

        res.Response = context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks!)
            .ThenInclude(x => x.Directory)
            .FirstOrDefault(x => x.Id == form.Id);

        return res;
    }
    #endregion

    /////////////// Документ. Описывается/настраивается конечный результат, который будет использоваться.
    // Может содержать одну или несколько вкладок/табов. На каждом табе/вкладке может располагаться одна или больше форм
    // Каждая располагаемая форма может быть помечена как [Табличная]. Т.е. пользователь будет добавлять сколь угодно строк одной и той же формы.
    // Пользователь при добавлении/редактировании строк таблицы будет видеть форму, которую вы настроили для этого, а внутри таба это будет выглядеть как обычная многострочная таблица с колонками, равными полям формы
    #region документы
    /// <inheritdoc/>
    public async Task<ConstructorFormsQuestionnairesPaginationResponseModel> RequestQuestionnaires(SimplePaginationRequestModel req, int projectId, CancellationToken cancellationToken)
    {
        if (req.PageSize < 1)
            req.PageSize = 10;

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormsQuestionnairesPaginationResponseModel res = new(req);

        IQueryable<ConstructorFormQuestionnaireModelDB> query = context_forms
            .Questionnaires
            .Where(x => x.ProjectId == projectId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
        {
            query = (from _quest in query
                     join _page in context_forms.QuestionnairesPages on _quest.Id equals _page.OwnerId into j_pages
                     from j_page in j_pages.DefaultIfEmpty()
                     join _join_form in context_forms.QuestionnairesPagesJoinForms on j_page.Id equals _join_form.OwnerId into j_join_forms
                     from j_join_form in j_join_forms.DefaultIfEmpty()
                     join _form in context_forms.Forms on j_join_form.FormId equals _form.Id into j_forms
                     from j_form in j_forms.DefaultIfEmpty()
                     where EF.Functions.Like(_quest.Name.ToUpper(), $"%{req.SimpleRequest.ToUpper()}%") || EF.Functions.Like(j_form.Name.ToUpper(), $"%{req.SimpleRequest.ToUpper()}%") || EF.Functions.Like(j_page.Name.ToUpper(), $"%{req.SimpleRequest.ToUpper()}%")
                     group _quest by _quest into g
                     select g.Key)
                        .AsQueryable();
        }

        //IQueryable<ConstructorFormQuestionnaireModelDB> query =
        //    (from _quest in context_forms.Questionnaires.Where(x => x.ProjectId == projectId)
        //     join _page in context_forms.QuestionnairesPages on _quest.Id equals _page.OwnerId
        //     join _join_form in context_forms.QuestionnairesPagesJoinForms on _page.Id equals _join_form.OwnerId
        //     join _form in context_forms.Forms on _join_form.FormId equals _form.Id
        //     where string.IsNullOrWhiteSpace(req.SimpleRequest) || EF.Functions.Like(_form.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") || EF.Functions.Like(_quest.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") || EF.Functions.Like(_page.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%")
        //     group _quest by _quest into g
        //     select g.Key)
        //    .AsQueryable();

        res.TotalRowsCount = await query.CountAsync(cancellationToken: cancellationToken);
        query = query.OrderBy(x => x.Id).Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        int[] ids = await query.Select(x => x.Id).ToArrayAsync(cancellationToken: cancellationToken);
        query = context_forms.Questionnaires.Include(x => x.Pages).Where(x => ids.Contains(x.Id)).Include(x => x.Pages).OrderBy(x => x.Name);
        res.Questionnaires = ids.Length != 0
            ? await query.ToArrayAsync(cancellationToken: cancellationToken)
            : Enumerable.Empty<ConstructorFormQuestionnaireModelDB>();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormQuestionnaireModelDB>> GetQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<ConstructorFormQuestionnaireModelDB> res = new()
        {
            Response = await context_forms
           .Questionnaires
           .Include(x => x.Pages!)
           .ThenInclude(x => x.JoinsForms)
           .FirstOrDefaultAsync(x => x.Id == questionnaire_id, cancellationToken: cancellationToken)
        };

        if (res.Response is null)
            res.AddError($"Опрос/анкета #{questionnaire_id} отсутствует в БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormQuestionnaireModelDB>> UpdateOrCreateQuestionnaire(EntryConstructedModel questionnaire, CancellationToken cancellationToken = default)
    {
        questionnaire.Name = Regex.Replace(questionnaire.Name, @"\s+", " ").Trim();

        TResponseModel<ConstructorFormQuestionnaireModelDB> res = new();
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(questionnaire);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnaireModelDB? questionnaire_db = await context_forms.Questionnaires.FirstOrDefaultAsync(x => x.Id != questionnaire.Id && x.ProjectId == questionnaire.ProjectId && (x.Name.ToUpper() == questionnaire.Name.ToUpper() || x.SystemName.ToUpper() == questionnaire.SystemName.ToUpper()), cancellationToken: cancellationToken);
        string msg;
        if (questionnaire_db is not null)
        {
            msg = $"Опрос/анкета с таким именем уже существует: #{questionnaire_db.Id} '{questionnaire_db.Name}' `{questionnaire_db.SystemName}`";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        if (questionnaire.Id < 1)
        {
            questionnaire_db = ConstructorFormQuestionnaireModelDB.Build(questionnaire, questionnaire.ProjectId);
            await context_forms.AddAsync(questionnaire_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Опрос/анкета создана #{questionnaire_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.Response = questionnaire_db;
            return res;
        }
        questionnaire_db = await context_forms.Questionnaires.FirstOrDefaultAsync(x => x.Id == questionnaire.Id, cancellationToken: cancellationToken);

        if (questionnaire_db is null)
        {
            msg = $"Опрос/анкета #{questionnaire.Id} отсутствует в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (questionnaire_db.SystemName == questionnaire.SystemName && questionnaire_db.Name == questionnaire.Name && questionnaire_db.Description == questionnaire.Description)
        {
            msg = $"Опрос/анкета #{questionnaire.Id} не требует изменений в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
        }
        else
        {
            questionnaire_db.SystemName = questionnaire.SystemName;
            questionnaire_db.Name = questionnaire.Name;
            questionnaire_db.Description = questionnaire.Description;
            context_forms.Update(questionnaire_db);
            msg = $"Опрос/анкета #{questionnaire.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        return await GetQuestionnaire(questionnaire_db.Id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnaireModelDB? Questionnaire_db = await context_forms.Questionnaires.Include(x => x.Pages!).ThenInclude(x => x.JoinsForms).FirstOrDefaultAsync(x => x.Id == questionnaire_id, cancellationToken: cancellationToken);
        if (Questionnaire_db is null)
            return ResponseBaseModel.CreateError($"Опрос/анкета #{questionnaire_id} не найдена в БД");

        context_forms.Remove(Questionnaire_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Опрос/анкета #{questionnaire_id} удалена");
    }
    #endregion
    // табы/вкладки схожи по смыслу табов/вкладок в Excel. Т.е. обычная группировка разных рабочих пространств со своим именем 
    #region табы документов
    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormQuestionnaireModelDB>> QuestionnairePageMove(int questionnaire_page_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormQuestionnaireModelDB> res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnairePageModelDB? questionnaire_db = await context_forms
            .QuestionnairesPages
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Pages)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_id, cancellationToken: cancellationToken);

        if (questionnaire_db is null)
        {
            res.AddError($"Страница опроса/анкеты #{questionnaire_page_id} отсутствует в БД. ошибка B68F235F-B4B3-4CB7-A77A-BAA007A7C412");
            return res;
        }
        questionnaire_db.Owner!.Pages = questionnaire_db.Owner.Pages!.OrderBy(x => x.SortIndex).ToList();

        ConstructorFormQuestionnairePageModelDB? _fns = questionnaire_db.Owner.GetOutermostPage(direct, questionnaire_db.SortIndex);

        if (_fns is null)
            res.AddError("Не удалось выполнить перемещение. ошибка 7BA66820-1DDF-42B0-AF6D-F8F0C920A40E");
        else
        {
            res.AddInfo($"Страницы опроса/анкеты меняются индексами сортировки: #{_fns.Id} i:{_fns.SortIndex} '{_fns.Name}' && #{questionnaire_db.Id} i:{questionnaire_db.SortIndex} '{questionnaire_db.Name}'");
            int next_index = _fns.SortIndex;
            int tmp_id = questionnaire_db.SortIndex;
            if (direct == VerticalDirectionsEnum.Down)
            {
                questionnaire_db.SortIndex = r.Next(10000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1), 50000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1));
            }
            else
            {
                questionnaire_db.SortIndex = r.Next(50000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1), 10000 * (direct == VerticalDirectionsEnum.Down ? 1 : -1));
            }
            context_forms.Update(questionnaire_db);
            await context_forms.SaveChangesAsync(cancellationToken);
            _fns.SortIndex = tmp_id;
            context_forms.Update(_fns);
            await context_forms.SaveChangesAsync(cancellationToken);
            questionnaire_db.SortIndex = next_index;
            context_forms.Update(questionnaire_db);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        if (!res.Success())
            return res;

        res.Response = await context_forms
            .Questionnaires
            .Include(x => x.Pages)
            .FirstAsync(x => x.Id == questionnaire_db.OwnerId, cancellationToken: cancellationToken);
        questionnaire_db.Owner!.Pages = questionnaire_db.Owner.Pages!.OrderBy(x => x.SortIndex).ToList();

        int i = 0;
        bool is_upd = false;
        foreach (ConstructorFormQuestionnairePageModelDB p in res.Response.Pages!)
        {
            i++;
            is_upd = is_upd || p.SortIndex != i;
            p.SortIndex = i;
        }

        if (is_upd)
        {
            res.AddWarning("Исправлена пересортица");
            context_forms.UpdateRange(res.Response.Pages!);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormQuestionnairePageResponseModel> CreateOrUpdateQuestionnairePage(EntryDescriptionOwnedModel questionnaire_page, CancellationToken cancellationToken = default)
    {
        if (questionnaire_page.Id < 0)
            questionnaire_page.Id = 0;

        questionnaire_page.Name = Regex.Replace(questionnaire_page.Name, @"\s+", " ").Trim();

        FormQuestionnairePageResponseModel res = new();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(questionnaire_page);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnaireModelDB? questionnaire_db = await context_forms
            .Questionnaires
            .Include(x => x.Pages)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page.OwnerId, cancellationToken: cancellationToken);
        string msg;
        if (questionnaire_db is null)
        {
            msg = $"Анкета/опрос #{questionnaire_page.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        ConstructorFormQuestionnairePageModelDB? questionnaire_page_db;
        if (questionnaire_page.Id < 1)
        {
            int _sort_index = questionnaire_db.Pages!.Any() ? questionnaire_db.Pages!.Max(x => x.SortIndex) : 0;

            questionnaire_page_db = ConstructorFormQuestionnairePageModelDB.Build(questionnaire_page, questionnaire_db, _sort_index + 1);

            await context_forms.AddAsync(questionnaire_page_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Страница анкеты/опроса создано #{questionnaire_page_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.QuestionnairePage = questionnaire_page_db;
            return res;
        }

        questionnaire_page_db = questionnaire_db.Pages!.FirstOrDefault(x => x.Id == questionnaire_page.Id);

        if (questionnaire_page_db is null)
        {
            msg = $"Страница опроса/анкеты #{questionnaire_page.Id} не найдено в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (questionnaire_page_db.Name != questionnaire_page.Name)
        {
            msg = $"Имя страницы опроса/анкеты #{questionnaire_page.Id} изменилось: [{questionnaire_page_db.Name}] -> [{questionnaire_page.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_db.Name = questionnaire_page.Name;
        }
        if (questionnaire_page_db.Description != questionnaire_page.Description)
        {
            msg = $"Описание страницы опроса/анкеты #{questionnaire_page.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_db.Description = questionnaire_page.Description;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            context_forms.Update(questionnaire_page_db);
            msg = $"Страница опроса/анкеты #{questionnaire_page.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        res.QuestionnairePage = questionnaire_page_db;
        return res;
    }

    /// <inheritdoc/>
    public async Task<FormQuestionnairePageResponseModel> GetQuestionnairePage(int questionnaire_page_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        FormQuestionnairePageResponseModel res = new()
        {
            QuestionnairePage = await context_forms
           .QuestionnairesPages

           .Include(x => x.JoinsForms!)
           .ThenInclude(x => x.Form)
           .ThenInclude(x => x!.Fields)

           .Include(x => x.JoinsForms!)
           .ThenInclude(x => x.Form)
           .ThenInclude(x => x!.FormsDirectoriesLinks)

           .FirstOrDefaultAsync(x => x.Id == questionnaire_page_id, cancellationToken: cancellationToken)
        };

        if (res.QuestionnairePage is null)
            res.AddError($"Форма #{questionnaire_page_id} не найдена в БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteQuestionnairePage(int questionnaire_page_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnairePageModelDB? questionnaire_page_db = await context_forms
            .QuestionnairesPages
            .Include(x => x.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_id, cancellationToken: cancellationToken);

        if (questionnaire_page_db is null)
            return ResponseBaseModel.CreateError($"Страница опроса/анкеты #{questionnaire_page_id} не найден в БД");

        context_forms.Remove(questionnaire_page_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Страница #{questionnaire_page_id} удалена из опроса/анкеты");
    }
    #endregion
    #region структура/схема таба/вкладки: формы, порядок и настройки поведения
    /// <inheritdoc/>
    public async Task<FormQuestionnairePageResponseModel> QuestionnairePageJoinFormMove(int questionnaire_page_join_form_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        FormQuestionnairePageResponseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnairePageJoinFormModelDB? questionnaire_page_join_db = await context_forms
            .QuestionnairesPagesJoinForms
            .Include(x => x.Owner)
            .ThenInclude(x => x!.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_join_form_id, cancellationToken: cancellationToken);

        if (questionnaire_page_join_db?.Owner?.JoinsForms is null)
        {
            res.AddError($"Форма для страницы опроса/анкеты #{questionnaire_page_join_form_id} отсутствует в БД. ошибка 66CB7541-20AE-4C26-A020-3A9546457C3D");
            return res;
        }

        ConstructorFormQuestionnairePageJoinFormModelDB? _fns = questionnaire_page_join_db.Owner.GetOutermostJoinForm(direct, questionnaire_page_join_db.SortIndex);

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

        res.QuestionnairePage = await context_forms
            .QuestionnairesPages
            .Include(x => x.JoinsForms)
            .FirstAsync(x => x.Id == questionnaire_page_join_db.OwnerId, cancellationToken: cancellationToken);

        questionnaire_page_join_db.Owner.JoinsForms = [.. questionnaire_page_join_db.Owner.JoinsForms!.OrderBy(x => x.SortIndex)];

        int i = 0;
        bool is_upd = false;
        foreach (ConstructorFormQuestionnairePageJoinFormModelDB p in res.QuestionnairePage.JoinsForms!)
        {
            i++;
            is_upd = is_upd || p.SortIndex != i;
            p.SortIndex = i;
        }

        if (is_upd)
        {
            res.AddWarning("Исправлена пересортица");
            context_forms.UpdateRange(res.QuestionnairePage.JoinsForms);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateOrUpdateQuestionnairePageJoinForm(ConstructorFormQuestionnairePageJoinFormModelDB page_join_form, CancellationToken cancellationToken = default)
    {
        page_join_form.Name = Regex.Replace(page_join_form.Name, @"\s+", " ").Trim();
        page_join_form.Description = page_join_form.Description?.Trim();

        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(page_join_form);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        ResponseBaseModel res = new();
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnairePageModelDB? questionnaire_page_db = await context_forms
            .QuestionnairesPages
            .Include(x => x.Owner)
            //.ThenInclude(x => x!.Pages)
            .Include(x => x.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == page_join_form.OwnerId, cancellationToken: cancellationToken);

        string msg;
        if (questionnaire_page_db is null)
        {
            msg = $"Страница анкеты/опроса #{page_join_form.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        ConstructorFormQuestionnairePageJoinFormModelDB? questionnaire_page_join_db;
        if (page_join_form.Id < 1)
        {
            int _sort_index = questionnaire_page_db.JoinsForms!.Any() ? questionnaire_page_db.JoinsForms!.Max(x => x.SortIndex) : 0;

            questionnaire_page_join_db = ConstructorFormQuestionnairePageJoinFormModelDB.Build(page_join_form);

            await context_forms.AddAsync(questionnaire_page_join_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Создана связь форма и страницы анкеты/опроса #{questionnaire_page_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        questionnaire_page_join_db = questionnaire_page_db.JoinsForms!.FirstOrDefault(x => x.Id == page_join_form.Id);

        if (questionnaire_page_join_db is null)
        {
            msg = $"Связь формы и страницы опроса/анкеты #{page_join_form.Id} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        if (questionnaire_page_join_db.Name != page_join_form.Name)
        {
            msg = $"Имя связи формы и страницы опроса/анкеты #{page_join_form.Id} изменилось: [{questionnaire_page_join_db.Name}] -> [{page_join_form.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.Name = page_join_form.Name;
        }
        if (questionnaire_page_join_db.Description != page_join_form.Description)
        {
            msg = $"Описание связи формы и страницы опроса/анкеты #{page_join_form.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.Description = page_join_form.Description;
        }
        if (questionnaire_page_join_db.ShowTitle != page_join_form.ShowTitle)
        {
            msg = $"Описание связи формы и страницы опроса/анкеты #{page_join_form.Id} изменение '{nameof(questionnaire_page_join_db.ShowTitle)}': [{questionnaire_page_join_db.ShowTitle}] => [{page_join_form.ShowTitle}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.ShowTitle = page_join_form.ShowTitle;
        }
        if (questionnaire_page_join_db.IsTable != page_join_form.IsTable)
        {
            msg = $"Признак [таблица] связи формы и страницы опроса/анкеты #{page_join_form.Id} изменение '{nameof(questionnaire_page_join_db.IsTable)}': [{questionnaire_page_join_db.IsTable}] => [{page_join_form.IsTable}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            questionnaire_page_join_db.IsTable = page_join_form.IsTable;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            context_forms.Update(questionnaire_page_join_db);
            msg = $"Связь формы и страницы опроса/анкеты #{page_join_form.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormQuestionnairePageJoinFormModelDB>> GetQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<ConstructorFormQuestionnairePageJoinFormModelDB> res = new()
        {
            Response = await context_forms
            .QuestionnairesPagesJoinForms
            .Include(x => x.Form)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_join_form_id, cancellationToken: cancellationToken)
        };

        if (res.Response is null)
            res.AddError($"Связь #{questionnaire_page_join_form_id} (форма<->опрос/анкета) не найдена в БД");
        else if (res.Response.Owner?.JoinsForms is not null)
            res.Response.Owner.JoinsForms = res.Response.Owner.JoinsForms.OrderBy(x => x.SortIndex).ToList();

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormQuestionnairePageJoinFormModelDB? questionnaire_page_db = await context_forms
            .QuestionnairesPagesJoinForms
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_join_form_id, cancellationToken: cancellationToken);

        if (questionnaire_page_db is null)
            return ResponseBaseModel.CreateError($"Связь формы и страницы опроса/анкеты #{questionnaire_page_join_form_id} не найдена в БД");

        context_forms.Remove(questionnaire_page_db);
        await context_forms.SaveChangesAsync(cancellationToken);
        return ResponseBaseModel.CreateSuccess("Связь формы и страницы удалена из опроса/анкеты");
    }
    #endregion

    /////////////// Пользовательский/публичный доступ к возможностям заполнения документа данными
    // Если у вас есть готовый к заполнению документ со всеми его табами и настройками, то вы можете создавать уникальные ссылки для заполнения данными
    // Каждая ссылка это всего лишь уникальный GUID к которому привязываются все данные, которые вводят конечные пользователи
    // Пользователи видят ваш документ, но сам документ данные не хранит. Хранение данных происходит в сессиях, которые вы сами выпускаете для любого вашего документа
    #region сессии документов (данные заполнения документов).
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetStatusSessionQuestionnaire(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormSessionModelDB? sq = await context_forms.Sessions.FirstOrDefaultAsync(x => x.Id == id_session, cancellationToken: cancellationToken);
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
    public async Task<TResponseModel<ConstructorFormSessionModelDB>> GetSessionQuestionnaire(int id_session, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        TResponseModel<ConstructorFormSessionModelDB> res = new()
        {
            Response = await context_forms
            .Sessions
            .Include(x => x.SessionValues)

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Pages!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.Fields) // поля

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Pages!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.FormsDirectoriesLinks) // поля
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == id_session, cancellationToken: cancellationToken)
        };
        string msg;
        if (res.Response is null)
        {
            msg = $"for {nameof(id_session)} = [{id_session}]. SessionQuestionnaire is null. error 965BED19-5E30-4AA5-8FBD-1B3EFEFC5B1D";
            logger.LogError(msg);
            res.AddError(msg);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ConstructorFormSessionModelDB>> UpdateOrCreateSessionQuestionnaire(ConstructorFormSessionModelDB session_json, CancellationToken cancellationToken = default)
    {
        TResponseModel<ConstructorFormSessionModelDB> res = new();

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
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        if (session_json.Id < 1)
        {
            if (!MailAddress.TryCreate(session_json.CreatorEmail, out _))
            {
                res.AddError("Ошибка! Ваш email не корректен. 4BFD1BFC-CAAE-4508-9FFE-20562661C368");
                return res;
            }

            session_json.CreatedAt = DateTime.Now;
            session_json.DeadlineDate = DateTime.Now.AddMinutes(_conf.Value.TimeActualityQuestionnaireSessionMinutes);
            session_json.SessionToken = Guid.NewGuid().ToString();

            await context_forms.AddAsync(session_json, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            return new() { Response = session_json, Messages = new() { new() { TypeMessage = ResultTypesEnum.Success, Text = $"Создана сессия #{session_json.Id}" } } };
        }
        ConstructorFormSessionModelDB? session_db = await context_forms.Sessions.FirstOrDefaultAsync(x => x.Id == session_json.Id, cancellationToken: cancellationToken);
        string msg;
        if (session_db is null)
        {
            msg = $"Сессия #{session_json.Id} не найдена в БД";
            logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        session_db.CreatedAt = session_db.CreatedAt;
        session_db.DeadlineDate = session_db.DeadlineDate;
        session_db.LastQuestionnaireUpdateActivity = session_db.LastQuestionnaireUpdateActivity;

        if (string.IsNullOrWhiteSpace(session_db.CreatorEmail))
        {
            logger.LogWarning($"Пустое значение создателя для сессии [#{session_db.Id} token:{session_db.SessionToken}]. Попытка установить текущего пользователя как автора: '{session_db.CreatorEmail}'");
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
        TResponseModel<ConstructorFormSessionModelDB> sr = await GetSessionQuestionnaire(session_json.Id, cancellationToken);
        if (res.Messages.Count != 0)
            sr.AddRangeMessages(res.Messages);
        return sr;
    }

    /// <inheritdoc/>
    public async Task<ConstructorFormsSessionsPaginationResponseModel> RequestSessionsQuestionnaires(RequestSessionsQuestionnairesRequestPaginationModel req, CancellationToken cancellationToken = default)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormsSessionsPaginationResponseModel res = new(req);
        IQueryable<ConstructorFormSessionModelDB> query = context_forms
            .Sessions
            .Include(x => x.Owner)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (req.QuestionnaireId > 0)
            query = query.Where(x => x.OwnerId == req.QuestionnaireId);

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
            query = query.Where(x => (x.Name != null && EF.Functions.Like(x.Name.ToLower(), $"%{req.SimpleRequest.ToLower()}%") || (x.SessionToken != null && x.SessionToken.ToLower() == req.SimpleRequest.ToLower()) || (!string.IsNullOrWhiteSpace(x.EmailsNotifications) && EF.Functions.Like(x.EmailsNotifications.ToLower(), $"%{req.SimpleRequest.ToLower()}%"))));

        res.TotalRowsCount = await query.CountAsync(cancellationToken: cancellationToken);
        query = query.Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        res.Sessions = await query.ToArrayAsync(cancellationToken: cancellationToken);
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<EntryDictModel[]>> FindSessionsQuestionnairesByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default)
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
                join _pjf in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == req.FormId) on _vs.QuestionnairePageJoinFormId equals _pjf.Id
                join _qp in context_forms.QuestionnairesPages on _pjf.OwnerId equals _qp.Id
                select new { Value = _vs, Session = _s, QuestionnairePageJoinForm = _pjf, QuestionnairePage = _qp };

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
                    { nameof(element_g.Session.CreatorEmail), element_g.Session.CreatorEmail },
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
                if (element_g.Session.LastQuestionnaireUpdateActivity is not null)
                    _d.Add(nameof(element_g.Session.LastQuestionnaireUpdateActivity), element_g.Session.LastQuestionnaireUpdateActivity);

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
        IQueryable<ConstructorFormSessionValueModelDB> q = from _vs in context_forms.ValuesSessions.Where(_vs => _vs.Name == req.FieldName)
                                                           join _s in context_forms.Sessions.Where(x => !req.SessionId.HasValue || x.Id == req.SessionId.Value) on _vs.OwnerId equals _s.Id
                                                           join _pjf in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == req.FormId) on _vs.QuestionnairePageJoinFormId equals _pjf.Id
                                                           select _vs;
        int _i = await q.CountAsync();
        if (_i == 0)
            return ResponseBaseModel.CreateSuccess("Значений нет (удалить нечего)");

        context_forms.RemoveRange(q);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Удалено значений: {_i}");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteSessionQuestionnaire(int session_id, CancellationToken cancellationToken = default)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ConstructorFormSessionModelDB? session_db = await context_forms
            .Sessions
            .Include(x => x.SessionValues)

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Pages!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.Fields) // поля

            .Include(s => s.Owner) // опрос/анкета
            .ThenInclude(x => x!.Pages!) // страницы опроса/анкеты
            .ThenInclude(x => x.JoinsForms!) // формы для страницы опроса/анкеты
            .ThenInclude(x => x.Form) // форма
            .ThenInclude(x => x!.FormsDirectoriesLinks) // поля
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
}