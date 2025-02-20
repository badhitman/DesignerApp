////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;
using ToolsMauiLib;

namespace ToolsMauiApp;

/// <inheritdoc/>
public static class MauiProgram
{
    /// <inheritdoc/>
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        //

        builder.Services.AddDbContextFactory<ToolsAppContext>(opt =>
        {
#if DEBUG
            opt.EnableSensitiveDataLogging(true);
            opt.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
#endif
        });


        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();

        ApiRestConfigModelDB _conf = new() { Name = "" };
        builder.Services.AddSingleton(sp => _conf);
        //builder.Services.AddCascadingValue(sp => _conf);

        builder.Services.AddScoped<ILogsService, LogsService>();
        builder.Services.AddScoped<IToolsAppManager, ToolsAppManager>();

        builder.Services.AddScoped<IClientHTTPRestService, ToolsSystemHTTPRestService>();
        builder.Services.AddScoped<IServerToolsService, ToolsSystemService>();

        builder.Services.AddHttpClient(HttpClientsNamesEnum.Tools.ToString(), cc =>
        {
            //cc.BaseAddress = new Uri(_conf.AddressBaseUri ?? "localhost");
            cc.DefaultRequestHeaders.Add(_conf.HeaderName, _conf.TokenAccess);
        });
        // #if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        // #endif
        return builder.Build();
    }
}