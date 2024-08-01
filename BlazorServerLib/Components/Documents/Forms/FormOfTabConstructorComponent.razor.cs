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

    bool BoolSimpleValue(FieldFitModel field)
    {
        return false;
    }

    void SetSimpleFieldBoolValue(FieldFitModel field, bool value)
    {
        
    }

    int IntSimpleValue(FieldFitModel field)
    {
        return 0;
    }

    void SetSimpleFieldIntValue(FieldFitModel field, int value)
    {

    }

    double DoubleSimpleValue(FieldFitModel field)
    {
        return 0;
    }

    void SetSimpleFieldDoubleValue(FieldFitModel field, double value)
    {

    }

    string GetFieldDomId(BaseRequiredFormFitModel bf)
    {
        return "";
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