using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsRowsEditUI;

/// <summary>
/// Field form row view
/// </summary>
public partial class FieldFormRowViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IJSRuntime JsRuntimeRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }

    /// <summary>
    /// Поле формы
    /// </summary>
    [Parameter, EditorRequired]
    public required ConstructorFieldFormBaseLowModel Field { get; set; }

    /// <summary>
    /// Перезагрузка полей (обработчик события)
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<ConstructorFormModelDB?> ReloadFieldsHandler { get; set; }

    ConstructorFieldFormBaseLowModel _field_master = default!;

    /// <inheritdoc/>
    protected FieldDirectoryFormRowEditComponent? FieldDirUI_ref;
    /// <inheritdoc/>
    protected FieldFormRowEditComponent? FieldEditUI_ref;

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText_ref;
    /// <inheritdoc/>
    protected SessionsValuesOfFieldViewComponent? _sessionsValuesOfFieldViewComponent_Ref;
    IEnumerable<EntryDictModel>? _elements = null;

    #region row of table (visual)

    /// <inheritdoc/>
    protected MarkupString TypeNameMS => (MarkupString)TypeName;

    string? _type_name;
    string TypeName
    {
        get
        {
            if (_type_name is null)
            {
                if (_field_master is ConstructorFieldFormModelDB sf)
                {
                    switch (sf.TypeField)
                    {
                        case TypesFieldsFormsEnum.Generator:
                            _type_name = $"<span class='badge bg-info text-dark text-wrap'>{sf.TypeField.DescriptionInfo()}</span>";
                            break;
                        case TypesFieldsFormsEnum.ProgrammCalcDouble:
                            _type_name = $"<span class='badge bg-primary text-wrap'>{sf.TypeField.DescriptionInfo()}</span>";
                            break;
                        default:
                            _type_name = sf.TypeField.DescriptionInfo();
                            break;
                    }
                }
                else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
                    _type_name = "<span class='badge bg-success text-wrap'>Справочник/Список</span>";
                else
                {
                    string msg = "ошибка {DDF60327-09D6-430D-B7B5-43F5A28D47A3}";
                    SnackbarRepo.Add(msg, Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    _type_name = msg;
                }
            }

            return _type_name ?? "<ошибка {305DC695-D71D-4BDA-B25E-1F910F1431BD}>";
        }
        set { _type_name = value; }
    }

    string? _information_field;

    /// <inheritdoc/>
    protected MarkupString InformationField
    {
        get
        {
            if (_information_field is null)
            {
                if (_field_master is ConstructorFieldFormModelDB sf)
                {
                    string? descriptor = sf.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)?.ToString();
                    DeclarationAbstraction? _d = DeclarationAbstraction.GetHandlerService(descriptor ?? "");
                    _information_field = $"<b>{_d?.Name ?? descriptor}</b> <u>{sf.TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter)}</u>";
                }
                else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
                    _information_field = df.Directory?.Name;
                else
                {
                    string msg = "ошибка {EF77C0E0-B335-45FA-9A88-17DBE46DFE73}";
                    SnackbarRepo.Add(msg, Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    _information_field = msg;
                    return (MarkupString)_information_field;
                }
            }
            _information_field ??= "<ошибка {808A1311-922F-40AC-A0F6-40537F314679}>";
            return (MarkupString)_information_field;
        }
    }

    /// <inheritdoc/>
    protected bool CanDown
    {
        get
        {
            if (Field is ConstructorFieldFormModelDB sf)
                return sf.SortIndex < (Form.Fields?.Count() + Form.FormsDirectoriesLinks?.Count());
            else if (Field is ConstructorFormDirectoryLinkModelDB df)
                return df.SortIndex < (Form.Fields?.Count() + Form.FormsDirectoriesLinks?.Count());
            else
                SnackbarRepo.Add("ошибка {67959803-04BE-4F50-8E66-EA654F4309E9}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

            return false;
        }
    }

    /// <inheritdoc/>
    protected bool CanUp
    {
        get
        {
            if (Field is ConstructorFieldFormModelDB sf)
                return sf.SortIndex > 1;
            else if (Field is ConstructorFormDirectoryLinkModelDB df)
                return df.SortIndex > 1;
            else
                SnackbarRepo.Add("ошибка {8EFCDE7F-19C7-485C-B766-E84D5A63D7D8}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

            return false;
        }
    }
    #endregion

    /// <inheritdoc/>
    protected bool CanSave
    {
        get
        {
            if (_field_master is ConstructorFieldFormModelDB sf)
            {

            }
            else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
            {
                if (df.DirectoryId < 1)
                    return false;
            }
            else
                SnackbarRepo.Add($"`{_field_master.GetType().Name}`. ошибка {{6D69C0F1-DBF1-49A1-8250-FB4E5CAA6905}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

            return true;
        }
    }

    /// <inheritdoc/>
    protected bool IsEditRow = false;

    /// <inheritdoc/>
    protected static string GetInfoRow(SessionsStatusesEnum? _mark_as_done, DateTime deadline_date)
    {
        switch (_mark_as_done)
        {
            case SessionsStatusesEnum.Accepted:
                return "Принято";
            case SessionsStatusesEnum.Sended:
                return "На проверке";
            default:
                if (deadline_date > DateTime.Now)
                    return $"В работе до {deadline_date}";
                else
                    return "Просрочен";
        }
    }

    /// <inheritdoc/>
    protected async Task ClearValuesForFieldName(int? session_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.ClearValuesForFieldName(new() { FormId = Form.Id, FieldName = Field.Name, SessionId = session_id });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        _elements = null;
        if (_sessionsValuesOfFieldViewComponent_Ref is not null)
            await _sessionsValuesOfFieldViewComponent_Ref.FindFields();
    }

    /// <inheritdoc/>
    protected void ShowReferrals(IEnumerable<EntryDictModel> elements)
    {
        _elements = elements;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected async Task ResetEditField()
    {
        SetFieldAction(Field);
        ChildUpdates();
        if (_currentTemplateInputRichText_ref is not null)
            await JsRuntimeRepo.InvokeVoidAsync("CKEditorInterop.setValue", _currentTemplateInputRichText_ref.UID, _field_master.Description);
    }

    /// <inheritdoc/>
    protected async Task SaveEditField()
    {
        ResponseBaseModel rest;
        IsBusyProgress = true;
        Action act;
        if (_field_master is ConstructorFieldFormModelDB sf)
        {
            ConstructorFieldFormModelDB req = new()
            {
                Css = sf.Css,
                Description = sf.Description,
                Hint = sf.Hint,
                Id = sf.Id,
                MetadataValueType = sf.MetadataValueType,
                Name = sf.Name,
                OwnerId = sf.OwnerId,
                Required = sf.Required,
                SortIndex = sf.SortIndex,
                TypeField = sf.TypeField
            };
            rest = await FormsRepo.FormFieldUpdateOrCreate(req);
            act = () => { ((ConstructorFieldFormModelDB)Field).Update(sf); };
        }
        else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
        {
            ConstructorFormDirectoryLinkModelDB req = new()
            {
                Id = df.Id,
                Name = df.Name,
                Description = df.Description,
                DirectoryId = df.DirectoryId,
                Css = df.Css,
                Hint = df.Hint,
                OwnerId = df.OwnerId,
                Required = df.Required,
                SortIndex = df.SortIndex
            };
            rest = await FormsRepo.FormFieldDirectoryUpdateOrCreate(req);
            act = () => { ((ConstructorFormDirectoryLinkModelDB)Field).Update(df); };
        }
        else
        {
            SnackbarRepo.Add("Ошибка {BB20028A-0413-4012-B2C8-A6DA579DF340}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{5236C09E-C4BF-4AAA-9F6B-DA3F36648B0F}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        act();

        IsEditRow = false;

        _type_name = null;
        _information_field = null;

        await ReloadForm();
    }

    async Task ReloadForm()
    {
        IsBusyProgress = true;
        FormResponseModel rest = await FormsRepo.GetForm(Field.OwnerId);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{010B3124-8920-421E-B792-DF2D08CBE1C7}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Form?.Fields is null || rest.Form?.FormsDirectoriesLinks is null)
        {
            SnackbarRepo.Add($"Ошибка {{A0342E4D-3C00-4DA2-8EA3-7C1072D225F4}} rest.Content.Form?.Fields is null || rest.Content.Form?.FormsDirectoriesLinks is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        ReloadFieldsHandler(rest.Form);

        if (_field_master is ConstructorFieldFormModelDB sf)
        {
            if (rest.Form.Fields.Any(x => x.Id == _field_master.Id))
            {
                Field.Update(rest.Form.Fields.First(x => x.Id == _field_master.Id));
                _field_master = new ConstructorFieldFormModelDB()
                {
                    Css = Field.Css,
                    Description = Field.Description,
                    Hint = Field.Hint,
                    Id = Field.Id,
                    Name = Field.Name,
                    OwnerId = Field.OwnerId,
                    Required = Field.Required
                };
            }
        }
        else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
        {
            if (rest.Form.FormsDirectoriesLinks.Any(x => x.Id == _field_master.Id))
            {
                Field.Update(rest.Form.FormsDirectoriesLinks.First(x => x.Id == _field_master.Id));
                _field_master = new ConstructorFormDirectoryLinkModelDB()
                {
                    Css = Field.Css,
                    Description = Field.Description,
                    Hint = Field.Hint,
                    Id = Field.Id,
                    Name = Field.Name,
                    OwnerId = Field.OwnerId,
                    Required = Field.Required,
                    DirectoryId = df.DirectoryId
                };
            }
        }
        else
        {
            SnackbarRepo.Add($"{_field_master.GetType().FullName}. ошибка {{75A28907-ABF5-4847-B116-895A70C21B8C}}", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        SetFieldAction(Field);
        base.OnInitialized();
    }

    void ChildUpdates()
    {
        if (_field_master is ConstructorFieldFormModelDB sf)
            FieldEditUI_ref?.Update(sf);
        else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
            FieldDirUI_ref?.Update(df);
    }

    /// <summary>
    /// Удаление поля
    /// </summary>
    protected async Task DeleteClick()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest;
        if (_field_master is ConstructorFieldFormModelDB sf)
            rest = await FormsRepo.FormFieldDelete(sf.Id);
        else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
            rest = await FormsRepo.FormFieldDirectoryDelete(df.Id);
        else
        {
            SnackbarRepo.Add($"{_field_master.GetType().FullName}. ошибка {{B645D471-DA36-4876-9062-1F9731905001}}", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{C8D7E842-026C-4897-A0BA-D9A04F51E2B6}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
     ;
        await ReloadForm();
    }

    void SetFieldAction(ConstructorFieldFormBaseLowModel field)
    {
        if (field is ConstructorFieldFormModelDB sf)
            _field_master = new ConstructorFieldFormModelDB()
            {
                Css = sf.Css,
                Description = sf.Description,
                Hint = sf.Hint,
                Id = sf.Id,
                MetadataValueType = sf.MetadataValueType,
                Name = sf.Name,
                Owner = sf.Owner,
                OwnerId = sf.OwnerId,
                Required = sf.Required,
                SortIndex = sf.SortIndex,
                TypeField = sf.TypeField
            };
        else if (field is ConstructorFormDirectoryLinkModelDB df)
            _field_master = new ConstructorFormDirectoryLinkModelDB()
            {
                Css = df.Css,
                Description = df.Description,
                Directory = df.Directory,
                DirectoryId = df.DirectoryId,
                Hint = df.Hint,
                Id = df.Id,
                Name = df.Name,
                Owner = df.Owner,
                OwnerId = df.OwnerId,
                Required = df.Required,
                SortIndex = df.SortIndex
            };
        else
            SnackbarRepo.Add("error {FF425607-1C08-48A1-9BDD-D7A3C7A1F3E2}", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

        _type_name = null;
        _information_field = null;

        StateHasChanged();
    }

    /// <summary>
    /// Сдвиг поля на одну позицию в сторону конца
    /// </summary>
    protected async Task MoveFieldUp()
    {
        IsBusyProgress = true;
        FormResponseModel rest;
        if (_field_master is ConstructorFieldFormModelDB sf)
            rest = await FormsRepo.FieldFormMove(sf.Id, VerticalDirectionsEnum.Up);
        else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
            rest = await FormsRepo.FieldDirectoryFormMove(df.Id, VerticalDirectionsEnum.Up);
        else
        {
            SnackbarRepo.Add("ошибка {348A595E-3FC4-46A1-8BC2-76A040B22E78}", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{7F179487-6C23-4E16-9300-847D28416251}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Form is null)
        {
            SnackbarRepo.Add($"Ошибка {{CC30C3BB-1206-46D4-92E3-926AA77B611B}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        Form.Reload(rest.Form);
        ReloadFieldsHandler(Form);
    }

    /// <summary>
    /// Сдвиг поля на одну позицию в сторону конца
    /// </summary>
    protected async Task MoveFieldDown()
    {
        IsBusyProgress = true;
        FormResponseModel rest;
        if (_field_master is ConstructorFieldFormModelDB sf)
            rest = await FormsRepo.FieldFormMove(sf.Id, VerticalDirectionsEnum.Down);
        else if (_field_master is ConstructorFormDirectoryLinkModelDB df)
            rest = await FormsRepo.FieldDirectoryFormMove(df.Id, VerticalDirectionsEnum.Down);
        else
        {
            SnackbarRepo.Add("ошибка {19A946C6-394D-4853-B9A8-0BA853970709}", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{B75503B5-0E20-4DBD-BDF4-C9591E693E75}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Form is null)
        {
            SnackbarRepo.Add($"Ошибка {{6A63AC00-65E7-40C4-8487-1C586C149145}} rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        Form.Reload(rest.Form);
        ReloadFieldsHandler(Form);
    }
}