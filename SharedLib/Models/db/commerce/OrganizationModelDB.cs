////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Организация
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
    /// Находится ли объект в режиме запроса изменений реквизитов
    /// </summary>
    public bool HasRequestToChange
    {
        get
        {
            return
                !string.IsNullOrWhiteSpace(NewName) ||
                !string.IsNullOrWhiteSpace(NewBankBIC) ||
                !string.IsNullOrWhiteSpace(NewBankName) ||
                !string.IsNullOrWhiteSpace(NewLegalAddress) ||
                !string.IsNullOrWhiteSpace(NewCorrespondentAccount) ||
                !string.IsNullOrWhiteSpace(NewCurrentAccount) ||
                !string.IsNullOrWhiteSpace(NewINN) ||
                !string.IsNullOrWhiteSpace(NewKPP);
        }
    }

    /// <summary>
    /// Users
    /// </summary>
    public List<UserOrganizationModelDB>? Users { get; set; }

    /// <summary>
    /// Addresses
    /// </summary>
    public List<AddressOrganizationModelDB>? Addresses { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is OrganizationModelDB org_db)
            return
                org_db.IsDisabled == IsDisabled &&
                org_db.OGRN == OGRN &&
                org_db.CurrentAccount == CurrentAccount &&
                org_db.CorrespondentAccount == CorrespondentAccount &&
                org_db.BankName == BankName &&
                org_db.BankBIC == BankBIC &&
                org_db.Email == Email &&
                org_db.INN == INN &&
                org_db.Name == Name &&
                org_db.KPP == KPP &&
                org_db.Id == Id &&
                org_db.Phone == Phone &&
                org_db.LegalAddress == LegalAddress;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{Id}{KPP}{Name}{INN}{Email}{BankBIC}{BankName}{CorrespondentAccount}{CurrentAccount}{OGRN}{IsDisabled}{LegalAddress}{Phone}".GetHashCode();
}