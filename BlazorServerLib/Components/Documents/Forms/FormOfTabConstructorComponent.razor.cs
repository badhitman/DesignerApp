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

    #region bool
    bool BoolSimpleValue(FieldFitModel field)
    {
        return default;
    }

    void SetSimpleFieldBoolValue(FieldFitModel field, bool value)
    {

    }
    #endregion

    #region int
    int IntSimpleValue(FieldFitModel field)
    {
        return default;
    }

    void SetSimpleFieldIntValue(FieldFitModel field, int value)
    {

    }
    #endregion

    #region double
    double DoubleSimpleValue(FieldFitModel field)
    {
        return default;
    }

    void SetSimpleFieldDoubleValue(FieldFitModel field, double value)
    {

    }
    #endregion

    #region DateTime
    DateTime DateTimeSimpleValue(FieldFitModel field)
    {
        return default;
    }

    void SetSimpleFieldDateTimeValue(FieldFitModel field, DateTime? value)
    {

    }
    #endregion

    #region DateOnly
    DateOnly DateOnlySimpleValue(FieldFitModel field)
    {
        return default;
    }

    void SetSimpleFieldDateOnlyValue(FieldFitModel field, DateOnly? value)
    {

    }
    #endregion

    #region TimeOnly
    TimeOnly TimeOnlySimpleValue(FieldFitModel field)
    {
        return default;
    }

    void SetSimpleFieldTimeOnlyValue(FieldFitModel field, TimeOnly? value)
    {

    }
    #endregion

    #region string
    string StringSimpleValue(FieldFitModel field)
    {
        return default!;
    }

    void SetSimpleFieldStringValue(FieldFitModel field, string? value)
    {

    }
    #endregion

    string GetFieldDomId(BaseRequiredFormFitModel bf)
    {
        return $"";
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