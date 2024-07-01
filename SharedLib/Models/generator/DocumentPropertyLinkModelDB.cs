////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib.Models;

/// <summary>
/// Связи полей документов с вещественными типами данных
/// </summary>
[Index(nameof(TypedEnumId)), Index(nameof(TypedDocumentId))]
public class DocumentPropertyLinkModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    public int? OwnerPropertyMainBodyId { get; set; }

    /// <inheritdoc/>
    public DocumentPropertyMainBodyModelDB? OwnerPropertyMainBody { get; set; }
    /// <summary>
    /// Поле/свойство (владелец) табличной части документа.
    /// NULL если владелец - тело документа
    /// </summary>
    public int? OwnerPropertyMainGridId { get; set; }
    /// <summary>
    /// Поле/свойство (владелец) тела документа.
    /// NULL если владелец - табличная часть документа
    /// </summary>
    public DocumentPropertyGridModelDB? OwnerPropertyMainGrid { get; set; }

    /// <summary>
    /// Идентификатор тип:перечисления.
    /// NULL - если тип данных не перечисление
    /// </summary>
    public int? TypedEnumId { get; set; }
    /// <summary>
    /// Тип:перечисление. Ссылка на объект перечисления, который представляет тип данных, хранящийся в поле/свойстве документа.
    /// NULL - если тип данных не перечисление
    /// </summary>
    public EnumDesignModelDB? TypedEnum { get; set; }

    /// <summary>
    /// Идентификатор типа:документа
    /// NULL - если тип данных не документ
    /// </summary>
    public int? TypedDocumentId { get; set; }
    /// <summary>
    /// Тип:документ. Ссылка на объект документа, который представляет тип данных, хранящийся в поле/свойстве документа.
    /// NULL - если тип данных не документ
    /// </summary>
    public DocumentSchemeConstructorModelDB? TypedDocument { get; set; }

    /// <inheritdoc/>
    public int? TypeId
    {
        get
        {
            if (TypedDocumentId.GetValueOrDefault(0) > 0)
                return TypedDocumentId;
            if (TypedEnumId.GetValueOrDefault(0) > 0)
                return TypedEnumId;

            return null;
        }
    }

    /// <inheritdoc/>
    public static bool operator ==(DocumentPropertyLinkModelDB link1, DocumentPropertyLinkModelDB link2)
    {
        return link1.TypedEnumId == link2.TypedEnumId && link1.TypedDocumentId == link2.TypedDocumentId;
    }

    /// <inheritdoc/>
    public static bool operator !=(DocumentPropertyLinkModelDB prop_link1, DocumentPropertyLinkModelDB prop_link2) => !(prop_link1?.TypedEnumId == prop_link2?.TypedEnumId);


    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not DocumentPropertyLinkModelDB || obj is null)
        {
            return false;
        }
        DocumentPropertyLinkModelDB other = (DocumentPropertyLinkModelDB)obj;

        return this == other && Id == other.Id;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id} {TypedEnumId} {TypedEnum} {OwnerPropertyMainBodyId} {TypeId} {TypedDocumentId} {OwnerPropertyMainGridId}".GetHashCode();
    }
}