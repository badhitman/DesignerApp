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
    public virtual string GetView(IEnumerable<string>? routesView = null)
    {
        return $"{(routesView?.Any() == true ? $"{string.Join(Environment.NewLine, routesView)}{Environment.NewLine}{Environment.NewLine}" : "")}@inherits BlazorBusyComponentBaseModel" +
            $"{Environment.NewLine}" +
            $"{string.Join(Environment.NewLine, DomElements.Select(x => x.GetHTML()))}";
    }

    /// <summary>
    /// Get BlazorComponent: code part
    /// </summary>
    public virtual string GetCode(bool wrap_code)
    {
        static string convert(ParameterComponentModel parameter)
        {
            string res = $"{base_dom_root.TabString}[{(parameter.IsCascading ? "Cascading" : "")}Parameter";
            if (parameter.IsSupplyParameterFromQuery)
                res += $", SupplyParameterFromQuery";

            res += $"{(parameter.ParameterMode == ParameterModes.Required ? ", RequiredEditor" : "")}]{Environment.NewLine}";
            res += $"{base_dom_root.TabString}public {(parameter.ParameterMode == ParameterModes.Required ? "required " : "")}{parameter.Type}{(parameter.ParameterMode == ParameterModes.Nullable ? "?" : "")} {parameter.Name} {{ get; set; }}";

            return $"{res}{Environment.NewLine}";
        }

        string _raw = Parameters?.Count > 0
            ? $"{string.Join(Environment.NewLine, Parameters.Select(convert))}"
            : "";

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
                $"namespace {c_ns};{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"/// <summary>{Environment.NewLine}" +
                $"/// {ComponentDescription}{Environment.NewLine}" +
                $"/// </summary>{Environment.NewLine}" +
                $"public partial class {ComponentName} : BlazorBusyComponentBaseModel{Environment.NewLine}" +
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
    /// Параметры компонента
    /// </summary>
    public List<ParameterComponentModel>? Parameters { get; set; }

    /// <summary>
    /// Методы компонента
    /// </summary>
    public virtual List<BlazorMethodBuilder> Methods { get; set; } = [];

    /// <summary>
    /// Dom элементы
    /// </summary>
    public List<safe_base_dom_root> DomElements { get; set; } = [];
}