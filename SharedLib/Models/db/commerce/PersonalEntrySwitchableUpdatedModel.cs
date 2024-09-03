////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// PersonalEntrySwitchableUpdatedModel
/// </summary>
[Index(nameof(UserPersonIdentityId))]
public class PersonalEntrySwitchableUpdatedModel : EntrySwitchableUpdatedModel
{
    /// <summary>
    /// UserPersonIdentityId
    /// </summary>
    public required string UserPersonIdentityId { get; set; }
}