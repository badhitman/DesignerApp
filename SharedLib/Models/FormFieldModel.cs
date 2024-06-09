using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// 
/// </summary>
public class FormFieldModel
{
    /// <summary>
    /// 
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public string FieldName { get; set; } = default!;
}