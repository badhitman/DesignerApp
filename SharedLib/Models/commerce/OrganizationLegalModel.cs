////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// OrganizationLegalModel
/// </summary>
public class OrganizationLegalModel : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// Телефон
    /// </summary>
    [Required]
    public required string Phone { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    [Required]
    public required string Email { get; set; }


    /// <summary>
    /// Юридический адрес
    /// </summary>
    [Required]
    public required string LegalAddress { get; set; }

    /// <summary>
    /// ИНН
    /// </summary>
    [Required]
    public required string INN { get; set; }

    /// <summary>
    /// КПП
    /// </summary>
    [Required]
    public required string KPP { get; set; }

    /// <summary>
    /// ОГРН
    /// </summary>
    [Required]
    public required string OGRN { get; set; }

    /// <summary>
    /// Расчетный счет
    /// </summary>
    [Required]
    public required string CurrentAccount { get; set; }

    /// <summary>
    /// Корр. счет
    /// </summary>
    [Required]
    public required string CorrespondentAccount { get; set; }

    /// <summary>
    /// Банк
    /// </summary>
    [Required]
    public required string BankName { get; set; }

    /// <summary>
    /// БИК Банка
    /// </summary>
    [Required]
    public required string BankBIC { get; set; }
}