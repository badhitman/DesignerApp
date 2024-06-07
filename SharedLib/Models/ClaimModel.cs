using System.Security.Claims;

namespace SharedLib;

/// <summary>
/// Claim
/// </summary>
public class ClaimModel : ClaimBaseModel
{
    /// <summary>
    /// Роль или пользователь (в зависимости от типа Claim)
    /// </summary>
    public required string OwnerId { get; set; }

    /// <summary>
    /// Build
    /// </summary>
    public static ClaimModel Build(int claimId, string? claimType, string? claimValue, string ownerId)
        => new() { ClaimType = claimType, ClaimValue = claimValue, OwnerId = ownerId, Id = claimId };
}