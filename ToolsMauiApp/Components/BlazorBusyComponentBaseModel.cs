////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ToolsMauiApp;

/// <summary>
/// Базовый компонент с поддержкой состояния "занят". Компоненты, которые выполняют запросы
/// на время обработки переходят в состояние "IsBusyProgress" с целью обеспечения визуализации смены этого изменения
/// </summary>
public abstract class BlazorBusyComponentBaseModel : ComponentBase, IDisposable
{
    /// <summary>
    /// Snackbar
    /// </summary>
    [Inject]
    public ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Компонент занят отправкой REST запроса и обработки ответа
    /// </summary>
    public bool IsBusyProgress { get; set; }

    /// <summary>
    /// SetBusy
    /// </summary>
    public async Task SetBusy(bool is_busy = true, CancellationToken token = default)
    {
        IsBusyProgress = is_busy;
        await Task.Delay(1, token);
        StateHasChanged();
    }

    /// <summary>
    /// Уведомляет компонент об изменении его состояния.
    /// Когда применимо, это вызовет повторную визуализацию компонента.
    /// </summary>
    public virtual void StateHasChangedCall() => StateHasChanged();

    /// <summary>
    /// Signals to a System.Threading.CancellationToken that it should be canceled.
    /// </summary>
    protected CancellationTokenSource? _cts;
    /// <summary>
    /// Propagates notification that operations should be canceled.
    /// </summary>
    protected CancellationToken CancellationToken => (_cts ??= new()).Token;

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _cts?.Cancel();
        _cts?.Dispose();
    }
}