////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib.Models;

/// <summary>
/// Основное поле/свойство документа (базовая модель)
/// </summary>
[Index(nameof(DocumentOwnerId))]
public abstract class DocumentPropertyMainModelDB : MetaMapBaseModelDB
{
    /// <summary>
    /// Идентификатор документа (внешний ключ)
    /// </summary>
    public int DocumentOwnerId { get; set; }

    /// <summary>
    /// Документ
    /// </summary>
    public DocumentDesignModelDB? DocumentOwner { get; set; }
}