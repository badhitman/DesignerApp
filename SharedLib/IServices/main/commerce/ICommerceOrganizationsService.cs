////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Организации
/// </summary>
public partial interface ICommerceService
{
    /// <summary>
    /// Organization update
    /// </summary>
    public Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> req);

    /// <summary>
    /// Organizations select
    /// </summary>
    public Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req);

    /// <summary>
    /// Organizations read
    /// </summary>
    public Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] req);

    /// <summary>
    /// Organization set-legal
    /// </summary>
    public Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationLegalModel req);

    /// <summary>
    /// AddressOrganizationUpdate
    /// </summary>
    public Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req);

    /// <summary>
    /// AddressOrganizationDelete
    /// </summary>
    public Task<ResponseBaseModel> AddressOrganizationDelete(int address_id);

    /// <summary>
    /// AddressesOrganizationsRead
    /// </summary>
    public Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] organizationsIds);
}