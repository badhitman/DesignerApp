////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using HtmlGenerator.html5;
using SharedLib;

namespace CodegeneratorLib;

/// <summary>
/// Blazor code-generator
/// </summary>
public partial class BlazorCodeGenerator
{
    /// <summary>
    /// CodeGeneratorConfigModel
    /// </summary>
    public required CodeGeneratorConfigModel Config { get; set; }

    /// <summary>
    /// Get BlazorComponent: view part
    /// </summary>
    public virtual string GetView()
    {
        return $"@inherits BlazorBusyComponentBaseModel" +
            $"{Environment.NewLine}" +
            $"{string.Join(Environment.NewLine, DomElements.Select(x => x.GetHTML()))}";
    }

    /// <summary>
    /// Get BlazorComponent: code part
    /// </summary>
    public virtual string GetCode(bool wrap_code)
    {
        string _raw = $"{base_dom_root.TabString}// TODO: xxx";

        if (wrap_code)
            return $"@code {{{Environment.NewLine}" +
                $"{_raw}" +
                $"}}";
        else
        {
            string c_ns = string.IsNullOrWhiteSpace(ComponentDestination) ? "no_set_namespace" : NamespaceAttribute.NormalizeNameSpice(ComponentDestination, ".");
            while (c_ns.EndsWith('.'))
                c_ns = c_ns[..^1];
            while (c_ns.StartsWith('.'))
                c_ns = c_ns[1..];

            return
                $"////////////////////////////////////////////////{Environment.NewLine}" +
                $"// © https://github.com/badhitman - @fakegov{Environment.NewLine}" +
                $"////////////////////////////////////////////////{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"{c_ns}{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"/// <summary>{Environment.NewLine}" +
                $"/// {ComponentDescription}{Environment.NewLine}" +
                $"/// </summary>{Environment.NewLine}" +
                $"public partial class {ComponentName} : BlazorLib.BlazorBusyComponentBaseModel{Environment.NewLine}" +
                $"{{{Environment.NewLine}" +
                 $"{_raw}{Environment.NewLine}" +
                $"}}";
        }
    }

    /// <summary>
    /// Имя компонента
    /// </summary>
    public virtual string? ComponentName { get; set; }

    /// <summary>
    /// Описание компонента
    /// </summary>
    public virtual string? ComponentDescription { get; set; }

    /// <summary>
    /// Расположение компонента
    /// </summary>
    public virtual string? ComponentDestination { get; set; }

    /// <summary>
    /// Методы компонента
    /// </summary>
    public virtual List<BlazorMethodBuilder> Methods { get; set; } = [];

    /// <summary>
    /// Dom элементы
    /// </summary>
    public List<safe_base_dom_root> DomElements { get; set; } = [];
}