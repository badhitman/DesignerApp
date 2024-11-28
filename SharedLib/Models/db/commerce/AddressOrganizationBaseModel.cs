////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Адрес организации (базовая модель)
/// </summary>
public class AddressOrganizationBaseModel : EntryModel
{
    /// <summary>
    /// Название
    /// </summary>
    public override required string Name { get; set; }

    /// <summary>
    /// Регион/Город
    /// </summary>
    public required int ParentId { get; set; }

    /// <summary>
    /// Адрес
    /// </summary>
    [Required]
    public required string Address { get; set; }

    /// <summary>
    /// Контакты
    /// </summary>
    [Required]
    public string? Contacts { get; set; }

    /// <summary>
    /// Организация
    /// </summary>
    public int OrganizationId { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name} {Address} {(string.IsNullOrEmpty(Contacts) ? "" : $" (контакты:{Contacts})")}".Trim().Replace("  ", " ");
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id}{Name}{OrganizationId}{Address}{ParentId}{Contacts}".GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is AddressOrganizationBaseModel add)
            return
                add.Id == Id &&
                add.Contacts == Contacts &&
                add.OrganizationId == OrganizationId &&
                add.Address == Address &&
                add.Name == Name &&
                add.ParentId == ParentId;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public static bool operator ==(AddressOrganizationBaseModel off1, AddressOrganizationBaseModel off2) =>
        off1.Id == off2.Id &&
        off1.Name == off2.Name &&
        off1.ParentId == off2.ParentId &&
        off1.Contacts == off2.Contacts &&
        off1.Address == off2.Address;

    /// <inheritdoc/>
    public static bool operator !=(AddressOrganizationBaseModel off1, AddressOrganizationBaseModel off2)
    {
        return
                off1.Id != off2.Id ||
                off1.Name != off2.Name ||
                off1.ParentId != off2.ParentId ||
                off1.Contacts != off2.Contacts ||
                off1.Address != off2.Address;
    }
}