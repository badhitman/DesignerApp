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
    /// Tab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB Tab { get; set; }

    /// <summary>
    /// Join
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabJoinDocumentSchemeConstructorModelDB Join { get; set; }

    /// <summary>
    /// Form
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }


    /// <inheritdoc/>
    public override bool IsEdited => SessionValues.Any(x => _selfSessionValues!.Any(y => x == y)) || _selfSessionValues.Any(x => !SessionValues.Any(y => x == y));

    #region bool
    bool BoolSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        //var v = SessionValues.Where(x=>x.TabJoinDocumentSchemeId == )
        return default;
    }

    void SetSimpleFieldBoolValue(FieldFitModel field, bool value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region int
    int IntSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        // SessionValues
        return default;
    }

    void SetSimpleFieldIntValue(FieldFitModel field, int value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region double
    double DoubleSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        // SessionValues
        return default;
    }

    void SetSimpleFieldDoubleValue(FieldFitModel field, double value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region DateTime
    DateTime DateTimeSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        // SessionValues
        return default;
    }

    void SetSimpleFieldDateTimeValue(FieldFitModel field, DateTime? value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region DateOnly
    DateOnly DateOnlySimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        // SessionValues
        return default;
    }

    void SetSimpleFieldDateOnlyValue(FieldFitModel field, DateOnly? value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region TimeOnly
    TimeOnly TimeOnlySimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        // SessionValues
        return default;
    }

    void SetSimpleFieldTimeOnlyValue(FieldFitModel field, TimeOnly? value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region string
    string StringSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        // SessionValues
        return default!;
    }

    void SetSimpleFieldStringValue(FieldFitModel field, string? value, FieldFormConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region EntryModel
    EntryModel DictValue(FieldAkaDirectoryFitModel field, FieldFormAkaDirectoryConstructorModelDB e)
    {
        // SessionValues
        return default!;
    }

    void SetDictFieldValue(FieldAkaDirectoryFitModel field, EntryModel? value, FieldFormAkaDirectoryConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion

    #region EntryModel[]
    EntryModel[] DictsValue(FieldAkaDirectoryFitModel field, FieldFormAkaDirectoryConstructorModelDB e)
    {
        // SessionValues
        return default!;
    }

    void SetDictsFieldValue(FieldAkaDirectoryFitModel field, EntryModel[]? value, FieldFormAkaDirectoryConstructorModelDB e)
    {
        // SessionValues
        FormChangeAction(this);
    }
    #endregion



    string GetFieldDomId(BaseRequiredFormFitModel bf)
    {
        return $"{Join.Id}-{bf?.SystemName}";
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