using Microsoft.JSInterop;

namespace BlazorLib;

/// <summary>
/// This class provides an example of how JavaScript functionality can be wrapped in a .NET class for easy consumption.
/// The associated JavaScript module is loaded on demand when first needed.
/// This class can be registered as scoped DI service and then injected into Blazor components for use.
/// </summary>
public class ExampleJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask =
        new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorLib/exampleJsInterop.js").AsTask());

    /// <inheritdoc/>
    public async ValueTask<string> Prompt(string message)
    {
        IJSObjectReference module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            IJSObjectReference module = await moduleTask.Value;
            await module.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}