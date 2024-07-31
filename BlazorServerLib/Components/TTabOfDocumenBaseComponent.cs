////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace BlazorWebLib.Components;

/// <summary>
/// TabOfDocumentComponent
/// </summary>
public abstract class TTabOfDocumenBaseComponent : DocumenBodyBaseComponent
{
    /// <summary>
    /// Формы в табе
    /// </summary>
    public List<FormBaseModel> FormsStack { get; set; } = [];
}