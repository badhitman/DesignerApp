using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsClient;

/// <summary>
/// Client standard view form
/// </summary>
public partial class ClientStandardViewFormComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public string? Title { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public Action<bool>? HoldFormHandler { get; set; }

    /// <summary>
    /// Номер строки таблицы данных (0 - если не таблица)
    /// </summary>
    [CascadingParameter]
    public uint? RowNum { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    /// <inheritdoc/>
    protected List<FieldComponentBaseModel?> _fields_refs = [];

    /// <inheritdoc/>
    protected IEnumerable<EntryNestedModel> Directories = default!;

    int BusyEscalationBalance = default;
    /// <inheritdoc/>
    protected void BusyEscalationAction(VerticalDirectionsEnum toggle)
    {
        if (toggle == VerticalDirectionsEnum.Up)
            Interlocked.Increment(ref BusyEscalationBalance);
        else
            Interlocked.Decrement(ref BusyEscalationBalance);

        if (HoldFormHandler is not null)
            HoldFormHandler(!BusyEscalationBalance.Equals(default));
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await Update();
    }

    /// <inheritdoc/>
    public async Task Update(ConstructorFormModelDB? form = null)
    {
        if (form is not null)
            Form.Reload(form);

        if (Form.FormsDirectoriesLinks is not null && Form.FormsDirectoriesLinks.Count != 0)
        {
            IsBusyProgress = true;
            Directories = await FormsRepo.ReadDirectories(Form.FormsDirectoriesLinks.Select(x => x.DirectoryId).Distinct());
            IsBusyProgress = false;
        }

        StateHasChanged();
    }
}