////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Helpdesk;

/// <summary>
/// Rubrics Parameters
/// </summary>
public partial class RubricsParametersComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;


    bool _showDisabledRubrics;
    bool ShowDisabledRubrics
    {
        get => _showDisabledRubrics;
        set
        {
            _showDisabledRubrics = value;
            InvokeAsync(ToggleShowingDisabledRubrics);
        }
    }

    ModesSelectRubricsEnum _selectedOption;
    ModesSelectRubricsEnum SelectedOption
    {
        get => _selectedOption;
        set
        {
            _selectedOption = value;
            InvokeAsync(SaveModeSelectingRubrics);
        }
    }

    async Task SaveModeSelectingRubrics()
    {
        IsBusyProgress = true;
        TResponseModel<int> res = await SerializeStorageRepo.SaveParameter<ModesSelectRubricsEnum?>(SelectedOption, GlobalStaticConstants.CloudStorageMetadata.ModeSelectingRubrics);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    async Task ToggleShowingDisabledRubrics()
    {
        IsBusyProgress = true;
        TResponseModel<int> res = await SerializeStorageRepo.SaveParameter<bool?>(ShowDisabledRubrics, GlobalStaticConstants.CloudStorageMetadata.ParameterShowDisabledRubrics);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<bool?> res_ShowDisabledRubrics = await SerializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ParameterShowDisabledRubrics);
        TResponseModel<ModesSelectRubricsEnum?> res_ModeSelectingRubrics = await SerializeStorageRepo.ReadParameter<ModesSelectRubricsEnum?>(GlobalStaticConstants.CloudStorageMetadata.ModeSelectingRubrics);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res_ShowDisabledRubrics.Messages);
        SnackbarRepo.ShowMessagesResponse(res_ModeSelectingRubrics.Messages);
        _showDisabledRubrics = res_ShowDisabledRubrics.Response == true;

        if (res_ModeSelectingRubrics.Response is null || ((int)res_ModeSelectingRubrics.Response) == default)
            res_ModeSelectingRubrics.Response = ModesSelectRubricsEnum.AllowWithoutRubric;

        _selectedOption = res_ModeSelectingRubrics.Response!.Value;
    }
}