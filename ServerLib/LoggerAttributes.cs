namespace ServerLib;

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
/// При логировании заменяет значения длиной более <param>Size</param> на строку со значением длины
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LoggerStripDataAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public LoggerStripDataAttribute(int Size = 500) => this.Size = Size;

    /// <summary>
    /// 
    /// </summary>
    public int Size { get; }
}

/// <summary>
/// Сохраняет разницу json в контексте
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LoggerUseDiffAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public LoggerUseDiffAttribute(string ParamName) => this.ParamName = ParamName;

    /// <summary>
    /// 
    /// </summary>
    public string ParamName { get; }
}

/// <summary>
/// Добавляет текст результата
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class LoggerLogResultMessageAttribute : Attribute { }