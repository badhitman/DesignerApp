using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Сессия опроса/анкеты
/// </summary>
[Index(nameof(CreatorEmail)), Index(nameof(SessionToken)), Index(nameof(SessionStatus)), Index(nameof(CreatedAt)), Index(nameof(LastQuestionnaireUpdateActivity)), Index(nameof(DeadlineDate))]
public class ConstructorFormSessionModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Опрос/анкета
    /// </summary>
    public ConstructorFormQuestionnaireModelDB? Owner { get; set; }

    /// <summary>
    /// Адрес создателя
    /// </summary>
    public string CreatorEmail { get; set; } = string.Empty;

    /// <summary>
    /// Адреса для отправки уведомлений
    /// </summary>
    public string? EmailsNotifications { get; set; }

    /// <summary>
    /// Клиенты, вносившие данные в контексте сессии
    /// </summary>
    public string? Editors { get; set; }

    /// <summary>
    /// токен доступа к сессии
    /// </summary>
    public string? SessionToken { get; set; }

    /// <summary>
    /// Статус сессии
    /// </summary>
    public SessionsStatusesEnum SessionStatus { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Последнее обновление данных опроса/анкеты (последняя активность по вводу значений)
    /// </summary>
    public DateTime? LastQuestionnaireUpdateActivity { get; set; }

    /// <summary>
    /// Значения полей форм
    /// </summary>
    public List<ConstructorFormSessionValueModelDB>? SessionValues { get; set; }

    /// <summary>
    /// ВВедённые значения в табличную форму
    /// </summary>
    public IQueryable<ConstructorFormSessionValueModelDB>? QueryCurrentTablePageFormValues(int page_join_id)
    {
        return SessionValues?
               .Where(x => x.GroupByRowNum > 0 && x.QuestionnairePageJoinFormId == page_join_id)
               .AsQueryable();
    }

    /// <summary>
    /// 
    /// </summary>
    public IGrouping<uint, ConstructorFormSessionValueModelDB>[]? RowsData(int page_join_id)
    {
        return QueryCurrentTablePageFormValues(page_join_id)?
               .GroupBy(x => x.GroupByRowNum)
               .OrderBy(x => x.Key)
               .ToArray();
    }

    /// <summary>
    /// Первой вкладкой будет специальная для отображения описания.
    /// Что-то вроде вступительной страницы с общей информацией
    /// </summary>
    public bool ShowDescriptionAsStartPage { get; set; }

    /// <summary>
    /// Дата окончания актуальности сессии/токена
    /// </summary>
    public DateTime? DeadlineDate { get; set; }

    /// <summary>
    /// Перезагрузить объект
    /// </summary>
    public void Reload(ConstructorFormSessionModelDB other)
    {
        DeadlineDate = other.DeadlineDate;
        ShowDescriptionAsStartPage = other.ShowDescriptionAsStartPage;

        LastQuestionnaireUpdateActivity = other.LastQuestionnaireUpdateActivity;
        CreatedAt = other.CreatedAt;
        SessionStatus = other.SessionStatus;
        SessionToken = other.SessionToken;
        EmailsNotifications = other.EmailsNotifications;
        Name = other.Name;
        Description = other.Description;
        if (other.SessionValues is not null)
        {
            SessionValues ??= [];

            int i = SessionValues.FindIndex(x => !other.SessionValues.Any(y => y.Id == x.Id));
            while (i >= 0)
            {
                SessionValues.RemoveAt(i);
                i = SessionValues.FindIndex(x => !other.SessionValues.Any(y => y.Id == x.Id));
            }

            ConstructorFormSessionValueModelDB? _s;
            foreach (ConstructorFormSessionValueModelDB sv in SessionValues)
            {
                _s = other.SessionValues.FirstOrDefault(x => x.Id == sv.Id);
                if (_s is not null)
                    sv.Value = _s.Value;
            }

            ConstructorFormSessionValueModelDB[] _values = other.SessionValues.Where(x => !SessionValues.Any(y => y.Id == x.Id)).ToArray();
            if (_values.Length != 0)
                SessionValues.AddRange(_values);
        }

        if (other.Owner is not null && Owner is not null)
            Owner.Reload(other.Owner);
    }
}