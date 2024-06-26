////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Инициатор имени хоста.
/// Автоматически создаёт уникальное (в контексте сессии опроса) имя хоста
/// </summary>
public class HostTextFieldVA : TextFieldValueAgent
{
    /// <inheritdoc/>
    public override string About => "Установка значения по умолчанию для <code>поля</code> имени хоста. Генерируемое имя уникальное в таблице (по колонке/полю)";

    /// <inheritdoc/>
    public override string Name => "Генерация имени хоста";

    static readonly Random _rnd = new();

    /// <inheritdoc/>
    public override string? DefaultValueIfNull(ConstructorFieldFormModelDB field, ConstructorFormSessionModelDB session_Questionnaire, int page_join_form_id)
    {
        int _index_value = _rnd.Next(1000, 9999);
        while (session_Questionnaire.SessionValues?.Any(x => x.Name.Equals(field.Name) && x.Value?.Equals($"{_index_value}.host", StringComparison.OrdinalIgnoreCase) == true) == true)
            _rnd.Next(1000, 9999);
        return $"{_index_value}.host";
    }
}