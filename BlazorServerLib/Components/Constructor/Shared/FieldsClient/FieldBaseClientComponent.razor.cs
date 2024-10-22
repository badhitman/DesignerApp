////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsClient;

/// <summary>
/// Field base client
/// </summary>
public partial class FieldBaseClientComponent : FieldComponentBaseModel
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public FieldFormConstructorModelDB Field { get; set; } = default!;


    VirtualColumnCalculationAbstraction? _calculation_service;
    CommandAsEntryModel? _md;
    /// <inheritdoc/>
    public string AboutCalculationFieldValue
    {
        get
        {
            CommandEntryModel? md = CalculationsAsEntries?.FirstOrDefault(EqualCommand);
            return $"{md?.Name ?? "ошибка 99A9A8C3-4748-45BD-B4C9-A34BA6C13ECF"}: {Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)}";
        }
    }

    bool EqualCommand(CommandEntryModel comm)
    {
        object? ext = Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor);
        return comm.Id == ext?.ToString();
    }

    string? CalculateFieldValue
    {
        get
        {
            if (!InUse)
                return "<калькуляция>";
            else if (string.IsNullOrWhiteSpace(Field.MetadataValueType) || !CellsValuesOfCurrentRow.Any() || QueryFieldsOfNumericTypes is null)
                return null;

            _md ??= DeclarationAbstraction.ParseCommandsAsEntries(Field.MetadataValueType);

            if (_md?.Options.Any() != true)
                return null;

            _calculation_service ??= (DeclarationAbstraction.GetHandlerService(_md.CommandName) as VirtualColumnCalculationAbstraction);
            if (_calculation_service is null || SessionDocument is null || PageJoinForm is null)
                return null;

            Dictionary<string, double> columns = [];
            foreach (string _ff in FieldsNames)
                if (double.TryParse(CellsValuesOfCurrentRow.FirstOrDefault(x => x.Name.Equals(_ff))?.Value, out double _d))
                    columns.Add(_ff, _d);

            if (columns.Count == 0)
                return null;

            return _calculation_service.Calculate(columns, _md.Options).ToString();
        }
    }
    static IEnumerable<CommandEntryModel> Entries = [];

    /// <summary>
    /// Вид параметра
    /// </summary>
    string? Descriptor => Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)?.ToString();

    /// <summary>
    /// Параметр
    /// </summary>
    string? Parameter => Field.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter)?.ToString();

    IQueryable<FieldFormConstructorModelDB>? QueryFieldsOfNumericTypes => PageJoinForm?.Form?.QueryFieldsOfNumericTypes(Field.Name);
    IEnumerable<string> FieldsNames => QueryFieldsOfNumericTypes?.Select(x => x.Name) ?? Enumerable.Empty<string>();
    IEnumerable<ValueDataForSessionOfDocumentModelDB> CellsValuesOfCurrentRow => SessionDocument?.RowsData(PageJoinForm!.Id)?.FirstOrDefault(x => x.Key == GroupByRowNum) ?? Enumerable.Empty<ValueDataForSessionOfDocumentModelDB>();

    string? _stringFieldValue;
    /// <inheritdoc/>
    public string? StringFieldValue
    {
        get => _stringFieldValue;
        set
        {
            _stringFieldValue = value;
            InvokeAsync(async () =>
            {
                await SetBusy();
                StateHasChanged();
                await SetValue(_stringFieldValue, Field.Name);
                StateHasChanged();
                IsBusyProgress = false;
            });
        }
    }

    bool? _boolFieldValue;
    /// <inheritdoc/>
    public bool? BoolFieldValue
    {
        get => _boolFieldValue;
        set
        {
            _boolFieldValue = value;
            InvokeAsync(async () =>
            {
                await SetBusy();
                StateHasChanged();
                await SetValue(_boolFieldValue.ToString(), Field.Name);
                IsBusyProgress = false;
                StateHasChanged();
            });
        }
    }

    DateTime? _dateTimeFieldValue;
    /// <inheritdoc/>
    public DateTime? DateTimeFieldValue
    {
        get => _dateTimeFieldValue;
        set
        {
            _dateTimeFieldValue = value;
            InvokeAsync(async () =>
            {
                await SetBusy();
                StateHasChanged();
                await SetValue(_dateTimeFieldValue.ToString(), Field.Name);
                IsBusyProgress = false;
                StateHasChanged();
            });
        }
    }

    double? _doubleFieldValue;
    /// <inheritdoc/>
    public double? DoubleFieldValue
    {
        get => _doubleFieldValue;
        set
        {
            _doubleFieldValue = value;
            InvokeAsync(async () =>
            {
                await SetBusy();
                StateHasChanged();
                await SetValue(_doubleFieldValue.ToString(), Field.Name);
                IsBusyProgress = false;
                StateHasChanged();
            }
            );
        }
    }

    int? _intFieldValue;
    /// <inheritdoc/>
    public int? IntFieldValue
    {
        get => _intFieldValue;
        set
        {
            _intFieldValue = value;
            InvokeAsync(async () =>
            {
                await SetBusy();
                StateHasChanged();
                await SetValue(_intFieldValue.ToString(), Field.Name);
                IsBusyProgress = false;
                StateHasChanged();
            });
        }
    }

    /// <inheritdoc/>
    public override string DomID => $"form-{Form.Id}_{Field.GetType().FullName}-{DocumentPage?.Id}-{Field.Id}";

    /// <inheritdoc/>
    public string? FieldValue => SessionDocument?.DataSessionValues?.FirstOrDefault(x => x.JoinFormToTabId == PageJoinForm?.Id && x.Name.Equals(Field.Name, StringComparison.OrdinalIgnoreCase) && x.RowNum == GroupByRowNum)?.Value;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (!Entries.Any())
            Entries = DeclarationAbstraction.CommandsAsEntries<TextFieldValueAgent>();

        switch (Field.TypeField)
        {
            case TypesFieldsFormsEnum.Text or TypesFieldsFormsEnum.Password or TypesFieldsFormsEnum.Time or TypesFieldsFormsEnum.Generator:
                CommandEntryModel? _current_agent = Entries.FirstOrDefault(x => $"{x.Name} #{x.Id}".Equals(Parameter));
                if (_current_agent is not null)
                {
                    TextFieldValueAgent? _declaration = DeclarationAbstraction.GetHandlerService(_current_agent.Id) as TextFieldValueAgent;
                    if (_declaration is not null && FieldValue is null && SessionDocument is not null && DocumentPage is not null && PageJoinForm is not null)
                        StringFieldValue = _declaration.DefaultValueIfNull(Field, SessionDocument, PageJoinForm.Id);
                    else
                        _stringFieldValue = FieldValue;
                }
                else
                    _stringFieldValue = FieldValue;
                break;
            case TypesFieldsFormsEnum.Int:
                if (int.TryParse(FieldValue, out int _out_int))
                    _intFieldValue = _out_int;
                break;
            case TypesFieldsFormsEnum.Double:
                if (double.TryParse(FieldValue, out double _out_double))
                    _doubleFieldValue = _out_double;
                break;
            case TypesFieldsFormsEnum.Bool:
                if (bool.TryParse(FieldValue, out bool _out_bool))
                    _boolFieldValue = _out_bool;
                break;
            case TypesFieldsFormsEnum.DateTime or TypesFieldsFormsEnum.Date:
                if (DateTime.TryParse(FieldValue, out DateTime _out_dt))
                    _dateTimeFieldValue = _out_dt;
                break;
            case TypesFieldsFormsEnum.ProgramCalculationDouble:
                CalculationsAsEntries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
                _stringFieldValue = "<calculator>";
                break;
            default:
                throw new Exception($"Тип данных поля [{Field.TypeField}] не обработан. ошибка {{A715B8B4-80B0-4B18-ACAE-CD88E4307B35}}");
        }
        FieldsReferring?.Add(this);
    }
}