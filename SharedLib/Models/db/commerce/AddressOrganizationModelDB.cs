////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// AddressOrganizationModelDB
/// </summary>
public class AddressOrganizationModelDB : AddressOrganizationBaseModel
{
    /// <summary>
    /// Organization
    /// </summary>
    public OrganizationModelDB? Organization { get; set; }
}