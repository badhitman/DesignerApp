////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// E-Commerce Remote Transmission Service
/// </summary>
public interface ICommerceRemoteTransmissionService
{
    /// <summary>
    /// OrganizationsSelect
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>?>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req);

    /// <summary>
    /// OrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int?>> OrganizationUpdate(OrganizationModelDB org);

    /// <summary>
    /// OrganizationSetLegal
    /// </summary>
    public Task<TResponseModel<bool?>> OrganizationSetLegal(OrganizationModelDB org);

    /// <summary>
    /// OrganizationsRead
    /// </summary>
    public Task<TResponseModel<OrganizationModelDB[]?>> OrganizationsRead(int[] org);
}