////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Базовая модель ответа/результата на запрос
/// </summary>
public class ResponseBaseModel
{
    /// <summary>
    /// Сообщения, которые сервер присоединяет к ответу
    /// </summary>
    public List<ResultMessage> Messages { get; set; } = [];

    /// <summary>
    /// Результат обработки запроса.
    /// True - если удачно без ошибок. False  - если возникла ошибка
    /// </summary>
    public virtual bool Success() => !Messages.Any(x => x.TypeMessage == ResultTypesEnum.Error);

    /// <summary>
    /// Сообщение сервера. Если IsSuccess == false, то будет сообщение об ошибке
    /// </summary>
    public string Message()
    {
        Messages ??= [];
        return Messages.Count != 0 ? $"{string.Join($",{Environment.NewLine}", Messages.Select(x => $"[{x}]"))};" : string.Empty;
    }

    /// <summary>
    /// Сообщение со статусными сообщениями
    /// </summary>
    public static ResponseBaseModel Create(IEnumerable<ResultMessage> messages) => new() { Messages = new List<ResultMessage>(messages) };



    /// <summary>
    /// Приведение к текущему типу из Exception
    /// </summary>
    /// <param name="ex">Исходный объект для преобразования в текущий</param>
    public static implicit operator ResponseBaseModel(Exception ex) => CreateError(ex);

    /// <inheritdoc/>
    public static ResponseBaseModel CreateWarning(string msg) => new() { Messages = [new() { TypeMessage = ResultTypesEnum.Warning, Text = msg }] };

    /// <inheritdoc/>
    public static ResponseBaseModel CreateError(string msg) => new() { Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = msg }] };
    /// <inheritdoc/>
    public static List<ResultMessage> ErrorMessage(string msg) => [new ResultMessage() { TypeMessage = ResultTypesEnum.Error, Text = msg }];

    /// <inheritdoc/>
    public static ResponseBaseModel CreateError(Exception ex)
    {
        ResponseBaseModel err = new();
        err.Messages.InjectException(ex);
        return err;
    }


    /// <inheritdoc/>
    public static ResponseBaseModel CreateError(List<ValidationResult> validationResults)
    {
        ResponseBaseModel err = new();
        err.Messages.InjectException(validationResults);
        return err;
    }

    /// <inheritdoc/>
    public static ResponseBaseModel CreateInfo(string msg) => new() { Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Info, Text = msg }] };

    /// <inheritdoc/>
    public static ResponseBaseModel CreateSuccess(string msg) => new() { Messages = [new ResultMessage() { TypeMessage = ResultTypesEnum.Success, Text = msg }] };
    /// <inheritdoc/>
    public static List<ResultMessage> SuccessMessage(string msg) => [new ResultMessage() { TypeMessage = ResultTypesEnum.Success, Text = msg }];

    /// <inheritdoc/>
    public static ResponseBaseModel CreateSuccess(IEnumerable<string> messages) => new() { Messages = messages.Select(msg => new ResultMessage() { TypeMessage = ResultTypesEnum.Success, Text = msg }).ToList() }; //[new ResultMessage() { TypeMessage = ResultTypesEnum.Success, Text = msg }] };

    /// <summary>
    /// Добавить сообщение об успешном выполнении операции
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <returns>Текущий объект, но с уже добавленным сообщением</returns>
    public ResponseBaseModel AddSuccess(string text) => AddMessage(ResultTypesEnum.Success, text);
    /// <summary>
    /// Добавить сообщения об успешном выполнении операции
    /// </summary>
    /// <param name="messages">Тексты сообщений</param>
    /// <returns>Текущий объект, но с уже добавленными сообщениями</returns>
    public ResponseBaseModel AddSuccess(IEnumerable<string> messages)
    {
        foreach (string text in messages)
            AddMessage(ResultTypesEnum.Success, text);
        return this;
    }

    /// <summary>
    /// Добавить информационное сообщение к результату запроса
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <returns>Текущий объект, но с уже добавленным сообщением</returns>
    public ResponseBaseModel AddInfo(string text) => AddMessage(ResultTypesEnum.Info, text);

    /// <summary>
    /// Добавить сообщение об ошибке к результату запроса
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <returns>Текущий объект, но с уже добавленным сообщением</returns>
    public ResponseBaseModel AddError(string text) => AddMessage(ResultTypesEnum.Error, text);

    /// <summary>
    /// Добавить сообщение с уведомлением к результату запроса
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <returns>Текущий объект, но с уже добавленным сообщением</returns>
    public ResponseBaseModel AddAlert(string text) => AddMessage(ResultTypesEnum.Alert, text);

    /// <summary>
    /// Добавить сообщение с важным сообщением к результату запроса
    /// </summary>
    /// <param name="text">Текст сообщения</param>
    /// <returns>Текущий объект, но с уже добавленным сообщением</returns>
    public ResponseBaseModel AddWarning(string text) => AddMessage(ResultTypesEnum.Warning, text);

    /// <summary>
    /// Добавить сообщение
    /// </summary>
    /// <param name="type">Тип сообщения</param>
    /// <param name="text">Текст сообщения</param>
    /// <returns>Текущий объект, но с уже добавленным сообщением</returns>
    public ResponseBaseModel AddMessage(ResultTypesEnum type, string text)
    {
        Messages ??= [];
        Messages.Add(new ResultMessage() { TypeMessage = type, Text = text });
        return this;
    }

    /// <summary>
    /// Добавить сообщения порцией
    /// </summary>
    /// <param name="messages">Перечень сообщений для добавления к ответу</param>
    public ResponseBaseModel AddRangeMessages(IEnumerable<ResultMessage> messages)
    {
        if (!messages.Any())
            return this;

        Messages.AddRange(messages);

        return this;
    }
}