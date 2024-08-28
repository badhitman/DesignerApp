////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib.Components.Shared.tabs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

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
    /// CurrentTab
    /// </summary>
    public TabComponent? CurrentTab { get; private set; }

    /// <summary>
    /// TabsComponents
    /// </summary>
    public List<TTabOfDocumenBaseComponent> TabsComponents { get; set; } = [];

    /// <inheritdoc/>
    public override bool IsEdited => TabsComponents.Any(x => x.IsEdited);

    /// <summary>
    /// Признак того что обработку бизнес логики следует принудительно отменить/пропустить
    /// </summary>
    /// <remarks>в наследниках после вызова текущей реализации должен проверить этот признак и отказаться от дальнейшего выполнения</remarks>
    protected bool IsCancel;

    /// <summary>
    /// Tab change event
    /// </summary>
    public abstract void TabChangeEvent(TTabOfDocumenBaseComponent tab_sender);

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
    }

    /// <inheritdoc/>
    public void TabTryChange(TabComponent sender)
    {
        if (DocumentKey.HasValue && DocumentKey.Value > 0 && IsEdited)
        {
            sender.HoldTab = true;
            SnackbarRepo.Add("Существуют не сохранённые изменения. Сохраните форму!", Severity.Info);
            return;
        }
        CurrentTab = sender;
    }
}