namespace SharedLib;

/// <summary>
/// Обработчик входящего сообщения
/// </summary>
public interface IResponseReceive<TRequest, TResponse>
{
    /// <summary>
    /// Обработчик ответа на запрос
    /// </summary>
    public Task<TResponseModel<TResponse?>> ResponseHandleAction(TRequest? payload);
}