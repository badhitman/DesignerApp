////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Newtonsoft.Json;
using SharedLib;

namespace ToolsMauiApp;

/// <summary>
/// MauiProgram
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// INI file
    /// </summary>
    private static string ConfigFilename = "config.ini";
    /// <summary>
    /// ConfigPath
    /// </summary>
    public static string ConfigPath => Path.Combine(FileSystem.AppDataDirectory, ConfigFilename);

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
    /// ConfigStore
    /// </summary>
    public static TResponseModel<ConfigStoreModel> ConfigStore { get; private set; } = new();

    /// <summary>
    /// CreateMauiApp
    /// </summary>
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
                    ConfigStore.Response = _cs;
            }
            catch (Exception ex)
            {
                ConfigStore.AddError($"Не удалось прочитать/десериализовать файл конфигурации: {_fi.FullName}. Убедитесь, что есть права на доступ");
                ConfigStore.Messages.InjectException(ex);
            }
        }

        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<IToolsSystemService, ToolsSystemService>();

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
}