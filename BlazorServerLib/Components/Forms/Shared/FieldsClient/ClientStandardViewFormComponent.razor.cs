////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsClient;

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

    /// <summary>
    /// Номер строки таблицы данных (0 - если форма обычная, а не не таблица/многострочная)
    /// </summary>
    [CascadingParameter]
    public uint RowNum { get; set; } = 0;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    /// <summary>
    /// Доступ к перечню полей формы. Каждое поле формы добавляет себя к этому перечню при инициализации (в <c>OnInitialized()</c>) базового <cref name="FieldBaseClientComponent">компонента</cref>
    /// </summary>
    protected List<FieldComponentBaseModel?> FieldsReferring = [];

    /// <inheritdoc/>
    protected IEnumerable<EntryNestedModel> Directories = default!;

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