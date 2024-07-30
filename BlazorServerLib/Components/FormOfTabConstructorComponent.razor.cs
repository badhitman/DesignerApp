////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace BlazorWebLib.Components;

/// <summary>
/// Form of tab
/// </summary>
public partial class FormOfTabConstructorComponent : FormBaseModel
{
    /// <inheritdoc/>
    public override bool IsEdited => true;

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
}