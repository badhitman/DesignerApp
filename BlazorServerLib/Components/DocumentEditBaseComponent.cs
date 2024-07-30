////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using Microsoft.Extensions.Logging;

namespace BlazorWebLib.Components;

/// <summary>
/// Document edit
/// </summary>
public partial class DocumentEditBaseComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IJournalUniversalService JournalRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected NavigationManager NavigationRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ILogger<DocumentEditBaseComponent> LoggerRepo { get; set; } = default!;

    /// <summary>
    /// Тип документа
    /// </summary>
    [Parameter, EditorRequired]
    public required string DocumentNameOrId { get; set; }

    /// <summary>
    /// Document Key - идентификатор документа из БД. если меньше 1, то создание нового
    /// </summary>
    [Parameter, EditorRequired]
    public required int DocumentKey { get; set; }

    /// <summary>
    /// Tab name
    /// </summary>
    [Parameter, SupplyParameterFromQuery]
    public string? TabName { get; set; }


    /// <inheritdoc/>
    protected DocumentFitModel DocumentMetadata { get; set; } = default!;

    /// <inheritdoc/>
    protected bool _is_return;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Uri current_uri = new(NavigationRepo.Uri);
        IsBusyProgress = true;
        DocumentMetadata = await JournalRepo.GetDocumentMetadata(DocumentNameOrId);
        IsBusyProgress = false;

        if (DocumentMetadata.Tabs.Length == 0)
        {
            _is_return = true;
            return;
        }
        if (!DocumentMetadata.Tabs.Any(x => x.SystemName == TabName))
        {
            _is_return = true;
            NavigationRepo.NavigateTo($"{current_uri.AbsolutePath}?{nameof(TabName)}={DocumentMetadata.Tabs.First().SystemName}", true);
        }
    }
}
