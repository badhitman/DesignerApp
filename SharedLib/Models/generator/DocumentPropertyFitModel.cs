////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Поле документа (лёгкая модель)
/// </summary>
public class DocumentPropertyFitModel : SortableFitRealTypeModel
{
    /// <summary>
    /// Тип данных поля
    /// </summary>
    public PropertyTypesEnum PropertyType { get; set; }

    /// <summary>
    /// Метаданные поля
    /// </summary>
    public BaseFitRealTypeModel? PropertyTypeMetadata { get; set; }

    /// <inheritdoc/>
    public static explicit operator DocumentPropertyFitModel(DocumentPropertyMainBodyModelDB v)
    {
        return new DocumentPropertyFitModel()
        {
            SystemName = v.SystemName,
            Name = v.Name,
    //        Description = v.Description,
    //        Id = v.Id,
    //        PropertyType = v.PropertyType,
    //        SortIndex = v.SortIndex,
    //        PropertyTypeMetadata = v.PropertyType switch
    //        {
    //            PropertyTypesEnum.Int => null,
    //            PropertyTypesEnum.String => null,
    //            PropertyTypesEnum.Bool => null,
    //            PropertyTypesEnum.Decimal => null,
    //            PropertyTypesEnum.DateTime => null,
    //            PropertyTypesEnum.SimpleEnum => new BaseFitRealTypeModel()
    //            {
    //                Id = v.PropertyLink.TypedEnum.Id,
    //                Description = v.PropertyLink.TypedEnum.Description,
    //                Name = v.PropertyLink.TypedEnum.Name,
    //                SystemName = v.PropertyLink.TypedEnum.SystemName,
    //                IsDeleted = v.PropertyLink.TypedEnum.IsDeleted
    //            },
    //            PropertyTypesEnum.Document => new BaseFitRealTypeModel()
    //            {
    //                Id = v.PropertyLink.TypedDocument.Id,
    //                Description = v.PropertyLink.TypedDocument.Description,
    //                Name = v.PropertyLink.TypedDocument.Name,
    //                SystemName = v.PropertyLink.TypedDocument.SystemName,
    //                IsDeleted = v.PropertyLink.TypedDocument.IsDeleted
    //            }
    //        }
        };
    }

    /// <inheritdoc/>
    public static explicit operator DocumentPropertyFitModel(DocumentPropertyGridModelDB v)
    {
        return new DocumentPropertyFitModel()
        {
            SystemName = v.SystemName,
            Name = v.Name,
    //        IsDeleted = v.IsDeleted,
    //        Description = v.Description,
    //        Id = v.Id,
    //        PropertyType = v.PropertyType,
    //        SortIndex = v.SortIndex,
    //        PropertyTypeMetadata = v.PropertyType switch
    //        {
    //            PropertyTypesEnum.Int => null,
    //            PropertyTypesEnum.String => null,
    //            PropertyTypesEnum.Bool => null,
    //            PropertyTypesEnum.Decimal => null,
    //            PropertyTypesEnum.DateTime => null,
    //            PropertyTypesEnum.SimpleEnum => new BaseFitRealTypeModel()
    //            {
    //                Id = v.PropertyLink.TypedEnum.Id,
    //                Description = v.PropertyLink.TypedEnum.Description,
    //                Name = v.PropertyLink.TypedEnum.Name,
    //                SystemName = v.PropertyLink.TypedEnum.SystemName,
    //                IsDeleted = v.PropertyLink.TypedEnum.IsDeleted
    //            },
    //            PropertyTypesEnum.Document => new BaseFitRealTypeModel()
    //            {
    //                Id = v.PropertyLink.TypedDocument.Id,
    //                Description = v.PropertyLink.TypedDocument.Description,
    //                Name = v.PropertyLink.TypedDocument.Name,
    //                SystemName = v.PropertyLink.TypedDocument.SystemName,
    //                IsDeleted = v.PropertyLink.TypedDocument.IsDeleted
    //            }
    //        }
        };
    }
}