////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Значение поля из формы опроса/анкеты
/// </summary>
[Index(nameof(GroupByRowNum))]
public class ValueDataForSessionOfDocumentModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Значение поля из формы опроса/анкеты
    /// </summary>
    public static ValueDataForSessionOfDocumentModelDB Build(SetValueFieldSessionQuestionnaireModel req, TabJoinDocumentSchemeConstructorModelDB questionnaire_page_join, SessionOfDocumentDataModelDB session)
        => new()
        {
            Name = req.NameField,
            Value = req.FieldValue,
            Description = req.Description,
            QuestionnairePageJoinFormId = questionnaire_page_join.Id,
            QuestionnairePageJoinForm = questionnaire_page_join,
            Owner = session,
            OwnerId = session.OwnerId,
            GroupByRowNum = req.GroupByRowNum
        };

    /// <summary>
    /// Сессия
    /// </summary>
    public SessionOfDocumentDataModelDB? Owner { get; set; }

    /// <summary>
    /// [FK] Форма
    /// </summary>
    public int QuestionnairePageJoinFormId { get; set; }
    /// <summary>
    /// Форма
    /// </summary>
    public TabJoinDocumentSchemeConstructorModelDB? QuestionnairePageJoinForm { get; set; }

    /// <summary>
    /// Значение поля
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Группировка по номеру строки
    /// </summary>
    public uint GroupByRowNum { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}: [{Value}]";
}