////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public interface IToolsAppManager
{
    /// <inheritdoc/>
    public Task<ApiRestConfigModelDB[]> GetAllConfigurations();
}