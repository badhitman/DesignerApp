////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Агент обработки поведения текстового поля
/// </summary>
public abstract class TextFieldValueAgent : DeclarationAbstraction
{
    /// <summary>
    /// Автоматическая установка значения (если значение NULL)
    /// </summary>
    public abstract string? DefaultValueIfNull(FieldFormConstructorModelDB field, SessionOfDocumentDataModelDB session_Document, int page_join_form_id);
}