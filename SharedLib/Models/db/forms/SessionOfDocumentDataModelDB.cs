////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Сессия опроса/анкеты
/// </summary>
[Index(nameof(AuthorUser)), Index(nameof(SessionToken)), Index(nameof(SessionStatus)), Index(nameof(CreatedAt)), Index(nameof(LastQuestionnaireUpdateActivity)), Index(nameof(DeadlineDate))]
public class SessionOfDocumentDataModelDB
    : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Опрос/анкета
    /// </summary>
    public DocumentSchemeConstructorModelDB? Owner { get; set; }

    /// <summary>
    /// Автор/создатель ссылки
    /// </summary>
    public required string AuthorUser { get; set; }

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
    public List<ValueDataForSessionOfDocumentModelDB>? DataSessionValues { get; set; }

    /// <summary>
    /// ВВедённые значения в табличную форму
    /// </summary>
    public IQueryable<ValueDataForSessionOfDocumentModelDB>? QueryCurrentTablePageFormValues(int page_join_id)
    {
        return DataSessionValues?
               .Where(x => x.RowNum > 0 && x.TabJoinDocumentSchemeId == page_join_id)
               .AsQueryable();
    }

    /// <summary>
    /// 
    /// </summary>
    public IGrouping<uint, ValueDataForSessionOfDocumentModelDB>[]? RowsData(int page_join_id)
    {
        return QueryCurrentTablePageFormValues(page_join_id)?
               .GroupBy(x => x.RowNum)
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
    public void Reload(SessionOfDocumentDataModelDB other)
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
        if (other.DataSessionValues is not null)
        {
            DataSessionValues ??= [];

            int i = DataSessionValues.FindIndex(x => !other.DataSessionValues.Any(y => y.Id == x.Id));
            while (i >= 0)
            {
                DataSessionValues.RemoveAt(i);
                i = DataSessionValues.FindIndex(x => !other.DataSessionValues.Any(y => y.Id == x.Id));
            }

            ValueDataForSessionOfDocumentModelDB? _s;
            foreach (ValueDataForSessionOfDocumentModelDB sv in DataSessionValues)
            {
                _s = other.DataSessionValues.FirstOrDefault(x => x.Id == sv.Id);
                if (_s is not null)
                    sv.Value = _s.Value;
            }

            ValueDataForSessionOfDocumentModelDB[] _values = other.DataSessionValues.Where(x => !DataSessionValues.Any(y => y.Id == x.Id)).ToArray();
            if (_values.Length != 0)
                DataSessionValues.AddRange(_values);
        }

        if (other.Owner is not null && Owner is not null)
            Owner.Reload(other.Owner);
    }
}