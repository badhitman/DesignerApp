////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BlazorWebLib;

/// <summary>
/// Document edit
/// </summary>
public partial class DocumentEditBaseComponent : DocumenBodyBaseComponent
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

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Uri current_uri = new(NavigationRepo.Uri);
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
            IsCancel = true;
            NavigationRepo.NavigateTo($"{current_uri.AbsolutePath}?{nameof(TabName)}={DocumentMetadata.Tabs.First().SystemName}", true);
        }
    }
}
