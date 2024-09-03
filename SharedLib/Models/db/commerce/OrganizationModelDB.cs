////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Organization
/// </summary>
public class OrganizationModelDB : EntrySwitchableUpdatedModel
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
    /// НОВЫЙ Юридический адрес (запрос изменений)
    /// </summary>
    public string? NewLegalAddress { get; set; }

    /// <summary>
    /// ИНН
    /// </summary>
    [Required]
    public required string INN { get; set; }
    /// <summary>
    /// НОВЫЙ ИНН (запрос изменений)
    /// </summary>
    public string? NewINN { get; set; }

    /// <summary>
    /// КПП
    /// </summary>
    [Required]
    public required string KPP { get; set; }
    /// <summary>
    /// НОВЫЙ КПП (запрос изменений)
    /// </summary>
    [Required]
    public string? NewKPP { get; set; }

    /// <summary>
    /// ОГРН
    /// </summary>
    [Required]
    public required string OGRN { get; set; }
    /// <summary>
    /// НОВЫЙ ОГРН (запрос изменений)
    /// </summary>
    [Required]
    public required string NewOGRN { get; set; }

    /// <summary>
    /// Расчетный счет
    /// </summary>
    [Required]
    public required string CurrentAccount { get; set; }
    /// <summary>
    /// НОВЫЙ Расчетный счет (запрос изменений)
    /// </summary>
    public string? NewCurrentAccount { get; set; }

    /// <summary>
    /// Корр. счет
    /// </summary>
    [Required]
    public required string CorrespondentAccount { get; set; }
    /// <summary>
    /// НОВЫЙ Корр. счет (запрос изменений)
    /// </summary>
    public string? NewCorrespondentAccount { get; set; }

    /// <summary>
    /// Банк
    /// </summary>
    [Required]
    public required string BankName { get; set; }
    /// <summary>
    /// НОВЫЙ Банк (запрос изменений)
    /// </summary>
    public string? NewBankName { get; set; }

    /// <summary>
    /// БИК Банка
    /// </summary>
    [Required]
    public required string BankBIC { get; set; }
    /// <summary>
    /// НОВЫЙ БИК Банка (запрос изменений)
    /// </summary>
    [Required]
    public required string NewBankBIC { get; set; }


    /// <summary>
    /// Users
    /// </summary>
    public List<UserOrganizationModelDB>? Users { get; set; }

    /// <summary>
    /// Addresses
    /// </summary>
    public List<AddressOrganizationModelDB>? Addresses { get; set; }
}