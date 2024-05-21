using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace ServerLib;

/// <summary>
/// 
/// </summary>
public class UnhandledExceptionAttribute(ILogger<UnhandledExceptionAttribute> logger) : ExceptionFilterAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        logger.LogError(context.Exception, nameof(UnhandledExceptionAttribute));
        context.Result ??= new OkObjectResult((ResponseBaseModel)context.Exception);
        context.ExceptionHandled = true;
        await base.OnExceptionAsync(context);
    }
}