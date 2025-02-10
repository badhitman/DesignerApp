////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Altnames содержит сведения о соответствии кодов старых и новых наименований (обозначений домов) в случаях переподчинения 
/// и “сложного” переименования адресных объектов (когда коды записей со старым и новым наименованиями не совпадают).
/// </summary>
/// <remarks>
/// Возможные варианты “сложного” переименования:
/// улица разделилась на несколько новых улиц;
/// несколько улиц объединились в одну новую улицу;
/// населенный пункт стал улицей другого города(населенного пункта);
/// улица населенного пункта стала улицей другого города(населенного пункта).
/// В этих случаях производятся следующие действия:
/// вводятся новые объекты в файлы Kladr.dbf, Street.dbf и Doma.dbf;
/// старые объекты переводятся в разряд неактуальных;
/// в файл Altnames вводятся записи, содержащие соответствие старых и новых кодов адресных объектов.
/// </remarks>
[Index(nameof(OLDCODE)), Index(nameof(NEWCODE)), Index(nameof(LEVEL))]
public class AltnameKLADRModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <inheritdoc/>
    [Required, StringLength(19)]
    public required string OLDCODE { get; set; }

    /// <inheritdoc/>
    [Required, StringLength(19)]
    public required string NEWCODE { get; set; }

    /// <inheritdoc/>
    [Required, StringLength(1)]
    public required string LEVEL { get; set; }
}