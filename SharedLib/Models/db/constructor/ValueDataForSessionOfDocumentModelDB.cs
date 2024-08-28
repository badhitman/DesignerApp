////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Значение поля из формы опроса/анкеты
/// </summary>
[Index(nameof(RowNum))]
public class ValueDataForSessionOfDocumentModelDB : EntryDescriptionOwnedModel
{
    /// <summary>
    /// Сессия
    /// </summary>
    public SessionOfDocumentDataModelDB? Owner { get; set; }

    /// <summary>
    /// [FK] Связь со схемой документа
    /// </summary>
    public int JoinFormToTabId { get; set; }
    /// <summary>
    /// Связь формы с вкладкой/табом и далее со схемой документа
    /// </summary>
    public FormToTabJoinConstructorModelDB? JoinFormToTab { get; set; }

    /// <summary>
    /// Значение поля
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Номер строки данных если это таблица. если это не таблица - тогда 0
    /// </summary>
    public uint RowNum { get; set; }


    /// <inheritdoc/>
    public override string ToString() => $"{Name}: [{Value}]";

    /// <inheritdoc/>
    public static bool operator ==(ValueDataForSessionOfDocumentModelDB val1, ValueDataForSessionOfDocumentModelDB val2)
    {
        return
                val1.Name == val2.Name &&
                val1.Value == val2.Value &&
                val1.RowNum == val2.RowNum &&
                val1.JoinFormToTabId == val2.JoinFormToTabId &&
                val1.Description == val2.Description &&
                val1.OwnerId == val2.OwnerId &&
                val1.Id == val2.Id;
    }

    /// <inheritdoc/>
    public static bool operator !=(ValueDataForSessionOfDocumentModelDB val1, ValueDataForSessionOfDocumentModelDB val2)
    {
        return
            val1.Id != val2.Id ||
            val1.Name != val2.Name ||
            val1.Value != val2.Value ||
            val1.RowNum != val2.RowNum ||
            val1.JoinFormToTabId != val2.JoinFormToTabId ||
            val1.Description != val2.Description ||
            val1.OwnerId == val2.OwnerId;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is ValueDataForSessionOfDocumentModelDB val)
            return
                val.Name == Name &&
                val.Value == Value &&
                val.RowNum == RowNum &&
                val.JoinFormToTabId == JoinFormToTabId &&
                val.Description == Description &&
                val.OwnerId == OwnerId &&
                val.Id == Id;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id}{Name}{Value}{RowNum}{JoinFormToTabId}{Description}{OwnerId}".GetHashCode();
    }

    /// <summary>
    /// Значение поля из формы опроса/анкеты
    /// </summary>
    public static ValueDataForSessionOfDocumentModelDB Build(SetValueFieldDocumentDataModel req, FormToTabJoinConstructorModelDB questionnaire_page_join, SessionOfDocumentDataModelDB session)
        => new()
        {
            Name = req.NameField,
            Value = req.FieldValue,
            Description = req.Description,
            JoinFormToTabId = questionnaire_page_join.Id,
            JoinFormToTab = questionnaire_page_join,
            Owner = session,
            OwnerId = session.OwnerId,
            RowNum = req.GroupByRowNum,
        };
}