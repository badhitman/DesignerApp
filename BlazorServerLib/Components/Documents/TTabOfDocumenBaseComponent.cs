////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib;

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