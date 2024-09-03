////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace ApiRestService;

/// <summary>
/// Включает логирование для метода. Для класса контроллера включено всегда.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LoggerLogAttribute : Attribute { }

/// <summary>
/// Выключает логирования для класса контроллера или метода
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LoggerNologAttribute : Attribute { }

/// <summary>
/// Добавляет текст результата
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LoggerLogResultMessageAttribute : Attribute { }