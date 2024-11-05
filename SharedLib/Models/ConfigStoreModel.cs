////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// ConfigStoreModel
/// </summary>
public class ConfigStoreModel
{
    /// <summary>
    /// ApiAddress
    /// </summary>
    public string? ApiAddress { get; set; }

    /// <summary>
    /// AccessToken
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// LocalDirectory
    /// </summary>
    public string? LocalDirectory { get; set; }

    /// <summary>
    /// RemoteDirectory
    /// </summary>
    public string? RemoteDirectory { get; set; }

    /// <summary>
    /// Форма заполнена
    /// </summary>
    public bool ConnectionSet =>
        !string.IsNullOrWhiteSpace(AccessToken) &&
        !string.IsNullOrWhiteSpace(ApiAddress) && Uri.TryCreate(ApiAddress, uriKind: UriKind.Absolute, out _);

    /// <summary>
    /// Форма заполнена
    /// </summary>
    public bool FullSets =>
        ConnectionSet &&
        !string.IsNullOrWhiteSpace(LocalDirectory) &&
        !string.IsNullOrWhiteSpace(RemoteDirectory);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return JsonConvert.SerializeObject(this).GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is ConfigStoreModel _ob)
            return
                (_ob.AccessToken == AccessToken || (string.IsNullOrEmpty(_ob.AccessToken) && string.IsNullOrEmpty(AccessToken))) &&
                (_ob.LocalDirectory == LocalDirectory || (string.IsNullOrEmpty(_ob.LocalDirectory) && string.IsNullOrEmpty(LocalDirectory))) &&
                (_ob.RemoteDirectory == RemoteDirectory || (string.IsNullOrEmpty(_ob.RemoteDirectory) && string.IsNullOrEmpty(RemoteDirectory))) &&
                (_ob.ApiAddress == ApiAddress || (string.IsNullOrEmpty(_ob.ApiAddress) && string.IsNullOrEmpty(ApiAddress)));

        return false;
    }

    /// <summary>
    /// Update
    /// </summary>
    public void Update(ConfigStoreModel conf)
    {
        AccessToken = conf.AccessToken;
        LocalDirectory = conf.LocalDirectory;   
        RemoteDirectory = conf.RemoteDirectory;
        ApiAddress = conf.ApiAddress;
    }
}