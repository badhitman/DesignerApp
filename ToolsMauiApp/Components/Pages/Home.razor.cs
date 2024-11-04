////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace ToolsMauiApp.Components.Pages;

/// <summary>
/// Home
/// </summary>
public partial class Home : BlazorBusyComponentBaseModel
{
    ConfigStoreModel configEdit = new();

    bool CanSave => configEdit.FullSets && !configEdit.Equals(MauiProgram.ConfigStore.Response);

    async Task SaveConfig()
    {
        await SetBusy();
        await MauiProgram.SaveConfig(configEdit);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(MauiProgram.ConfigStore.Messages);
    }

    async Task TestConnect()
    {
        if (string.IsNullOrWhiteSpace(configEdit.LocalDirectory))
        {
            SnackbarRepo.Error("Не установлена локальная директория");
            return;
        }

        FileInfo _fi = new(configEdit.LocalDirectory);
        if (!_fi.Exists)
        {
            SnackbarRepo.Error("Локальная директория отсутствует");
            return;
        }
        await SetBusy();

        await SetBusy(false);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (MauiProgram.ConfigStore.Response is null)
            return;

        configEdit = GlobalTools.CreateDeepCopy(MauiProgram.ConfigStore.Response)!;
    }
}