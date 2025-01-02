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
    /// OrganizationOfferContractUpdate
    /// </summary>
    public Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req);

    /// <summary>
    /// ContractorsOrganizationsFind
    /// </summary>
    public Task<OrganizationContractorModel[]> ContractorsOrganizationsFind(ContractorsOrganizationsRequestModel req);

    /// <summary>
    /// UsersOrganizations select
    /// </summary>
    public Task<TPaginationResponseModel<UserOrganizationModelDB>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req);

    /// <summary>
    /// UserOrganization update
    /// </summary>
    public Task<TResponseModel<int>> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> req);

    /// <summary>
    /// UsersOrganizations read
    /// </summary>
    public Task<TResponseModel<UserOrganizationModelDB[]>> UsersOrganizationsRead(int[] req);

    /// <summary>
    /// Organization update
    /// </summary>
    public Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> req);

    /// <summary>
    /// Organizations select
    /// </summary>
    public Task<TPaginationResponseModel<OrganizationModelDB>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req);

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