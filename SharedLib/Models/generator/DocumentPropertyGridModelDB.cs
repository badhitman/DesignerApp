////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib.Models;

/// <summary>
/// Поле/свойство (основное) в таблице документа
/// </summary>
[Comment("Реквизиты табличной части документов")]
[Index(nameof(PropertyLinkId))]
public class DocumentPropertyGridModelDB : MetaMapBaseModelDB
{
    /// <summary>
    /// FK: Табличная часть документа
    /// </summary>
    public int GridId { get; set; }

    /// <summary>
    /// Табличная часть документа
    /// </summary>
    public DocumentGridModelDB? Grid { get; set; }
}