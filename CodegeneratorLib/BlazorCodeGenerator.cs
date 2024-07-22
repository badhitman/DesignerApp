////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using HtmlGenerator.html5;

namespace CodegeneratorLib;

/// <summary>
/// Blazor code-generator
/// </summary>
public class BlazorCodeGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public required safe_base_dom_root DomTree {  get; set; }

    /// <summary>
    /// Методы компонента
    /// </summary>
    public List<BlazorMethodBuilder> Methods = [];
}