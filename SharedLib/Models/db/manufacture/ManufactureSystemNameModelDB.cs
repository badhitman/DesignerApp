using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// ManufactureSystemName
/// </summary>
[Index(nameof(TypeDataName), nameof(ManufactureId), nameof(SystemName), nameof(TypeDataId), IsUnique = true)]
public class ManufactureSystemNameModelDB : UpdateSystemNameModel, ICloneable
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Manufacture
    /// </summary>
    public ManageManufactureModelDB? Manufacture { get; set; }

    /// <inheritdoc/>
    public static ManufactureSystemNameModelDB Build(UpdateSystemNameModel request)
    {
        return new()
        {
            ManufactureId = request.ManufactureId,
            SystemName = request.SystemName,
            TypeDataName = request.TypeDataName,
            TypeDataId = request.TypeDataId,
            Qualification = request.Qualification,
        };
    }

    /// <inheritdoc/>
    public object Clone()
    {
        return new ManufactureSystemNameModelDB()
        {
            ManufactureId = ManufactureId,
            TypeDataId = TypeDataId,
            TypeDataName = TypeDataName,
            Qualification = Qualification,
            Id = Id,
            Manufacture = Manufacture,
            SystemName = SystemName
        };
    }
}