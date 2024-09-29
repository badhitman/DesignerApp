using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Newtonsoft.Json;
using SharedLib;

namespace ApiRestService;

/// <inheritdoc/>
public class LoggerActionFilter : IActionFilter
{
    readonly ILogger _logger;
    ReadOnlyDictionary<string, object?>? actionArguments;
    string project_name;
    /// <inheritdoc/>
    public LoggerActionFilter(ILoggerFactory loggerFactory)
    {
        project_name ??= Assembly.GetCallingAssembly().GetName().Name ?? "asp_action";
        _logger = loggerFactory.CreateLogger(project_name);
    }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        ControllerActionDescriptor? cad = context.ActionDescriptor as ControllerActionDescriptor;

        // Пропускаем логирование для отмеченных Nolog
        if (context.Controller.GetType().GetTypeInfo().IsDefined(typeof(LoggerNologAttribute)) || cad?.MethodInfo.IsDefined(typeof(LoggerNologAttribute)) == true)
            return;

        string pars = string.Empty;

        // Собираем параметры, если они еще не готовы
        if (string.IsNullOrWhiteSpace(pars) && actionArguments != null && cad != null)
        {
            List<string> fs = cad.Parameters.OfType<ControllerParameterDescriptor>()
                .Where(x => x.ParameterInfo.GetCustomAttribute<FromServicesAttribute>() != null)
                .Select(x => x.Name).ToList();

            Dictionary<string, object?> pf = actionArguments.Where(x => x.Value is not CancellationToken && !fs.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            pars = JsonConvert.SerializeObject(pf, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings);
        }

        string msg = $"{context.HttpContext.Request.Path}|{pars}";

        if (cad?.MethodInfo.IsDefined(typeof(LoggerLogResultMessageAttribute)) == true && context.Result is ResponseBaseModel && (context.Result as ResponseBaseModel)?.Messages.Any() == true)
            msg += "|" + string.Join(';', (context.Result as ResponseBaseModel)!.Messages);

        if (context.Exception == null || context.ExceptionHandled)
            _logger.LogInformation("{msg}", msg);
        else
            _logger.LogError("{msg}|{ex}", msg, context.Exception.Message.Replace('\n', ' '));
    }

    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller.GetType().GetTypeInfo().IsDefined(typeof(LoggerNologAttribute)) || (context.ActionDescriptor is ControllerActionDescriptor cad && cad.MethodInfo.IsDefined(typeof(LoggerNologAttribute))))
            return;

        actionArguments = new ReadOnlyDictionary<string, object?>(context.ActionArguments.Where(x => x.Value is not CancellationToken).ToDictionary(x => x.Key, x => x.Value));
        //actionArguments = new ReadOnlyDictionary<string, object?>(context.ActionArguments.Where(x => x.Value is not CancellationToken));
    }
}