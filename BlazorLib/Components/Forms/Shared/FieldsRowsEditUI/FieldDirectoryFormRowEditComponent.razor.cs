using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

public partial class FieldDirectoryFormRowEditComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<FieldDirectoryFormRowEditComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public Action<ConstructorFormDirectoryLinkModelDB> StateHasChangedHandler { get; set; } = default!;

    [Parameter, EditorRequired]
    public ConstructorFormDirectoryLinkModelDB Field { get; set; } = default!;

    protected IEnumerable<EntryModel> Entries = Enumerable.Empty<EntryModel>();

    protected string dom_id => $"{Field.GetType().FullName}_{Field.Id}";

    public int SelectedDirectoryField
    {
        get => Field.DirectoryId;
        private set
        {
            Field.DirectoryId = value;
            if (Field.Directory is not null)
                Field.Directory.Id = value;
            StateHasChangedHandler(Field);
        }
    }

    public void Update(ConstructorFormDirectoryLinkModelDB field)
    {
        Field.Update(field);
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        EntriesResponseModel rest = await _forms.GetDirectories();
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        if (rest.Entries is null)
        {
            _snackbar.Add($"Ошибка {{4BBC7550-5753-4D96-9E17-3E9B21F08493}} rest.Content.Entries is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        Entries = rest.Entries;
        StateHasChanged();
    }
}