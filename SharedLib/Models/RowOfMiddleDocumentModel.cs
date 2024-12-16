////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// RowOfMiddleDocumentModel
/// </summary>
[Index(nameof(Quantity))]
public abstract class RowOfMiddleDocumentModel : RowOfBaseDocumentModel
{
    /// <summary>
    /// Количество
    /// </summary>
    public decimal Quantity { get; set; }
}