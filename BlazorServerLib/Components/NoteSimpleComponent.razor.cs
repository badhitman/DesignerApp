////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components;

public partial class NoteSimpleComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Label
    /// </summary>
    [Parameter, EditorRequired]
    public required string Label { get; set; }

    /// <summary>
    /// Hint
    /// </summary>
    [Parameter, EditorRequired]
    public required string Hint { get; set; }

    /// <summary>
    /// Имя приложения, которое обращается к службе облачного хранения параметров
    /// </summary>
    [Parameter, EditorRequired]
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Имя параметра
    /// </summary>
    [Parameter, EditorRequired]
    public required string Name { get; set; }

    /// <summary>
    /// Тип сериализуемого параметра
    /// </summary>
    [Parameter, EditorRequired]
    public required string TypeName { get; set; }

    /// <summary>
    /// Префикс имени (опционально)
    /// </summary>
    [Parameter]
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Связанный PK строки базы данных (опционально)
    /// </summary>
    [Parameter]
    public int? OwnerPrimaryKey { get; set; }


    StorageCloudParameterModel KeyStorage => new StorageCloudParameterModel()
    {
        ApplicationName = ApplicationName,
        Name = Name,
        OwnerPrimaryKey = OwnerPrimaryKey,
        PrefixPropertyName = PrefixPropertyName,
    };

    string domId = default!;

    string? initValue;
    string? editValue;

    async Task SaveText()
    {
        IsBusyProgress = true;
        TResponseModel<int> rest = await StorageRepo.SaveParameter(editValue, KeyStorage);
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        IsBusyProgress = false;
        initValue = editValue;
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        domId = $"{OwnerPrimaryKey}/{nameof(NoteSimpleComponent)}{ApplicationName}{Name}{TypeName}{PrefixPropertyName}";
        IsBusyProgress = true;
        TResponseModel<string?> rest = await StorageRepo.ReadParameter<string>(KeyStorage);
        IsBusyProgress = false;
        initValue = rest.Response;
        editValue = initValue;
    }
}