////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Organization
/// </summary>
public class OrganizationModelDB : OrganizationLegalModel
{
    /// <summary>
    /// НОВОЕ Название (запрос изменений)
    /// </summary>
    public string? NewName { get; set; }
    /// <summary>
    /// НОВЫЙ Юридический адрес (запрос изменений)
    /// </summary>
    public string? NewLegalAddress { get; set; }
    /// <summary>
    /// НОВЫЙ ИНН (запрос изменений)
    /// </summary>
    public string? NewINN { get; set; }
    /// <summary>
    /// НОВЫЙ КПП (запрос изменений)
    /// </summary>
    public string? NewKPP { get; set; }
    /// <summary>
    /// НОВЫЙ ОГРН (запрос изменений)
    /// </summary>
    public string? NewOGRN { get; set; }
    /// <summary>
    /// НОВЫЙ Расчетный счет (запрос изменений)
    /// </summary>
    public string? NewCurrentAccount { get; set; }
    /// <summary>
    /// НОВЫЙ Корр. счет (запрос изменений)
    /// </summary>
    public string? NewCorrespondentAccount { get; set; }
    /// <summary>
    /// НОВЫЙ Банк (запрос изменений)
    /// </summary>
    public string? NewBankName { get; set; }
    /// <summary>
    /// НОВЫЙ БИК Банка (запрос изменений)
    /// </summary>
    public string? NewBankBIC { get; set; }


    /// <summary>
    /// Users
    /// </summary>
    public List<UserOrganizationModelDB>? Users { get; set; }

    /// <summary>
    /// Addresses
    /// </summary>
    public List<AddressOrganizationModelDB>? Addresses { get; set; }
}