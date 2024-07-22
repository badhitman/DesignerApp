////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using HtmlGenerator.html5;

namespace CodegeneratorLib;

/// <summary>
/// Blazor code-generator
/// </summary>
public partial class BlazorCodeGenerator
{
    /// <summary>
    /// Get BlazorComponent: view part
    /// </summary>
    public virtual string GetView()
    {
        return $"@inherits BlazorBusyComponentBaseModel" +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"<div class=\"card\">{Environment.NewLine}" +
            $"\t<div class=\"card-body\">{Environment.NewLine}" +
            $"\t\tThis is some text within a card body.{Environment.NewLine}" +
            $"\t</div>{Environment.NewLine}" +
            $"</div>";
    }

    /// <summary>
    /// Get BlazorComponent: code part
    /// </summary>
    public virtual string GetCode()
    {
        return $"@code {{{Environment.NewLine}" +
            $"\t{Environment.NewLine}" +
            $"}};";
    }

    /// <summary>
    /// Методы компонента
    /// </summary>
    public List<BlazorMethodBuilder> Methods { get; set; } = [];

    /// <summary>
    /// Dom элементы
    /// </summary>
    public List<safe_base_dom_root> DomElements { get; set; } = [];
}