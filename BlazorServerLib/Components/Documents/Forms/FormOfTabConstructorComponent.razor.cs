////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Documents.Forms;

/// <summary>
/// Form of tab
/// </summary>
public partial class FormOfTabConstructorComponent : FormBaseModel
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required IEnumerable<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }

    ValueDataForSessionOfDocumentModelDB[] _selfSessionValues = [];

    /// <inheritdoc/>
    public override bool IsEdited => SessionValues.Any(x => _selfSessionValues!.Any(y => x == y)) || _selfSessionValues.Any(x => !SessionValues.Any(y => x == y));

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
        SessionValues.ToList().CopyTo(_selfSessionValues);

        //IsBusyProgress = false;
        //var res = await JournalRepo.
        //IsBusyProgress = false;
        await base.OnInitializedAsync();
    }
}