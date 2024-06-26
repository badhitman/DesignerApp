////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Значение поля из формы опроса/анкеты
/// </summary>
[Index(nameof(GroupByRowNum))]
public class ConstructorFormSessionValueModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Значение поля из формы опроса/анкеты
    /// </summary>
    public static ConstructorFormSessionValueModelDB Build(SetValueFieldSessionQuestionnaireModel req, ConstructorFormQuestionnairePageJoinFormModelDB questionnaire_page_join, ConstructorFormSessionModelDB session)
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
    public ConstructorFormSessionModelDB? Owner { get; set; }

    /// <summary>
    /// [FK] Форма
    /// </summary>
    public int QuestionnairePageJoinFormId { get; set; }
    /// <summary>
    /// Форма
    /// </summary>
    public ConstructorFormQuestionnairePageJoinFormModelDB? QuestionnairePageJoinForm { get; set; }

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