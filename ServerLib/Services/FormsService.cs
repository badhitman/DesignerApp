using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedLib;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ServerLib;

/// <summary>
/// Forms служба
/// </summary>
public class FormsService(MainDbAppContext context_forms, ILogger<FormsService> logger, IOptions<ServerConfigModel> _conf) : IFormsService
{
    static readonly Random r = new();

    #region public
    /// <inheritdoc/>
    public async Task<FormSessionQuestionnaireResponseModel> GetSessionQuestionnaire(string guid_session, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(guid_session, out Guid guid_parsed) || guid_parsed == Guid.Empty)
            return new() { Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Токен сессии имеет не корректный формат" }] };

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

        FormSessionQuestionnaireResponseModel res = new()
        {
            SessionQuestionnaire = await q
            .FirstOrDefaultAsync(x => x.SessionToken == guid_session, cancellationToken: cancellationToken)
        };
        string msg;
        if (res.SessionQuestionnaire is null)
        {
            msg = $"Токен '{guid_session}' не найден или просрочен.";
            res.AddError(msg);
            logger.LogError($"{msg} {nameof(res.SessionQuestionnaire)} is null. error {{5F3FF6DA-75AC-49B5-8608-8E909FF7C6C3}}");
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetDoneSessionQuestionnaire(string token_session, CancellationToken cancellationToken = default)
    {
        ConstructorFormSessionModelDB? sq = await context_forms.Sessions.FirstOrDefaultAsync(x => x.SessionToken == token_session, cancellationToken: cancellationToken);
        if (sq is null)
            return ResponseBaseModel.CreateError($"Сессия [{token_session}] не найдена в БД. ошибка {{B4DE6F2E-9F6F-4FEB-B2B9-426493C9A464}}");

        if (sq.SessionStatus >= SessionsStatusesEnum.Sended)
            return ResponseBaseModel.CreateSuccess("Сессия уже отмечена как выполненая и не требует вмешательства");

        //await context_forms.Sessions
        //        .Where(x => x.Id == sq.Id)
        //        .ExecuteUpdateAsync(s => s.SetProperty(b => b.SessionStatus, SessionsStatusesEnum.Sended).SetProperty(b => b.Editors, editors), cancellationToken: cancellationToken);

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
                logger.LogError($"error 6C359E53-9817-4CF4-AB51-79199FE3FBA0: {ex.Message}\n: {ex.StackTrace}");
                res.AddWarning($"Не удалось отправить уведомление наблюдателям [{sq.EmailsNotifications}]. Возникло исключение {{6C359E53-9817-4CF4-AB51-79199FE3FBA0}}: {ex.Message}");
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
    public async Task<FormSessionQuestionnaireResponseModel> SetValueFieldSessionQuestionnaire(SetValueFieldSessionQuestionnaireModel req, CancellationToken cancellationToken = default)
    {
        FormSessionQuestionnaireResponseModel session = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
        if (!session.Success())
            return session;

        ConstructorFormSessionModelDB? session_Questionnaire = session.SessionQuestionnaire;
        FormSessionQuestionnaireResponseModel res = new();

        if (session_Questionnaire?.Owner?.Pages is null || session_Questionnaire.SessionValues is null)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка {{CF37E7A0-6DFC-4007-B9C8-755783774E8E}}");
            return res;
        }
        //var v = IsReadonly(_httpContextAccessor.HttpContext.User, session_Questionnaire);

        if (session_Questionnaire.SessionStatus >= SessionsStatusesEnum.Sended)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (находиться в статусе {session_Questionnaire.SessionStatus}).");
            return res;
        }

        session_Questionnaire.LastQuestionnaireUpdateActivity = DateTime.Now;

        ConstructorFormQuestionnairePageJoinFormModelDB? form_join = session_Questionnaire.Owner.Pages.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FormsDirectoriesLinks is null)
        {
            res.AddError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена. ошибка {{64196795-F224-4C0F-9248-97FE46A8F07E}}");
            return res;
        }

        ConstructorFieldFormBaseLowModel? field_by_name = form_join.Form.AllFields.FirstOrDefault(x => x.Name.Equals(req.NameField, StringComparison.OrdinalIgnoreCase));
        if (field_by_name is null)
        {
            res.AddError($"Поле '{req.NameField}' не найдено в форме #{form_join.Form.Id} '{form_join.Form.Name}'. ошибка {{BF95A8F0-9B7B-4BB1-843E-57DC2A753FDD}}");
            return res;
        }

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
    public async Task<CreateObjectOfIntKeyResponseModel> AddRowToTable(FieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default)
    {
        FormSessionQuestionnaireResponseModel get_s = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
        if (!get_s.Success())
            return new CreateObjectOfIntKeyResponseModel() { Messages = get_s.Messages };
        CreateObjectOfIntKeyResponseModel res = new();
        ConstructorFormSessionModelDB? session = get_s.SessionQuestionnaire;

        if (session?.Owner?.Pages is null || session.SessionValues is null)
        {
            res.AddError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка {{DB52DC58-6B50-495D-9DE8-201E93A3FB3E}}");
            return res;
        }

        if (session.SessionStatus >= SessionsStatusesEnum.Sended)
            return (CreateObjectOfIntKeyResponseModel)ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (в статусе {session.SessionStatus}).");

        ConstructorFormQuestionnairePageJoinFormModelDB? form_join = session.Owner.Pages.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FormsDirectoriesLinks is null || !form_join.IsTable)
        {
            res.AddError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена или повреждена. ошибка {{8596931B-110E-4D33-9F98-AC5AFC96284B}}");
            return res;
        }
        IQueryable<ConstructorFormSessionValueModelDB> q = session.SessionValues.Where(x => x.QuestionnairePageJoinFormId == form_join.Id && x.GroupByRowNum > 0).AsQueryable();
        res.Id = (int)(q.Any() ? (q.Max(x => x.GroupByRowNum) + 1) : 1);

        await context_forms.AddRangeAsync(form_join.Form.AllFields.Where(ScolarOnly).Select(x => new ConstructorFormSessionValueModelDB()
        {
            GroupByRowNum = (uint)res.Id,
            Name = x.Name,
            Owner = session,
            OwnerId = session.Id,
            QuestionnairePageJoinForm = form_join,
            QuestionnairePageJoinFormId = form_join.Id
        }), cancellationToken);
        session.LastQuestionnaireUpdateActivity = DateTime.Now;

        await context_forms.SaveChangesAsync(cancellationToken);
        res.AddSuccess($"Добавлена строка в таблицу: №п/п {res.Id}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteValuesFieldsByGroupSessionQuestionnaireByRowNum(ValueFieldSessionQuestionnaireBaseModel req, CancellationToken cancellationToken = default)
    {
        FormSessionQuestionnaireResponseModel get_s = await GetSessionQuestionnaire(req.SessionId, cancellationToken);
        if (!get_s.Success())
            return get_s;

        ConstructorFormSessionModelDB? session = get_s.SessionQuestionnaire;
        if (session?.Owner?.Pages is null || session.SessionValues is null)
            return ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} не найдена в БД. ошибка {{73DFEAA2-3020-4CB2-BED4-4523E5003BE5}}");

        if (session.SessionStatus >= SessionsStatusesEnum.Sended)
            return ResponseBaseModel.CreateError($"Сессия опроса/анкеты {nameof(req.SessionId)}#{req.SessionId} заблокирована (статус {session.SessionStatus}).");

        ConstructorFormQuestionnairePageJoinFormModelDB? form_join = session.Owner.Pages.SelectMany(x => x.JoinsForms!).FirstOrDefault(x => x.Id == req.JoinFormId);
        if (form_join?.Form?.Fields is null || form_join.Form.FormsDirectoriesLinks is null || !form_join.IsTable)
            return ResponseBaseModel.CreateError($"Связь формы со страницей опроса/анкеты #{req.JoinFormId} не найдена или повреждена. ошибка {{C829EC71-39DB-4AAD-8CD9-FD8B025D61F5}}");

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

    static bool ScolarOnly(ConstructorFieldFormBaseLowModel x) => !(x is ConstructorFieldFormModelDB _f && _f.TypeField == TypesFieldsFormsEnum.ProgrammCalcDouble);

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

    static bool IsForce(ClaimsPrincipal clp, ConstructorFormSessionModelDB sq)
    {
        string? email = clp.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value;
        return clp.Claims.Any(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && (x.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))) || sq.CreatorEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
    }

    #region сессии опросов/анкет
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetStatusSessionQuestionnaire(int id_session, SessionsStatusesEnum status, CancellationToken cancellationToken = default)
    {
        ConstructorFormSessionModelDB? sq = await context_forms.Sessions.FirstOrDefaultAsync(x => x.Id == id_session, cancellationToken: cancellationToken);
        if (sq is null)
            return ResponseBaseModel.CreateError($"Сессия [{id_session}] не найдена в БД. ошибка {{8EE510F9-57BD-4764-AB38-040BBE2B0AD6}}");

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
                logger.LogError($"error 2366356C-7416-47F8-A365-DEAEB6F71F41: {ex.Message}\n: {ex.StackTrace}");
                res.AddWarning($"Не удалось отправить уведомление наблюдателям [{sq.EmailsNotifications}]. Возникло исключение {{DF39BB22-E1E1-45FC-925F-D7664DE842BE}}: {ex.Message}");
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormSessionQuestionnaireResponseModel> GetSessionQuestionnaire(int id_session, CancellationToken cancellationToken = default)
    {
        FormSessionQuestionnaireResponseModel res = new()
        {
            SessionQuestionnaire = await context_forms
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
        if (res.SessionQuestionnaire is null)
        {
            msg = $"for {nameof(id_session)} = [{id_session}]. {nameof(res.SessionQuestionnaire)} is null. error {{6DF75C8A-C0FD-4165-88D8-15AEEA8606ED}}";
            logger.LogError(msg);
            res.AddError(msg);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormSessionQuestionnaireResponseModel> UpdateOrCreateSessionQuestionnaire(ConstructorFormSessionModelDB session_json, CancellationToken cancellationToken = default)
    {
        FormSessionQuestionnaireResponseModel res = new();
        if (!string.IsNullOrWhiteSpace(session_json.EmailsNotifications))
        {
            string[] een = session_json.EmailsNotifications.SplitToList().Where(x => !MailAddress.TryCreate(x, out _)).ToArray();
            if (een.Length != 0)
            {
                res.AddError($"Не корректные адреса получателей. {JsonConvert.SerializeObject(een)}. error {{BC1B3DA4-0952-4937-8FB8-A99BD22019C3}}");
                return res;
            }
        }
        if (session_json.Id < 1)
        {
            if (!MailAddress.TryCreate(session_json.CreatorEmail, out _))
            {
                res.AddError("Ошибка! Ваш email не корректен. {FF1C3CB2-697F-4F77-A42A-5D827D12DA9F}");
                return res;
            }

            session_json.CreatedAt = DateTime.Now;
            session_json.DeadlineDate = DateTime.Now.AddMinutes(_conf.Value.TimeActualityQuestionnaireSessionMinutes);
            session_json.SessionToken = Guid.NewGuid().ToString();

            await context_forms.AddAsync(session_json, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            return new() { SessionQuestionnaire = session_json, Messages = new() { new() { TypeMessage = ResultTypesEnum.Success, Text = $"Создана сессия #{session_json.Id}" } } };
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
        FormSessionQuestionnaireResponseModel sr = await GetSessionQuestionnaire(session_json.Id, cancellationToken);
        if (res.Messages.Count != 0)
            sr.AddRangeMessages(res.Messages);
        return sr;
    }

    /// <inheritdoc/>
    public async Task<ConstructorFormsSessionsPaginationResponseModel> RequestSessionsQuestionnaires(RequestSessionsQuestionnairesRequestPaginationModel req, CancellationToken cancellationToken = default)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        ConstructorFormsSessionsPaginationResponseModel res = new(req);
        IQueryable<ConstructorFormSessionModelDB> q = context_forms
            .Sessions
            .Include(x => x.Owner)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (req.QuestionnaireId > 0)
            q = q.Where(x => x.OwnerId == req.QuestionnaireId);

        if (!string.IsNullOrWhiteSpace(req.SimpleRequest))
            q = q.Where(x => (x.Name != null && x.Name.Contains(req.SimpleRequest, StringComparison.CurrentCultureIgnoreCase)) || x.SessionToken == req.SimpleRequest || (!string.IsNullOrWhiteSpace(x.EmailsNotifications) && EF.Functions.Like(x.EmailsNotifications, $"%{req.SimpleRequest}%")));

        res.TotalRowsCount = await q.CountAsync(cancellationToken: cancellationToken);
        q = q.Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        res.Sessions = await q.ToArrayAsync(cancellationToken: cancellationToken);
        return res;
    }

    /// <inheritdoc/>
    public async Task<EntriesDictResponseModel> FindSessionsQuestionnairesByFormFieldName(FormFieldModel req, CancellationToken cancellationToken = default)
    {
        EntriesDictResponseModel res = new();
        if (string.IsNullOrWhiteSpace(req.FieldName))
        {
            res.AddError("Не указано имя поля/колонки");
            return res;
        }

        var q = from _vs in context_forms.ValuesSessions.Where(_vs => _vs.Name == req.FieldName)
                join _s in context_forms.Sessions on _vs.OwnerId equals _s.Id
                join _pjf in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == req.FormId) on _vs.QuestionnairePageJoinFormId equals _pjf.Id
                join _qp in context_forms.QuestionnairesPages on _pjf.OwnerId equals _qp.Id
                select new { Value = _vs, Session = _s, QuestionnairePageJoinForm = _pjf, QuestionnairePage = _qp };

        var data_rows = await q.ToArrayAsync(cancellationToken: cancellationToken);

        res.Elements = data_rows
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
        res.AddInfo($"Получено ссылок {res.Elements.Count()}");
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ClearValuesForFieldName(FormFieldOfSessionModel req, CancellationToken cancellationToken = default)
    {
        IQueryable<ConstructorFormSessionValueModelDB> q = from _vs in context_forms.ValuesSessions.Where(_vs => _vs.Name == req.FieldName)
                                                           join _s in context_forms.Sessions.Where(x => !req.SessionId.HasValue || x.Id == req.SessionId.Value) on _vs.OwnerId equals _s.Id
                                                           join _pjf in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == req.FormId) on _vs.QuestionnairePageJoinFormId equals _pjf.Id
                                                           select _vs;
        int _i = await q.CountAsync();
        if (_i == 0)
            return ResponseBaseModel.CreateSuccess("Значений нет (удалиять нечего)");

        context_forms.RemoveRange(q);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Удалено значений: {_i}");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteSessionQuestionnaire(int session_id, CancellationToken cancellationToken = default)
    {
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

    #region опросы/анкеты
    /// <inheritdoc/>
    public async Task<ConstructorFormsQuestionnairesPaginationResponseModel> RequestQuestionnaires(SimplePaginationRequestModel req, CancellationToken cancellationToken)
    {
        if (req.PageSize < 1)
            req.PageSize = 10;

        ConstructorFormsQuestionnairesPaginationResponseModel res = new(req);
        IQueryable<ConstructorFormQuestionnaireModelDB> q =
            (from _quest in context_forms.Questionnaires
             join _page in context_forms.QuestionnairesPages on _quest.Id equals _page.OwnerId
             join _join_form in context_forms.QuestionnairesPagesJoinForms on _page.Id equals _join_form.OwnerId
             join _form in context_forms.Forms on _join_form.FormId equals _form.Id
             where string.IsNullOrWhiteSpace(req.SimpleRequest) || EF.Functions.Like(_form.Name, req.SimpleRequest) || EF.Functions.Like(_quest.Name, req.SimpleRequest) || EF.Functions.Like(_page.Name, req.SimpleRequest)
             group _quest by _quest into g
             select g.Key)
            .AsQueryable();

        res.TotalRowsCount = await q.CountAsync(cancellationToken: cancellationToken);
        q = q.OrderBy(x => x.Id).Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        int[] ids = await q.Select(x => x.Id).ToArrayAsync(cancellationToken: cancellationToken);
        q = context_forms.Questionnaires.Include(x => x.Pages).Where(x => ids.Contains(x.Id)).Include(x => x.Pages).OrderBy(x => x.Name);
        res.Questionnaires = ids.Any()
            ? await q.ToArrayAsync(cancellationToken: cancellationToken)
            : Enumerable.Empty<ConstructorFormQuestionnaireModelDB>();

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormQuestionnaireResponseModel> GetQuestionnaire(int questionnaire_id, CancellationToken cancellationToken = default)
    {
        FormQuestionnaireResponseModel res = new()
        {
            Questionnaire = await context_forms
           .Questionnaires
           .Include(x => x.Pages!)
           .ThenInclude(x => x.JoinsForms)
           .FirstOrDefaultAsync(x => x.Id == questionnaire_id, cancellationToken: cancellationToken)
        };

        if (res.Questionnaire is null)
            res.AddError($"Опрос/анкета #{questionnaire_id} отсутствует в БД");

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormQuestionnaireResponseModel> UpdateOrCreateQuestionnaire(EntryDescriptionModel questionnaire, CancellationToken cancellationToken = default)
    {
        questionnaire.Name = Regex.Replace(questionnaire.Name, @"\s+", " ").Trim();

        FormQuestionnaireResponseModel res = new();
        ConstructorFormQuestionnaireModelDB? questionnaire_db = await context_forms.Questionnaires.FirstOrDefaultAsync(x => x.Id != questionnaire.Id && EF.Functions.Like(x.Name, questionnaire.Name), cancellationToken: cancellationToken);
        string msg;
        if (questionnaire_db is not null)
        {
            msg = $"Опрос/анкета с таким именем уже существует: #{questionnaire_db.Id} '{questionnaire_db.Name}'";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        if (questionnaire.Id < 1)
        {
            questionnaire_db = ConstructorFormQuestionnaireModelDB.Build(questionnaire);
            await context_forms.AddAsync(questionnaire_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Опрос/анкета создана #{questionnaire_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            res.Questionnaire = questionnaire_db;
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

        if (questionnaire_db.Name != questionnaire.Name && questionnaire_db.Description != questionnaire.Description)
        {
            msg = $"Опрос/анкета #{questionnaire.Id} не требует изменений в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
        }
        else
        {
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
        ConstructorFormQuestionnaireModelDB? Questionnaire_db = await context_forms.Questionnaires.Include(x => x.Pages!).ThenInclude(x => x.JoinsForms).FirstOrDefaultAsync(x => x.Id == questionnaire_id, cancellationToken: cancellationToken);
        if (Questionnaire_db is null)
            return ResponseBaseModel.CreateError($"Опрос/анкета #{questionnaire_id} не найдена в БД");

        context_forms.Remove(Questionnaire_db);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Опрос/анкета #{questionnaire_id} удалена");
    }
    #endregion

    #region страницы опросов/анкет
    /// <inheritdoc/>
    public async Task<FormQuestionnaireResponseModel> QuestionnairePageMove(int questionnaire_page_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        FormQuestionnaireResponseModel res = new();
        ConstructorFormQuestionnairePageModelDB? questionnaire_db = await context_forms
            .QuestionnairesPages
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Pages)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_id, cancellationToken: cancellationToken);

        if (questionnaire_db is null)
        {
            res.AddError($"Страница опроса/анкеты #{questionnaire_page_id} отсутствует в БД. ошибка {{14C5D6E8-2DCB-4B72-8691-BEFB773C7A73}}");
            return res;
        }
        questionnaire_db.Owner!.Pages = questionnaire_db.Owner.Pages!.OrderBy(x => x.SortIndex).ToList();

        ConstructorFormQuestionnairePageModelDB? _fns = questionnaire_db.Owner.GetOutermostPage(direct, questionnaire_db.SortIndex);

        if (_fns is null)
            res.AddError("Не удалось выполнить перемещение. ошибка {50CCAF13-65B2-4702-B0F0-88A0C39CF62A}");
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

        res.Questionnaire = await context_forms
            .Questionnaires
            .Include(x => x.Pages)
            .FirstAsync(x => x.Id == questionnaire_db.OwnerId, cancellationToken: cancellationToken);
        questionnaire_db.Owner!.Pages = questionnaire_db.Owner.Pages!.OrderBy(x => x.SortIndex).ToList();

        int i = 0;
        bool is_upd = false;
        foreach (ConstructorFormQuestionnairePageModelDB p in res.Questionnaire.Pages!)
        {
            i++;
            is_upd = is_upd || p.SortIndex != i;
            p.SortIndex = i;
        }

        if (is_upd)
        {
            res.AddWarning("Исправлена пересортица");
            context_forms.UpdateRange(res.Questionnaire.Pages!);
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

    #region структура страниц опросов/анкет: формы и настройки
    /// <inheritdoc/>
    public async Task<FormQuestionnairePageResponseModel> QuestionnairePageJoinFormMove(int questionnaire_page_join_form_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        FormQuestionnairePageResponseModel res = new();
        ConstructorFormQuestionnairePageJoinFormModelDB? questionnaire_page_join_db = await context_forms
            .QuestionnairesPagesJoinForms
            .Include(x => x.Owner)
            .ThenInclude(x => x!.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_join_form_id, cancellationToken: cancellationToken);

        if (questionnaire_page_join_db?.Owner?.JoinsForms is null)
        {
            res.AddError($"Форма для страницы опроса/анкеты #{questionnaire_page_join_form_id} отсутствует в БД. ошибка {{9CF1B33A-B26C-4A60-B0CD-35EECB756431}}");
            return res;
        }

        ConstructorFormQuestionnairePageJoinFormModelDB? _fns = questionnaire_page_join_db.Owner.GetOutermostJoinForm(direct, questionnaire_page_join_db.SortIndex);

        if (_fns is null)
            res.AddError("Не удалось выполнить перемещение. ошибка {F68A183D-FE97-479D-8C86-AFACE8329C3D}");
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

        questionnaire_page_join_db.Owner.JoinsForms = questionnaire_page_join_db.Owner.JoinsForms!.OrderBy(x => x.SortIndex).ToList();

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
        ResponseBaseModel res = new();

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
    public async Task<FormQuestionnairePageJoinFormResponseModel> GetQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default)
    {
        FormQuestionnairePageJoinFormResponseModel res = new()
        {
            QuestionnairePageJoinForm = await context_forms
            .QuestionnairesPagesJoinForms
            .Include(x => x.Form)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.JoinsForms)
            .FirstOrDefaultAsync(x => x.Id == questionnaire_page_join_form_id, cancellationToken: cancellationToken)
        };

        if (res.QuestionnairePageJoinForm is null)
            res.AddError($"Связь #{questionnaire_page_join_form_id} (форма<->опрос/анкета) не найдена в БД");
        else if (res.QuestionnairePageJoinForm.Owner?.JoinsForms is not null)
            res.QuestionnairePageJoinForm.Owner.JoinsForms = res.QuestionnairePageJoinForm.Owner.JoinsForms.OrderBy(x => x.SortIndex).ToList();

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteQuestionnairePageJoinForm(int questionnaire_page_join_form_id, CancellationToken cancellationToken = default)
    {
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

    #region формы
    /// <inheritdoc/>
    public async Task<ConstructorFormsPaginationResponseModel> SelectForms(AltSimplePaginationRequestModel req, CancellationToken cancellationToken = default)
    {
        ConstructorFormsPaginationResponseModel res = new(req);
        IQueryable<ConstructorFormModelDB> q =
            (from _form in context_forms.Forms
             join _field in context_forms.Fields on _form.Id equals _field.OwnerId
             where string.IsNullOrWhiteSpace(req.SimpleRequest) || EF.Functions.Like(_form.Name, $"%{req.SimpleRequest}%") || EF.Functions.Like(_field.Name, $"%{req.SimpleRequest}%")
             group _form by _form into g
             select g.Key)
             .OrderBy(x => x.Name)
            .AsQueryable();

        if (req.StrongMode)
            q = from ss in q
                where context_forms.Fields.Any(x => x.OwnerId == ss.Id)
                select ss;

        res.TotalRowsCount = await q.CountAsync(cancellationToken: cancellationToken);
        q = q.OrderBy(x => x.Id).Skip(req.PageSize * req.PageNum).Take(req.PageSize);

        int[] ids = await q.Select(x => x.Id).ToArrayAsync(cancellationToken: cancellationToken);

        res.Elements = ids.Length != 0
            ? await context_forms.Forms.Where(x => ids.Contains(x.Id)).Include(x => x.Fields).Include(x => x.FormsDirectoriesLinks).ToArrayAsync(cancellationToken: cancellationToken)
            : Enumerable.Empty<ConstructorFormModelDB>();
        return res;
    }

    /// <inheritdoc/>
    public async Task<FormResponseModel> GetForm(int form_id, CancellationToken cancellationToken = default)
    {
        FormResponseModel res = new()
        {
            Form = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks!)
            .ThenInclude(x => x.Directory)
            .FirstOrDefaultAsync(x => x.Id == form_id, cancellationToken: cancellationToken)
        };

        if (res.Form is null)
            res.AddError($"Форма #{form_id} не найдена в БД");
        else
        {
            if (res.Form.Fields is not null)
            {
                string msg;
                List<ConstructorFieldFormModelDB> _upd = new();
                Dictionary<MetadataExtensionsFormFieldsEnum, object?> mdvs;
                foreach (ConstructorFieldFormModelDB f in res.Form.Fields)
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
                if (_upd.Any())
                {
                    context_forms.UpdateRange(_upd);
                    await context_forms.SaveChangesAsync(cancellationToken);
                }
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormResponseModel> FormUpdateOrCreate(ConstructorFormBaseModel form, CancellationToken cancellationToken = default)
    {
        form.Name = Regex.Replace(form.Name, @"\s+", " ").Trim();
        FormResponseModel res = new();
        ConstructorFormModelDB? form_db = await context_forms.Forms.FirstOrDefaultAsync(x => x.Id != form.Id && EF.Functions.Like(x.Name, form.Name), cancellationToken: cancellationToken);
        string msg;
        if (form_db is not null)
        {
            msg = $"Форма с таким именем уже существует: #{form_db.Id} '{form_db.Name}'";
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
            res.Form = form_db;
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
        res.Form = await context_forms
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
    public async Task<FormResponseModel> FieldFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        FormResponseModel res = new();
        ConstructorFieldFormModelDB? field_db = await context_forms
            .Fields
            .Include(x => x.Owner)
            .ThenInclude(x => x!.Fields)
            .Include(x => x.Owner)
            .ThenInclude(x => x!.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_id, cancellationToken: cancellationToken);

        if (field_db is null)
        {
            res.AddError($"Поле формы (простого типа) #{field_id} не найден в БД. ошибка {{22BE3288-025D-49D3-BCBF-745ECB75DAC3}}");
            return res;
        }

        if ((field_db.SortIndex <= 1 && direct == VerticalDirectionsEnum.Up) || (field_db.SortIndex >= field_db.Owner!.AllFields.Count() && direct == VerticalDirectionsEnum.Down))
        {
            res.AddWarning($"Поле формы (простого типа) #{field_id} не может быть перемещено: оно уже в крайнем положении. ошибка {{F0344B5E-4C57-445E-A434-86C15DB36FEE}}");
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
            res.AddError("Не удалось выполнить перемещение. ошибка {233A4FF6-8E60-4689-985E-C11C54CF13EE}");

        if (res.Success())
        {
            await context_forms.SaveChangesAsync(cancellationToken);
            res.Form = await context_forms
                .Forms
                .Include(x => x.Fields)
                .Include(x => x.FormsDirectoriesLinks)
                .FirstAsync(x => x.Id == field_db.OwnerId, cancellationToken: cancellationToken);
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<FormResponseModel> FieldDirectoryFormMove(int field_id, VerticalDirectionsEnum direct, CancellationToken cancellationToken = default)
    {
        FormResponseModel res = new();
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
            res.AddError($"Поле формы (тип: справочник) #{field_id} не найден в БД. ошибка {{0A4F1C04-A1EB-4915-9610-54AECD441C10}}");
            return res;
        }

        if ((field_db.SortIndex <= 1 && direct == VerticalDirectionsEnum.Up) || (field_db.SortIndex >= field_db.Owner!.AllFields.Count() && direct == VerticalDirectionsEnum.Down))
        {
            res.AddWarning($"Поле формы (тип: справочник) #{field_id} не может быть перемещено: оно уже в крайнем положении. ошибка {{353527CD-793C-45A9-B616-7FC8036E27F1}}");
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
            res.AddError("Не удалось выполнить перемещение. ошибка {1A869633-2DC7-49E5-A604-257CE5EE6357}");

        if (res.Success())
        {
            await context_forms.SaveChangesAsync(cancellationToken);
            res.Form = await context_forms
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
        ResponseBaseModel res = new();

        ConstructorFormModelDB? form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == form_field.OwnerId, cancellationToken: cancellationToken);

        string msg;
        if (form_db is null)
        {
            msg = $"Форма #{form_field.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        ConstructorFieldFormModelDB? form_field_db;
        if (form_field.Id < 1)
        {
            if (form_db.AllFields.Any(x => x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase)))
                return ResponseBaseModel.CreateError("Поле с таким именем уже существует. ошибка {3EE86A19-152B-4581-941B-834D81AA1F5F}");

            int _sort_index = form_db.FormsDirectoriesLinks!.Any() ? form_db.FormsDirectoriesLinks!.Max(x => x.SortIndex) : 0;
            _sort_index = Math.Max(_sort_index, form_db.Fields!.Any() ? form_db.Fields!.Max(x => x.SortIndex) : 0);

            form_field_db = ConstructorFieldFormModelDB.Build(form_field, form_db, _sort_index + 1);
            
            await context_forms.AddAsync(form_field_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Поле (простого типа) создано #{form_field_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        if (form_db.AllFields.Any(x => (x.GetType() == typeof(ConstructorFormDirectoryLinkModelDB) && x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase)) || (x.GetType() == typeof(ConstructorFieldFormModelDB)) && x.Id != form_field.Id && x.Name.Equals(form_field.Name, StringComparison.OrdinalIgnoreCase)))
            return ResponseBaseModel.CreateError("Поле с таким именем уже существует. ошибка {D923339F-E340-48FB-9136-15A65B2A2E5C}");

        form_field_db = form_db.Fields!.FirstOrDefault(x => x.Id == form_field.Id);

        if (form_field_db is null)
        {
            msg = $"Поле (простого типа) формы #{form_field.Id} не найдено в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context_forms.Database.BeginTransaction();

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
                msg = $"Тип поля (простого типа) формы #{form_field.Id} не может быть изменён ([{form_field_db.TypeField}] -> [{form_field.TypeField}]): найдены ссылки введёных значений ({values_updates.Count} штук) для этого поля";
                res.AddError(msg);
                logger.LogError(msg);
                return res;
            }

            if (form_field.TypeField == TypesFieldsFormsEnum.ProgrammCalcDouble && form_field.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, null) is null)
            {
                msg = $"";
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
            msg = $"Имя поля (простого типа) формы #{form_field.Id} изменилось: [{form_field_db.Name}] -> [{form_field.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_field_db.Name = form_field.Name;

            if (values_updates.Any())
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
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
            transaction.Commit();
        }
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDirectoryUpdateOrCreate(ConstructorFormDirectoryLinkModelDB field_directory, CancellationToken cancellationToken = default)
    {
        field_directory.Name = Regex.Replace(field_directory.Name, @"\s+", " ").Trim();
        if (string.IsNullOrEmpty(field_directory.Name))
            return ResponseBaseModel.CreateError("Имя поля не может быть пустым");

        ResponseBaseModel res = new();

        ConstructorFormDirectoryModelDB? dir_db = await context_forms.Directories.FirstOrDefaultAsync(x => x.Id == field_directory.DirectoryId, cancellationToken: cancellationToken);

        if (dir_db is null)
            return ResponseBaseModel.CreateError($"Не найден справочник #{field_directory.DirectoryId}");

        ConstructorFormModelDB? form_db = await context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks)
            .FirstOrDefaultAsync(x => x.Id == field_directory.OwnerId, cancellationToken: cancellationToken);
        string msg;
        if (form_db is null)
        {
            msg = $"Форма #{field_directory.OwnerId} не найдена в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        ConstructorFormDirectoryLinkModelDB? form_fieldd_db;
        if (field_directory.Id < 1)
        {
            if (form_db.AllFields.Any(x => x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase)))
                return ResponseBaseModel.CreateError("Поле с таким именем уже существует. ошибка {97767B59-6CBA-49EC-BB09-DD673AD07C84}");

            int _sort_index = form_db.FormsDirectoriesLinks!.Any() ? form_db.FormsDirectoriesLinks!.Max(x => x.SortIndex) : 0;
            _sort_index = Math.Max(_sort_index, form_db.Fields!.Any() ? form_db.Fields!.Max(x => x.SortIndex) : 0);

            form_fieldd_db = new()
            {
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
            await context_forms.AddAsync(form_fieldd_db, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken);
            msg = $"Поле (списочного типа) создано #{form_fieldd_db.Id}";
            res.AddSuccess(msg);
            logger.LogInformation(msg);
            return res;
        }

        if (form_db.AllFields.Any(x => (x.GetType() == typeof(ConstructorFieldFormModelDB) && x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase)) || (x.GetType() == typeof(ConstructorFormDirectoryLinkModelDB) && x.Id != field_directory.Id && x.Name.Equals(field_directory.Name, StringComparison.OrdinalIgnoreCase))))
            return ResponseBaseModel.CreateError("Поле с таким именем уже существует. ошибка {B5062132-5EDD-434F-9FB7-C34F9EF92DCA}");

        form_fieldd_db = form_db.FormsDirectoriesLinks!.FirstOrDefault(x => x.Id == field_directory.Id);
        if (form_fieldd_db is null)
        {
            msg = $"Поле (списочного типа) формы #{field_directory.Id} не найдено в БД";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }

        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context_forms.Database.BeginTransaction();

        List<ConstructorFormSessionValueModelDB> values_updates = await (from val_s in context_forms.ValuesSessions.Where(x => x.Name == field_directory.Name)
                                                                         join session in context_forms.Sessions on val_s.OwnerId equals session.Id
                                                                         join Questionnaire in context_forms.Questionnaires on session.OwnerId equals Questionnaire.Id
                                                                         join page in context_forms.QuestionnairesPages on Questionnaire.Id equals page.OwnerId
                                                                         join form_join in context_forms.QuestionnairesPagesJoinForms.Where(x => x.FormId == form_db.Id) on page.Id equals form_join.OwnerId
                                                                         select val_s)
                                                                     .ToListAsync(cancellationToken: cancellationToken);

        if (form_fieldd_db.DirectoryId != field_directory.DirectoryId)
        {
            if (values_updates.Any())
            {
                msg = $"Тип поля (списочного типа) формы #{field_directory.Id} не может быть изменён ([{form_fieldd_db.DirectoryId}] -> [{field_directory.DirectoryId}]): найдены ссылки введёных значений ({values_updates.Count} штук) для этого поля";
                res.AddError(msg);
                logger.LogError(msg);
                return res;
            }

            msg = $"Подтип поля (списочного типа) формы #{field_directory.Id} изменился: [{form_fieldd_db.DirectoryId}] -> [{field_directory.DirectoryId}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_fieldd_db.DirectoryId = field_directory.DirectoryId;
        }

        if (form_fieldd_db.Name != field_directory.Name)
        {
            if (values_updates.Any())
            {
                values_updates.ForEach(x => { x.Name = field_directory.Name; });
                context_forms.UpdateRange(values_updates);
            }

            msg = $"Имя поля (списочного типа) формы #{field_directory.Id} изменилось: [{form_fieldd_db.Name}] -> [{field_directory.Name}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_fieldd_db.Name = field_directory.Name;
        }
        if (form_fieldd_db.Css != field_directory.Css)
        {
            msg = $"CSS поля (списочного типа) формы #{field_directory.Id} изменилось: [{form_fieldd_db.Css}] -> [{field_directory.Css}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_fieldd_db.Css = field_directory.Css;
        }
        if (form_fieldd_db.Description != field_directory.Description)
        {
            msg = $"Описание поля (списочного типа) формы #{field_directory.Id} изменилось";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_fieldd_db.Description = field_directory.Description;
        }
        if (form_fieldd_db.Hint != field_directory.Hint)
        {
            msg = $"Подсказка поля (списочного типа) формы #{field_directory.Id} изменилась: [{form_fieldd_db.Hint}] -> [{field_directory.Hint}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_fieldd_db.Hint = field_directory.Hint;
        }
        if (form_fieldd_db.Required != field_directory.Required)
        {
            msg = $"Признак [{nameof(form_fieldd_db.Required)}] поля (списочного типа) формы #{field_directory.Id} изменился: [{form_fieldd_db.Required}] -> [{field_directory.Required}]";
            res.AddWarning(msg);
            logger.LogInformation(msg);
            form_fieldd_db.Required = field_directory.Required;
        }

        if (res.Messages.Any(x => x.TypeMessage == ResultTypesEnum.Warning))
        {
            context_forms.Update(form_fieldd_db);
            msg = $"Поле (списочного типа) формы #{field_directory.Id} обновлено в БД";
            res.AddInfo(msg);
            logger.LogInformation(msg);
            await context_forms.SaveChangesAsync(cancellationToken);
        }
        transaction.Commit();
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> FormFieldDelete(int form_field_id, CancellationToken cancellationToken = default)
    {
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
    public async Task<FormResponseModel> CheckAndNormalizeSortIndex(ConstructorFormModelDB form, CancellationToken cancellationToken = default)
    {
        FormResponseModel res = new();
        int i = 0;
        List<ConstructorFormDirectoryLinkModelDB> fields_dir = new();
        List<ConstructorFieldFormModelDB> fields_st = new();

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
        if (fields_st.Any())
            context_forms.UpdateRange(fields_st);
        if (fields_dir.Any())
            context_forms.UpdateRange(fields_dir);

        if (fields_st.Any() || fields_dir.Any())
            await context_forms.SaveChangesAsync(cancellationToken);
        else
            res.AddInfo("Пересортица отсутствует");

        res.Form = context_forms
            .Forms
            .Include(x => x.Fields)
            .Include(x => x.FormsDirectoriesLinks!)
            .ThenInclude(x => x.Directory)
            .FirstOrDefault(x => x.Id == form.Id);

        return res;
    }
    #endregion

    #region справочники/списки
    /// <inheritdoc/>
    public async Task<IEnumerable<EntryNestedModel>> ReadDirectories(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        if (!ids.Any())
            return Enumerable.Empty<EntryNestedModel>();

        ConstructorFormDirectoryModelDB[] res = await context_forms.Directories.Include(x => x.Elements).Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken: cancellationToken);

        return res.Select(x => new EntryNestedModel() { Id = x.Id, Name = x.Name, Childs = x.Elements!.Select(y => new EntryModel() { Id = y.Id, Name = y.Name }) });
    }

    /// <inheritdoc/>
    public async Task<EntriesResponseModel> GetDirectories(string? name_filter = null, CancellationToken cancellationToken = default)
    {
        return new EntriesResponseModel() { Entries = await context_forms.Directories.Select(x => new EntryModel() { Id = x.Id, Name = x.Name }).ToArrayAsync(cancellationToken: cancellationToken) };
    }

    /// <inheritdoc/>
    public async Task<CreateObjectOfIntKeyResponseModel> UpdateOrCreateDirectory(EntryModel _dir, CancellationToken cancellationToken = default)
    {
        CreateObjectOfIntKeyResponseModel res = new();
        string msg;
        if (string.IsNullOrWhiteSpace(_dir.Name))
        {
            msg = $"Имя справочника #{_dir.Id} не может быть пустым";
            res.AddError(msg);
            logger.LogError(msg);
            return res;
        }
        _dir.Name = Regex.Replace(_dir.Name, @"\s+", " ").Trim();
        ConstructorFormDirectoryModelDB? ne = await context_forms.Directories.FirstOrDefaultAsync(x => x.Id != _dir.Id && EF.Functions.Like(x.Name, _dir.Name), cancellationToken: cancellationToken);
        if (ne is not null)
        {
            msg = $"Справочник с именем '{ne.Name}' уже существует под идентификатором `#{ne.Id}`";
            logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        if (_dir.Id < 1)
        {
            ne = new() { Name = _dir.Name };
            try
            {
                await context_forms.AddAsync(ne, cancellationToken);
                await context_forms.SaveChangesAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                res.AddError($"exception {{63552C0A-CC7B-4F9C-A26A-23B22952DA2D}}: {ex.Message}");
                return res;
            }

            res.AddSuccess($"Справочник успешно создан #{res.Id}");
        }
        else
        {
            ne = await context_forms.Directories.FirstOrDefaultAsync(x => x.Id == _dir.Id, cancellationToken: cancellationToken);
            if (ne is null)
            {
                msg = $"Справочник #{_dir.Id} не найден в БД";
                logger.LogError(msg);
                res.AddError(msg);
                return res;
            }
            if (ne.Name.Equals(_dir.Name))
            {
                res.AddInfo("Имя не требует изменения");
            }
            else
            {
                msg = $"Справочник #{_dir.Id} переименован: `{ne.Name}` -> `{_dir.Name}`";
                ne.Name = _dir.Name;
                context_forms.Update(ne);
                await context_forms.SaveChangesAsync(cancellationToken);
                logger.LogInformation(msg);
                res.AddSuccess(msg);
            }
        }
        res.Id = ne.Id;
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
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
    public async Task<EntriesResponseModel> GetElementsOfDirectory(int directory_id, CancellationToken cancellationToken = default)
    {
        EntriesResponseModel res = new();
        ConstructorFormDirectoryModelDB? dir = await context_forms.Directories
            .Include(x => x.Elements)
            .FirstOrDefaultAsync(x => x.Id == directory_id, cancellationToken: cancellationToken);

        if (dir is null)
        {
            res.AddError($"Справочник #{directory_id} не найден в БД");
            return res;
        }

        res.Entries = dir.Elements?.ToArray();
        return res;
    }

    /// <inheritdoc/>
    public async Task<CreateObjectOfIntKeyResponseModel> CreateElementForDirectory(string name_element_of_dir, int directory_id, CancellationToken cancellationToken = default)
    {
        name_element_of_dir = Regex.Replace(name_element_of_dir, @"\s+", " ").Trim();
        CreateObjectOfIntKeyResponseModel res = new();
        ConstructorFormDirectoryModelDB? dir = await context_forms.Directories
            .Include(x => x.Elements)
            .FirstOrDefaultAsync(x => x.Id == directory_id, cancellationToken: cancellationToken);

        if (dir is null)
        {
            res.AddError($"Справочник #{directory_id} не найден в БД");
            return res;
        }

        ConstructorFormDirectoryElementModelDB? ne = await context_forms.DirectoriesElements.FirstOrDefaultAsync(x => x.ParentId == directory_id && EF.Functions.Like(x.Name, name_element_of_dir), cancellationToken: cancellationToken);


        if (ne is not null)
        {
            res.AddError("Такой элемент справочника уже существует");
            return res;
        }

        ne = new() { Name = name_element_of_dir, ParentId = dir.Id, Parent = dir };

        try
        {
            await context_forms.AddAsync(ne, cancellationToken);
            await context_forms.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            res.AddError($"exception {{FE27D0BC-E1DA-4DE6-AB35-BB514A71D944}}: {ex.Message}");
            return res;
        }

        res.Id = ne.Id;
        res.AddSuccess($"Элемент справочника успешно создан #{res.Id}");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateElementOfDirectory(EntryModel element, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(element.Name))
            return ResponseBaseModel.CreateError("Имя элемента не может быть пустым");
        element.Name = Regex.Replace(element.Name, @"\s+", " ").Trim();
        ResponseBaseModel res = new();

        ConstructorFormDirectoryElementModelDB? element_db = await context_forms.DirectoriesElements.FirstOrDefaultAsync(x => x.Id == element.Id, cancellationToken: cancellationToken);
        if (element_db is null)
        {
            res.AddError($"Элемент справочника #{element.Id} не найден в БД");
            return res;
        }

        if (element_db.Name.Equals(element.Name))
        {
            res.AddInfo("Изменений нет - обновления не требуется");
            return res;
        }

        if (await context_forms.DirectoriesElements.AnyAsync(x => x.Id != element.Id && EF.Functions.Like(element_db.Name, element.Name) && x.ParentId == element_db.ParentId, cancellationToken: cancellationToken))
        {
            res.AddError("Элемент с таким именем уже существует в справочнике");
            return res;
        }

        element_db.Name = element.Name;
        context_forms.Update(element_db);
        await context_forms.SaveChangesAsync(cancellationToken);
        res.AddSuccess("Элемент справочника успешно обновлён");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteElementFromDirectory(int element_id, CancellationToken cancellationToken = default)
    {
        ConstructorFormDirectoryElementModelDB? el = await context_forms.DirectoriesElements.FirstOrDefaultAsync(x => x.Id == element_id, cancellationToken: cancellationToken);
        if (el is null)
            return ResponseBaseModel.CreateError($"Элемент справочника #{element_id} не найден в БД");
        context_forms.Remove(el);
        await context_forms.SaveChangesAsync(cancellationToken);

        return ResponseBaseModel.CreateSuccess($"Элемент справочника #{element_id} успешно удалён из БД");
    }
    #endregion
}