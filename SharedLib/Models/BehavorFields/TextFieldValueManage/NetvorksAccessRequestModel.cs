namespace SharedLib;

/// <summary>
/// Запрос списка от генератора
/// </summary>
public class NetvorksAccessRequestModel
{
    /// <summary>
    /// Имя вкладки 'OCP'
    /// </summary>
    public string PageNameOCP { get; set; } = default!;

    /// <summary>
    /// Имя колонки 'Среда' на вкладке 'OCP'
    /// </summary>
    public string FieldRuntimeNameOCP { get; set; } = default!;

    /// <summary>
    /// Имя вкладки 'ВМ'
    /// </summary>
    public string PageNameVM { get; set; } = default!;

    /// <summary>
    /// Имя колонки 'Среда' на вкладке 'ВМ'
    /// </summary>
    public string FieldRuntimeNameVM { get; set; } = default!;

    /// <summary>
    /// Имя колонки 'Роль' на вкладке 'ВМ'
    /// </summary>
    public string FieldRoleNameVM { get; set; } = default!;

    /// <summary>
    /// Имя колонки 'Техническое имя' на вкладке 'ВМ'
    /// </summary>
    public string FieldTechNameVM { get; set; } = default!;
}