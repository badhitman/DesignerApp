////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// принадлежность Claim: для пользователя или для роли
/// </summary>
public enum ClaimAreasEnum
{
    /// <summary>
    /// Для пользователей
    /// </summary>
    [Description("Для пользователей")]
    ForUser,

    /// <summary>
    /// Для ролей
    /// </summary>
    [Description("Для ролей")]
    ForRole
}
