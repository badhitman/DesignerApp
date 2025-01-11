////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace IdentityLib;

/// <summary>
/// IdentityStatic
/// </summary>
public static class IdentityStatic
{
    /// <summary>
    /// CreateInstanceUser
    /// </summary>
    public static ApplicationUser CreateInstanceUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Не могу создать экземпляр '{nameof(ApplicationUser)}'. " +
                $"Убедитесь, что '{nameof(ApplicationUser)}' не является абстрактным классом и имеет конструктор без параметров.");
        }
    }
}