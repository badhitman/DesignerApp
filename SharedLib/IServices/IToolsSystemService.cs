////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// IToolsSystemService
/// </summary>
public interface IToolsSystemService
{
    /// <summary>
    /// GetMe
    /// </summary>
    public Task<TResponseModel<ExpressProfileResponseModel>> GetMe();
}