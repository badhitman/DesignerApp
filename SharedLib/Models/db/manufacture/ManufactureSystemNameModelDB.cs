using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// ManufactureSystemName
/// </summary>
[Index(nameof(TypeDataName), nameof(ManufactureId), nameof(SystemName), nameof(TypeDataId), IsUnique = true)]
public class ManufactureSystemNameModelDB : UpdateSystemNameModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

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
}