////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// AddressOrganizationBaseModel
/// </summary>
public class AddressOrganizationBaseModel : EntryModel
{
    /// <summary>
    /// Регион/Город
    /// </summary>
    public required int ParentId { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [Required]
    public required string Address { get; set; }

    /// <summary>
    /// Contacts
    /// </summary>
    [Required]
    public string? Contacts { get; set; }

    /// <summary>
    /// Organization
    /// </summary>
    public int OrganizationId { get; set; }
}