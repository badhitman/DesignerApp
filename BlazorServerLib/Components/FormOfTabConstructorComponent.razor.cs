////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// Form of tab
/// </summary>
public partial class FormOfTabConstructorComponent : FormBaseModel
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ValueDataForSessionOfDocumentModelDB[] SessionValues { get; set; }

    /// <inheritdoc/>
    public override bool IsEdited => false;

    /// <inheritdoc/>
    public override Task SaveForm()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task ResetForm()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        //IsBusyProgress = false;
        //var res = await JournalRepo.
        //IsBusyProgress = false;
        await base.OnInitializedAsync();
    }
}