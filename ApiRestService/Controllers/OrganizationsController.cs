﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLib;

namespace ApiRestService.Controllers;

/// <summary>
/// Организации
/// </summary>
[Route("api/[controller]/[action]"), ApiController, ServiceFilter(typeof(UnhandledExceptionAttribute)), LoggerNolog, Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrganizationsReadCommerce)},{nameof(ExpressApiRolesEnum.OrganizationsWriteCommerce)}")]
public class OrganizationsController(ICommerceRemoteTransmissionService commRepo) : ControllerBase
{
    /// <summary>
    /// Прочитать данные организаций по их идентификаторам
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrganizationsReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrganizationsWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.ORGANIZATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.READ_ACTION_NAME}")]
    public async Task<TResponseModel<OrganizationModelDB[]>> ReadOrganizations(int[] organizations_ids)
        => await commRepo.OrganizationsRead(organizations_ids);

    /// <summary>
    /// Подбор организаций с параметрами запроса
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrganizationsReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrganizationsWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.ORGANIZATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}")]
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req)
        => await commRepo.OrganizationsSelect(req);

    /// <summary>
    /// Прочитать данные адресов организаций по их идентификаторам
    /// </summary>
    /// <remarks>
    /// Роли: <see cref="ExpressApiRolesEnum.OrganizationsReadCommerce"/>, <see cref="ExpressApiRolesEnum.OrganizationsWriteCommerce"/>
    /// </remarks>
    [HttpPut($"/api/{GlobalStaticConstants.Routes.ORGANIZATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ADDRESSES_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.READ_ACTION_NAME}")]
    public async Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] ids)
        => await commRepo.AddressesOrganizationsRead(ids);


    /// <summary>
    /// Установить реквизиты организации (+ сброс запроса редактирования)
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrganizationsWriteCommerce"/>.
    /// Если организация находиться в статусе запроса изменения реквизитов - этот признак обнуляется.
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORGANIZATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LEGAL_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrganizationsWriteCommerce)}"), LoggerLog]
    public async Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationModelDB org)
        => await commRepo.OrganizationSetLegal(org);

    /// <summary>
    /// Обновление параметров организации. Юридические параметры не меняются, а формируется запрос на изменение, которое должна подтвердить сторонняя система
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrganizationsWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORGANIZATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrganizationsWriteCommerce)}"), LoggerLog]
    public async Task<TResponseModel<int>> OrganizationUpdate(OrganizationModelDB org)
        => await commRepo.OrganizationUpdate(new() { Payload = org, SenderActionUserId = GlobalStaticConstants.Roles.System });

    /// <summary>
    /// Обновить/Создать адрес организации
    /// </summary>
    /// <remarks>
    /// Роль: <see cref="ExpressApiRolesEnum.OrganizationsWriteCommerce"/>
    /// </remarks>
    [HttpPost($"/api/{GlobalStaticConstants.Routes.ORGANIZATIONS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.ADDRESS_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}"), Authorize(Roles = $"{nameof(ExpressApiRolesEnum.OrganizationsWriteCommerce)}"), LoggerLog]
    public async Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
        => await commRepo.AddressOrganizationUpdate(req);
}