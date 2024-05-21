////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Результат регистрации нового пользователя
/// </summary>
public class RegistrationNewUserResponseModel : TResponseModel<string?>
{
    /// <summary>
    /// Флаг, указывающий, требуется ли для входа подтвержденная учетная запись.
    /// </summary>
    /// <value>True, если у пользователя должна быть подтвержденная учетная запись, прежде чем он сможет войти в систему, в противном случае — false.</value>
    public bool? RequireConfirmedAccount { get; set; }

    /// <summary>
    /// Флаг, указывающий, требуется ли для входа подтвержденный адрес электронной почты.
    /// </summary>
    /// <value>True, если у пользователя должен быть подтвержденный адрес электронной почты, прежде чем он сможет войти в систему, в противном случае — false.</value>
    public bool? RequireConfirmedEmail { get; set; }

    /// <summary>
    /// Флаг, указывающий, требуется ли для входа в систему подтвержденный номер телефона.
    /// </summary>
    /// <value>True, если у пользователя должен быть подтвержденный номер телефона, прежде чем он сможет войти в систему, в противном случае — false.</value>
    public bool? RequireConfirmedPhoneNumber { get; set; }
}