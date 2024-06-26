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
    /// Сессия
    /// </summary>
    public SessionOfDocumentDataModelDB? Owner { get; set; }

    /// <summary>
    /// [FK] Связь со схемой документа
    /// </summary>
    public int TabJoinDocumentSchemeId { get; set; }
    /// <summary>
    /// Связь со схемой документа
    /// </summary>
    public TabJoinDocumentSchemeConstructorModelDB? TabJoinDocumentScheme { get; set; }

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

    /// <summary>
    /// Значение поля из формы опроса/анкеты
    /// </summary>
    public static ValueDataForSessionOfDocumentModelDB Build(SetValueFieldSessionQuestionnaireModel req, TabJoinDocumentSchemeConstructorModelDB questionnaire_page_join, SessionOfDocumentDataModelDB session)
        => new()
        {
            Name = req.NameField,
            Value = req.FieldValue,
            Description = req.Description,
            TabJoinDocumentSchemeId = questionnaire_page_join.Id,
            TabJoinDocumentScheme = questionnaire_page_join,
            Owner = session,
            OwnerId = session.OwnerId,
            GroupByRowNum = req.GroupByRowNum,
        };
}