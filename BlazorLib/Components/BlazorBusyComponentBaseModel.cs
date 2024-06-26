////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;

namespace BlazorLib;

/// <summary>
/// Базовый компонент с поддержкой состояния "занят". Компоненты, которые выполняют запросы
/// на время обработки переходят в состояние "IsBusyProgress" с целью обеспечения визуализации смены этого изменения
/// </summary>
public abstract class BlazorBusyComponentBaseModel : ComponentBase, IDisposable
{
    /// <summary>
    /// Компонент занят отправкой REST запроса и обработки ответа
    /// </summary>
    public bool IsBusyProgress { get; set; }

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