namespace SharedLib;

/// <summary>
/// Информация для входа и источник записи пользователя.
/// </summary>
/// <param name="loginProvider">Поставщик, связанный с этой информацией для входа.</param>
/// <param name="providerKey">Уникальный идентификатор этого пользователя, предоставленный провайдером входа.</param>
/// <param name="displayName">Отображаемое имя поставщика входа в систему.</param>
/// <remarks>
/// Представляет информацию для входа и источник записи пользователя.
/// </remarks>
public class UserLoginInfoModel(string loginProvider, string providerKey, string? displayName)
{
    /// <summary>
    /// Поставщик для этого экземпляра <see cref="UserLoginInfoModel"/>.
    /// </summary>
    /// <value>Поставщик этого экземпляра <see cref="UserLoginInfoModel"/></value>
    /// <remarks>
    /// Примерами провайдера могут быть Local, Facebook, Google и т. д.
    /// </remarks>
    public string LoginProvider { get; set; } = loginProvider;

    /// <summary>
    /// Уникальный идентификатор пользователя, предоставленный поставщиком входа в систему.
    /// </summary>
    /// <value>
    /// Уникальный идентификатор пользователя, предоставляемый провайдером входа.
    /// </value>
    /// <remarks>
    /// Это будет уникально для каждого поставщика, например, @microsoft в качестве ключа поставщика Twitter.
    /// </remarks>
    public string ProviderKey { get; set; } = providerKey;

    /// <summary>
    /// Отображаемое имя поставщика.
    /// </summary>
    /// <remarks>
    /// Примерами отображаемого имени могут быть local, FACEBOOK, Google и т. д.
    /// </remarks>
    public string? ProviderDisplayName { get; set; } = displayName;
}