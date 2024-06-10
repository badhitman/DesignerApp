namespace BlazorLib.Components;

/// <summary>
/// DOM
/// </summary>
public interface IDomBaseComponent
{
    /// <summary>
    /// Идентификатор DOM элемента. Уникальность строгая в рамках всей страницы т.к. используется как @key
    /// </summary>
    public string DomID { get; }
}