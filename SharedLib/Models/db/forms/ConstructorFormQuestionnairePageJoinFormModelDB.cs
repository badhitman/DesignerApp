using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Связь формы со страницей опросоа/анкеты
/// </summary>
[Index(nameof(SortIndex)), Index(nameof(IsTable))]
public class ConstructorFormQuestionnairePageJoinFormModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Связь формы со страницей опроса/анкеты
    /// </summary>
    public static ConstructorFormQuestionnairePageJoinFormModelDB Build(EntryDescriptionOwnedModel page_join_form)
        => new()
        {
            OwnerId = page_join_form.OwnerId,
            Description = page_join_form.Description,
            Id = page_join_form.Id,
            Name = page_join_form.Name,
        };

    /// <summary>
    /// Сортировка 
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// Отображать заголовок таблицы
    /// </summary>
    public bool ShowTitle { get; set; }

    /// <summary>
    /// Табличная часть
    /// </summary>
    public bool IsTable { get; set; }

    /// <summary>
    /// Страница
    /// </summary>
    public ConstructorFormQuestionnairePageModelDB? Owner { get; set; }

    /// <summary>
    /// [FK] Форма
    /// </summary>
    public int FormId { get; set; }
    /// <summary>
    /// Форма
    /// </summary>
    public ConstructorFormModelDB? Form { get; set; }
}