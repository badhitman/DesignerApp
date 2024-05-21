////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Простой объект с идентификатором/ID, именем и описанием
/// </summary>
public class IdNameDescriptionSimpleModel : NameDescriptionSimpleModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Required]
    [Key]
    public int Id { get; set; }
}
