////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Связь формы со страницей опроса/анкеты
/// </summary>
[Index(nameof(SortIndex)), Index(nameof(IsTable))]
public class FormToTabJoinConstructorModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Описание/примечание для объекта
    /// </summary>
    public string? Description { get; set; }

    /// <inheritdoc/>
    public string? Name { get; set; }

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
    /// Таб/Вкладка
    /// </summary>
    public int TabId { get; set; }

    /// <summary>
    /// Таб/Вкладка
    /// </summary>
    public TabOfDocumentSchemeConstructorModelDB? Tab { get; set; }

    /// <summary>
    /// [FK] Форма
    /// </summary>
    public int FormId { get; set; }
    /// <summary>
    /// Форма
    /// </summary>
    public FormConstructorModelDB? Form { get; set; }
    
    /// <inheritdoc/>
    public static bool operator ==(FormToTabJoinConstructorModelDB tj1, FormToTabJoinConstructorModelDB tj2)
    {
        return
            tj1.Id == tj2.Id &&
            tj1.Description == tj2.Description &&
            tj1.Name == tj2.Name &&
            tj1.SortIndex == tj2.SortIndex &&
            tj1.ShowTitle == tj2.ShowTitle &&
            tj1.FormId == tj2.FormId &&
            tj1.TabId == tj2.TabId &&
            tj1.IsTable == tj2.IsTable;
    }

    /// <inheritdoc/>
    public static bool operator !=(FormToTabJoinConstructorModelDB tj1, FormToTabJoinConstructorModelDB tj2)
    {
        return
            tj1.Id != tj2.Id ||
            tj1.Description != tj2.Description ||
            tj1.Name != tj2.Name ||
            tj1.SortIndex != tj2.SortIndex ||
            tj1.ShowTitle != tj2.ShowTitle ||
            tj1.FormId != tj2.FormId ||
            tj1.TabId != tj2.TabId ||
            tj1.IsTable != tj2.IsTable;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is FormToTabJoinConstructorModelDB tj)
            return this == tj;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id}{Description}{Name}{SortIndex}{ShowTitle}{FormId}{TabId}{IsTable}".GetHashCode();
    }

    /// <summary>
    /// Связь формы со страницей опроса/анкеты
    /// </summary>
    public static FormToTabJoinConstructorModelDB Build(EntryDescriptionOwnedModel page_join_form)
        => new()
        {
            FormId = page_join_form.OwnerId,
            Description = page_join_form.Description,
            Id = page_join_form.Id,
            Name = page_join_form.Name,
        };
}