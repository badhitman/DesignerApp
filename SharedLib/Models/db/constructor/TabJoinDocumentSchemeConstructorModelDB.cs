////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Связь формы со страницей опроса/анкеты
/// </summary>
[Index(nameof(SortIndex)), Index(nameof(IsTable))]
public class TabJoinDocumentSchemeConstructorModelDB
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

    /// <summary>
    /// Связь формы со страницей опроса/анкеты
    /// </summary>
    public static TabJoinDocumentSchemeConstructorModelDB Build(EntryDescriptionOwnedModel page_join_form)
        => new()
        {
            FormId = page_join_form.OwnerId,
            Description = page_join_form.Description,
            Id = page_join_form.Id,
            Name = page_join_form.Name,
        };
}