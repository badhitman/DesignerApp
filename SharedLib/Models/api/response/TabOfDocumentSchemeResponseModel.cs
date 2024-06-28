////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// таб/вкладка схемы документа
/// </summary>
public class TabOfDocumentSchemeResponseModel : ResponseBaseModel
{
    /// <summary>
    /// таб/вкладка схемы документа
    /// </summary>
    public TabOfDocumentSchemeConstructorModelDB? TabOfDocumentScheme { get; set; }
}