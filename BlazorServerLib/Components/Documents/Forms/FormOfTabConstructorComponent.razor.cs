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
    [CascadingParameter, EditorRequired]
    public required List<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }
    List<ValueDataForSessionOfDocumentModelDB> _selfSessionValues = [];

    /// <summary>
    /// Form
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }


    /// <inheritdoc/>
    public override bool IsEdited => SessionValues.Any(x => _selfSessionValues!.Any(y => x == y)) || _selfSessionValues.Any(x => !SessionValues.Any(y => x == y));

    
    void SetSimpleFieldValue(FieldFitModel field, bool value)
    {
        
    }

    /// <inheritdoc/>
    public override Task SaveForm()
    {
        SessionValues.Clear();
        SessionValues.AddRange(_selfSessionValues);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override void ResetForm()
    {
        _selfSessionValues.Clear();
        _selfSessionValues.AddRange(SessionValues);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        _selfSessionValues.AddRange(SessionValues);

        await base.OnInitializedAsync();
    }
}