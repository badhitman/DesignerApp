using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MudBlazor.Services;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;
using ToolsMauiLib;

namespace ToolsMauiApp;

/// <inheritdoc/>
public static class MauiProgram
{
    /// <summary>
    /// INI file
    /// </summary>
    private static string ConfigFilename = "config.ini";
    /// <summary>
    /// INI file
    /// </summary>
    private static string CommandsFilename = "commands.ini";
    /// <summary>
    /// ConfigPath
    /// </summary>
    public static string ConfigPath => Path.Combine(FileSystem.AppDataDirectory, ConfigFilename);
    /// <summary>
    /// ConfigPath
    /// </summary>
    public static string CommandsPath => Path.Combine(FileSystem.AppDataDirectory, CommandsFilename);

    /// <summary>
    /// ConfigStore
    /// </summary>
    public static TResponseModel<ConfigStoreModel> ConfigStore { get; private set; } = new();

    /// <summary>
    /// Exe Commands
    /// </summary>
    public static TResponseModel<List<ExeCommandModelDB>> ExeCommands { get; private set; } = new();

    /// <inheritdoc/>
    public static MauiApp CreateMauiApp()
    {
        FileInfo _fi = new(ConfigPath);
        if (!_fi.Exists)
        {
            try
            {
                File.WriteAllText(_fi.FullName, JsonConvert.SerializeObject(new ConfigStoreModel() { }));
                ConfigStore.AddInfo($"Создан файл конфигурации: {_fi.FullName}");
            }
            catch (Exception ex)
            {
                ConfigStore.AddError($"Не удалось создать файл конфигурации: {_fi.FullName}. Убедитесь, что есть права на запись");
                ConfigStore.Messages.InjectException(ex);
            }
        }
        else
        {
            try
            {
                ConfigStoreModel? _cs = JsonConvert.DeserializeObject<ConfigStoreModel>(File.ReadAllText(_fi.FullName));
                if (_cs is null)
                {
                    File.WriteAllText(_fi.FullName, JsonConvert.SerializeObject(new ConfigStoreModel() { }));
                    ConfigStore.AddWarning($"Создан новый (перезаписан) файл конфигурации: {_fi.FullName}");
                }
                else
                {
                    ConfigStore.Response = _cs;
                    ConfigStore.AddSuccess($"Прочитан файл конфигурации: {_fi.FullName} (изменён: {_fi.LastWriteTime})");
                }
            }
            catch (Exception ex)
            {
                ConfigStore.AddError($"Не удалось прочитать/десериализовать файл конфигурации: {_fi.FullName}. Убедитесь, что есть права на доступ и формат файла корректный.");
                ConfigStore.Messages.InjectException(ex);
            }
        }

        _fi = new(CommandsPath);
        if (!_fi.Exists)
        {
            try
            {
                File.WriteAllText(_fi.FullName, JsonConvert.SerializeObject(new List<ExeCommandModelDB>() { }));
                ConfigStore.AddInfo($"Создан файл команд: {_fi.FullName}");
            }
            catch (Exception ex)
            {
                ConfigStore.AddError($"Не удалось создать файл команд: {_fi.FullName}. Убедитесь, что есть права на запись");
                ConfigStore.Messages.InjectException(ex);
            }
        }
        else
        {
            try
            {
                List<ExeCommandModelDB>? _cs = JsonConvert.DeserializeObject<List<ExeCommandModelDB>>(File.ReadAllText(_fi.FullName));
                if (_cs is null)
                {
                    File.WriteAllText(_fi.FullName, JsonConvert.SerializeObject(new List<ExeCommandModelDB>() { }));
                    ConfigStore.AddWarning($"Создан новый (перезаписан) файл команд: {_fi.FullName}");
                }
                else
                {
                    ExeCommands.Response = _cs;
                    ExeCommands.AddSuccess($"Прочитан файл команд: {_fi.FullName} (изменён: {_fi.LastWriteTime})");
                }
            }
            catch (Exception ex)
            {
                ExeCommands.AddError($"Не удалось прочитать/десериализовать файл команд: {_fi.FullName}. Убедитесь, что есть права на доступ и формат файла корректный.");
                ExeCommands.Messages.InjectException(ex);
            }
        }

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
        }).UseMauiCommunityToolkit();
        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddScoped<ILogsService, LogsService>();
        builder.Services.AddScoped<IToolsAppManager, ToolsAppManager>();
        
        builder.Services.AddMudServices();
        builder.Services.AddScoped<IClientHTTPRestService, ToolsSystemHTTPRestService>();
        builder.Services.AddScoped<IServerToolsService, ToolsSystemService>();
        //
        if (ConfigStore.Response is not null)
            builder.Services.AddHttpClient(HttpClientsNamesEnum.Tools.ToString(), cc =>
            {
                cc.BaseAddress = new Uri(ConfigStore.Response.ApiAddress ?? "localhost");
                cc.DefaultRequestHeaders.Add("token-access", ConfigStore.Response.AccessToken);
            });
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }


    /// <summary>
    /// SaveConfig
    /// </summary>
    public static async Task SaveConfig(ConfigStoreModel conf)
    {
        ConfigStore.Messages.Clear();
        FileInfo _fi = new(ConfigPath);
        ConfigStore.Response ??= new();
        try
        {
            await File.WriteAllTextAsync(_fi.FullName, JsonConvert.SerializeObject(conf));
            ConfigStore.Response.Update(conf);
            ConfigStore.AddInfo($"Записано: {_fi.FullName}");
        }
        catch (Exception ex)
        {
            ConfigStore.AddError($"Не удалось создать файл конфигурации: {_fi.FullName}. Убедитесь, что есть права на запись");
            ConfigStore.Messages.InjectException(ex);
        }
    }

    /// <summary>
    /// SaveCommands
    /// </summary>
    public static async Task SaveCommands(List<ExeCommandModelDB> commands)
    {
        ExeCommands.Messages.Clear();
        FileInfo _fi = new(CommandsPath);
        ExeCommands.Response ??= [];
        try
        {
            await File.WriteAllTextAsync(_fi.FullName, JsonConvert.SerializeObject(commands));
            ExeCommands.Response = commands;
            ExeCommands.AddInfo($"Записано: {_fi.FullName}");
        }
        catch (Exception ex)
        {
            ExeCommands.AddError($"Не удалось создать файл команд: {_fi.FullName}. Убедитесь, что есть права на запись");
            ExeCommands.Messages.InjectException(ex);
        }
    }
}