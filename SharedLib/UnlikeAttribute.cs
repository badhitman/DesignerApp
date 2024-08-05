////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedLib;

/// <summary>
/// Атрибут поля "не-равенство"
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class UnlikeAttribute : ValidationAttribute
{
    private string DependentProperty { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dependentProperty">Имя зависимого свойства</param>
    /// <exception cref="ArgumentNullException">Исключение, возникающее при передаче NULL в имени зависимого свойства.</exception>
    public UnlikeAttribute(string dependentProperty)
    {
        if (string.IsNullOrEmpty(dependentProperty))
        {
            throw new ArgumentNullException(nameof(dependentProperty));
        }
        DependentProperty = dependentProperty;
    }

    /// <summary>
    /// Определяет, допустимо ли указанное значение объекта.
    /// </summary>
    /// <param name="value">Значение для проверки</param>
    /// <param name="validationContext">Контекст проверки значения</param>
    /// <returns>Представляет контейнер для результатов запроса проверки.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null)
        {
            PropertyInfo? otherProperty = validationContext.ObjectInstance.GetType().GetProperty(DependentProperty);
            object? otherPropertyValue = otherProperty?.GetValue(validationContext.ObjectInstance, null);
            if (value.Equals(otherPropertyValue))
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        return ValidationResult.Success;
    }

    //private void MergeAttribute(IDictionary<string, string> attributes,
    //    string key,
    //    string value)
    //{
    //    if (attributes.ContainsKey(key))
    //    {
    //        return;
    //    }
    //    attributes.Add(key, value);
    //}
}