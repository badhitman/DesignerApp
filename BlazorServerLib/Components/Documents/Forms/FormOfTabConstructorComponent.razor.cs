////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.Documents.Forms;

/// <summary>
/// Form of tab
/// </summary>
public partial class FormOfTabConstructorComponent : FormBaseModel
{
    [Inject]
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter]
    public List<ValueDataForSessionOfDocumentModelDB>? SessionValues { get; set; }
    List<ValueDataForSessionOfDocumentModelDB>? _selfSessionValues;

    /// <summary>
    /// PK строки БД
    /// </summary>
    [CascadingParameter]
    public SessionOfDocumentDataModelDB? Session { get; set; }

    /// <summary>
    /// Tab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB Tab { get; set; }

    /// <summary>
    /// Join
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormToTabJoinConstructorModelDB Join { get; set; }


    void SetSimpleFieldValue(FieldFitModel field, string? value, FieldFormConstructorModelDB e)
    {
        if (Session is null || SessionValues is null)
            throw new Exception();

        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is null)
        {
            _value_field = new()
            {
                Name = e.Name,
                Value = value,
                RowNum = 0,
                OwnerId = Session.Id,
                JoinFormToTabId = Join.Id
            };
            SessionValues.Add(_value_field);
        }
        else
            _value_field.Value = value;

        FormChangeAction(this);
    }


    bool BoolSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        return _value_field?.Value?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
    }

    int IntSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is not null && int.TryParse(_value_field.Value, out int _d))
            return _d;

        return 0;
    }

    double DoubleSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is not null && double.TryParse(_value_field.Value, out double _d))
            return _d;

        return 0;
    }

    DateTime? DateTimeSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is not null && DateTime.TryParse(_value_field.Value, out DateTime _d))
            return _d;

        return default;
    }

    DateOnly? DateOnlySimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is not null && DateOnly.TryParse(_value_field.Value, out DateOnly _d))
            return _d;

        return default;
    }

    TimeOnly? TimeOnlySimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is not null && TimeOnly.TryParse(_value_field.Value, out TimeOnly _d))
            return _d;

        return default;
    }

    string? StringSimpleValue(FieldFitModel field, FieldFormConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        return _value_field?.Value;
    }


    #region directory
    #region single
    int DictValue(FieldAkaDirectoryFitModel field, FieldFormAkaDirectoryConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is not null && int.TryParse(_value_field.Value, out int _d))
            return _d;

        return 0;
    }

    void SetDirectoryFieldValue(FieldAkaDirectoryFitModel field, int? value, FieldFormAkaDirectoryConstructorModelDB e)
    {
        if (Session is null || SessionValues is null)
            throw new Exception();

        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is null)
        {
            _value_field = new()
            {
                Name = e.Name,
                Value = value.ToString(),
                RowNum = 0,
                OwnerId = Session.Id,
                JoinFormToTabId = Join.Id
            };
            SessionValues.Add(_value_field);
        }
        else
            _value_field.Value = value.ToString();

        FormChangeAction(this);
    }
    #endregion

    #region multiselect
    int[] DictsValue(FieldAkaDirectoryFitModel field, FieldFormAkaDirectoryConstructorModelDB e)
    {
        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues?
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (!string.IsNullOrWhiteSpace(_value_field?.Value) && _value_field.Value.Trim().StartsWith('[') && _value_field.Value.Trim().EndsWith(']'))
        {
            try
            {
                return JsonConvert.DeserializeObject<int[]>(_value_field.Value) ?? throw new Exception();
            }
            catch
            {
                return [];
            }
        }

        return [];
    }

    void SetSimpleFieldValue(FieldAkaDirectoryFitModel field, int[]? value, FieldFormAkaDirectoryConstructorModelDB e)
    {
        if (Session is null || SessionValues is null)
            throw new Exception();

        ValueDataForSessionOfDocumentModelDB? _value_field = SessionValues
            .FirstOrDefault(s => s.RowNum == 0 && s.Name == e.Name);

        if (_value_field is null)
        {
            _value_field = new()
            {
                Name = e.Name,
                Value = value?.Length < 1 ? null : JsonConvert.SerializeObject(value),
                RowNum = 0,
                OwnerId = Session.Id,
                JoinFormToTabId = Join.Id
            };
            SessionValues.Add(_value_field);
        }
        else
            _value_field.Value = value?.Length < 1 ? null : JsonConvert.SerializeObject(value);

        FormChangeAction(this);
    }
    #endregion
    #endregion


    string GetFieldDomId(BaseRequiredFormFitModel bf)
    {
        return $"{Join.Id}-{bf?.SystemName}";
    }

    /// <inheritdoc/>
    public override async Task SaveForm()
    {
        if (DocumentKey is null || SessionValues is null)
            throw new Exception();

        await SetBusy();
        
        TResponseModel<ValueDataForSessionOfDocumentModelDB[]> res = await ConstructorRepo.SaveSessionForm(new() { JoinFormToTab = Join.Id, SessionId = DocumentKey.Value, SessionValues = SessionValues });
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        IsBusyProgress = false;

        SessionValues.Clear();
        if (res.Response?.Length > 0)
        {
            SessionValues.AddRange(res.Response);

            _selfSessionValues ??= [];
            _selfSessionValues.AddRange(SessionValues);
        }

        foreach (FieldBaseComponentModel fb in FieldsComponents)
        {
            fb.CommitChange();
            fb.StateHasChangedCall();
        }
    }

    /// <inheritdoc/>
    public override void ResetForm()
    {
        _selfSessionValues?.Clear();
        _selfSessionValues ??= [];
        if (SessionValues?.Count > 0)
            _selfSessionValues.AddRange(SessionValues);

        base.ResetForm();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (SessionValues is not null)
        {
            _selfSessionValues ??= [];
            _selfSessionValues.AddRange(SessionValues);
        }

        await base.OnInitializedAsync();
    }
}