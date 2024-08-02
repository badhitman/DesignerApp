////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib;

/// <summary>
/// Document edit
/// </summary>
public abstract class DocumentEditBaseComponent : DocumenBodyBaseComponent
{
    /// <inheritdoc/>
    [Inject]
    protected NavigationManager NavigationRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ILogger<DocumentEditBaseComponent> LoggerRepo { get; set; } = default!;

    /// <summary>
    /// Тип документа
    /// </summary>
    [Parameter, EditorRequired]
    public required string DocumentNameOrId { get; set; }

    /// <summary>
    /// Tab name
    /// </summary>
    [Parameter, SupplyParameterFromQuery]
    public string? TabName { get; set; }


    /// <summary>
    /// Признак того что обработку бизнес логики следует принудительно отменить/пропустить
    /// </summary>
    /// <remarks>в наследниках после вызова текущей реализации должен проверить этот признак и отказаться от дальнейшего выполнения</remarks>
    protected bool IsCancel;

    /// <summary>
    /// Tab change event
    /// </summary>
    public abstract Task TabChangeEvent(TTabOfDocumenBaseComponent tab_sender);

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {

        IsBusyProgress = true;
        DocumentMetadata = await JournalRepo.GetDocumentMetadata(DocumentNameOrId);
        IsBusyProgress = false;

        if (DocumentMetadata.Tabs.Count == 0)
        {
            IsCancel = true;
            return;
        }
        if (!DocumentMetadata.Tabs.Any(x => x.SystemName == TabName))
        {
            string first_tab_system_name = DocumentMetadata.Tabs.First().SystemName;
            Uri uriBuilder = new(NavigationRepo.Uri);
            uriBuilder = new(uriBuilder.AppendQueryParameter(nameof(TabName), first_tab_system_name));

            IsCancel = true;
            NavigationRepo.NavigateTo($"{uriBuilder}", true);
        }
    }
}
