////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;
using BlazorLib;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebLib.Components.Constructor.Shared.FieldsRowsEditUI;

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
    IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;

    [Inject]
    AuthenticationStateProvider authRepo { get; set; } = default!;


    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }

    /// <summary>
    /// Поле формы
    /// </summary>
    [Parameter, EditorRequired]
    public required FieldFormBaseLowConstructorModel Field { get; set; }

    /// <summary>
    /// Перезагрузка полей (обработчик события)
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<FormConstructorModelDB?> ReloadFieldsHandler { get; set; }


    UserInfoMainModel user = default!;

    FieldFormBaseLowConstructorModel _field_master = default!;

    /// <inheritdoc/>
    protected FieldDirectoryFormRowEditComponent? FieldDirUI_ref;
    /// <inheritdoc/>
    protected FieldFormRowEditComponent? FieldEditUI_ref;

    /// <inheritdoc/>
    protected InputRichTextComponent? _currentTemplateInputRichText_ref;
    /// <inheritdoc/>
    protected SessionsValuesOfFieldViewComponent? _sessionsValuesOfFieldViewComponent_Ref;
    EntryDictModel[]? _elements = null;

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
                if (_field_master is FieldFormConstructorModelDB sf)
                {
                    switch (sf.TypeField)
                    {
                        case TypesFieldsFormsEnum.Generator:
                            _type_name = $"<span class='badge bg-info text-dark text-wrap'>{sf.TypeField.DescriptionInfo()}</span>";
                            break;
                        case TypesFieldsFormsEnum.ProgramCalculationDouble:
                            _type_name = $"<span class='badge bg-primary text-wrap'>{sf.TypeField.DescriptionInfo()}</span>";
                            break;
                        default:
                            _type_name = sf.TypeField.DescriptionInfo();
                            break;
                    }
                }
                else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
                    _type_name = $"<span class='badge bg-success text-wrap position-relative'>Справочник/Список{(df.IsMultiSelect ? "<span title='мульти-выбор' class='position-absolute top-0 start-100 translate-middle p-1 ms-1 bg-danger border border-light rounded-circle'>ml<span class='visually-hidden'>multi select</span></span>" : "")}</span>";
                else
                {
                    string msg = "ошибка CDAD94BA-51E8-49F4-9B15-6901494B8EE4";
                    SnackbarRepo.Add(msg, Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    _type_name = msg;
                }
            }

            return _type_name ?? "<ошибка 72FB2301-9AD0-44A7-A99F-D2186F73FE34>";
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
                if (_field_master is FieldFormConstructorModelDB sf)
                {
                    string? _descriptor = sf.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)?.ToString();
                    string? _parameter = sf.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter)?.ToString();

                    if (!string.IsNullOrEmpty(_descriptor))
                    {
                        DeclarationAbstraction? _d = string.IsNullOrEmpty(_descriptor) ? null : DeclarationAbstraction.GetHandlerService(_descriptor);
                        _information_field = $"<b title='Имя вызываемого метода'>{_d?.Name ?? _descriptor}</b> {(string.IsNullOrWhiteSpace(_parameter) ? "" : $"<code title='параметры запуска'>{_parameter}</code>")}";
                    }

                    if (!string.IsNullOrEmpty(_parameter) && _parameter.TryParseJson(out string[]? out_res) && out_res is not null && out_res.Length != 0)
                    {
                        string[] lost_fields = out_res
                            .Where(x => !Form.AllFields.Any(y => y.Name.Equals(x)))
                            .ToArray();

                        if (lost_fields.Length != 0)
                        {
                            string msg = $"Некоторых полей нет в форме: {string.Join("; ", lost_fields)};";
                            SnackbarRepo.Error(msg);
                            _information_field = $"{_information_field} <span class='font-monospace text-danger'>{msg}</span>";
                        }
                    }
                }
                else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
                    _information_field = df.Directory?.Name;
                else
                {
                    string msg = "ошибка 640D6DCE-0027-425E-81D1-00C16A2D5FCB";
                    SnackbarRepo.Add(msg, Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                    _information_field = msg;
                    return (MarkupString)_information_field;
                }
            }
            _information_field ??= "";
            return (MarkupString)_information_field;
        }
    }

    /// <inheritdoc/>
    protected bool CanDown
    {
        get
        {
            if (Field is FieldFormConstructorModelDB sf)
                return sf.SortIndex < (Form.Fields?.Count + Form.FieldsDirectoriesLinks?.Count);
            else if (Field is FieldFormAkaDirectoryConstructorModelDB df)
                return df.SortIndex < (Form.Fields?.Count + Form.FieldsDirectoriesLinks?.Count);
            else
                SnackbarRepo.Add("ошибка C0688447-05EE-4982-B9E0-D48C7DA89C3F", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

            return false;
        }
    }

    /// <inheritdoc/>
    protected bool CanUp
    {
        get
        {
            if (Field is FieldFormConstructorModelDB sf)
                return sf.SortIndex > 1;
            else if (Field is FieldFormAkaDirectoryConstructorModelDB df)
                return df.SortIndex > 1;
            else
                SnackbarRepo.Add("ошибка EAAC696C-1CDE-41C3-8009-8F8FD4CC2D8E", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

            return false;
        }
    }
    #endregion

    /// <inheritdoc/>
    protected bool CanSave
    {
        get
        {
            if (_field_master is FieldFormConstructorModelDB sf)
            {

            }
            else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
            {
                if (df.DirectoryId < 1)
                    return false;
            }
            else
                SnackbarRepo.Add($"`{_field_master.GetType().Name}`. ошибка 418856D6-DBCA-4AC3-9322-9C86D6EF115B", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

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
                if (deadline_date > DateTime.UtcNow)
                    return $"В работе до {deadline_date}";
                else
                    return "Просрочен";
        }
    }

    /// <inheritdoc/>
    protected async Task ClearValuesForFieldName(int? session_id)
    {
        IsBusyProgress = true;
        ResponseBaseModel rest = await ConstructorRepo.ClearValuesForFieldName(new() { FormId = Form.Id, FieldName = Field.Name, SessionId = session_id });
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
            return;

        _elements = null;
        if (_sessionsValuesOfFieldViewComponent_Ref is not null)
            await _sessionsValuesOfFieldViewComponent_Ref.FindFields();
    }

    /// <inheritdoc/>
    protected void ShowReferrals(EntryDictModel[] elements)
    {
        _elements = elements;
        if (_elements.Length == 0)
            SnackbarRepo.Add("Ссылок нет");

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected void ResetEditField()
    {
        SetFieldAction(Field);
        ChildUpdates();
        _currentTemplateInputRichText_ref?.SetValue(_field_master.Description);
    }

    /// <inheritdoc/>
    protected async Task SaveEditField()
    {
        ResponseBaseModel rest;
        IsBusyProgress = true;
        Action act;
        if (_field_master is FieldFormConstructorModelDB sf)
        {
            string? field_valid = sf.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)?.ToString();
            if (field_valid is not null && Enum.TryParse(field_valid, out PropsTypesMDFieldsEnum myDescriptor))
            {
                string? parameter = sf.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter)?.ToString();

                switch (myDescriptor)
                {
                    case PropsTypesMDFieldsEnum.TextMask:
                        if (string.IsNullOrWhiteSpace(parameter))
                        {
                            IsBusyProgress = false;
                            SnackbarRepo.Error("Укажите маску. Выбран режим [Маска], но сама маска не установлена.");
                            return;
                        }
                        break;
                    case PropsTypesMDFieldsEnum.Template:

                        break;
                    default:

                        break;
                }
            }

            FieldFormConstructorModelDB req = new()
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
            rest = await ConstructorRepo.FormFieldUpdateOrCreate(new() { Payload = req, SenderActionUserId = user.UserId });
            act = () => { ((FieldFormConstructorModelDB)Field).Update(sf); };
        }
        else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
        {
            FieldFormAkaDirectoryConstructorModelDB req = new()
            {
                Id = df.Id,
                Name = df.Name,
                Description = df.Description,
                DirectoryId = df.DirectoryId,
                Css = df.Css,
                Hint = df.Hint,
                OwnerId = df.OwnerId,
                Required = df.Required,
                SortIndex = df.SortIndex,
                IsMultiSelect = df.IsMultiSelect,
            };
            rest = await ConstructorRepo.FormFieldDirectoryUpdateOrCreate(new() { Payload = req, SenderActionUserId = user.UserId });
            _field_master.Update(req);
            act = () => { ((FieldFormAkaDirectoryConstructorModelDB)Field).Update(df); };
        }
        else
        {
            SnackbarRepo.Add("Ошибка 9ACCA3B7-52ED-4687-BEC2-C16AC6A2C3C0", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
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
        TResponseModel<FormConstructorModelDB> rest = await ConstructorRepo.GetForm(Field.OwnerId);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка CD1DAE53-0199-40BE-9EF2-4A3347BAF5E9 Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.Response?.Fields is null || rest.Response?.FieldsDirectoriesLinks is null)
        {
            SnackbarRepo.Add($"Ошибка DA9D4B08-EBB7-47C3-BA72-F3BB81E1A7E3 rest.Content.Form?.Fields is null || rest.Content.Form?.FormsDirectoriesLinks is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        ReloadFieldsHandler(rest.Response);

        if (_field_master is FieldFormConstructorModelDB sf)
        {
            if (rest.Response.Fields.Any(x => x.Id == _field_master.Id))
            {
                Field.Update(rest.Response.Fields.First(x => x.Id == _field_master.Id));
                //_field_master.Update(Field);
                _field_master = FieldFormConstructorModelDB.Build(Field);
                //_field_master = new FieldFormConstructorModelDB()
                //{
                //    Css = Field.Css,
                //    Description = Field.Description,
                //    Hint = Field.Hint,
                //    Id = Field.Id,
                //    Name = Field.Name,
                //    OwnerId = Field.OwnerId,
                //    Required = Field.Required
                //};
            }
        }
        else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
        {
            if (rest.Response.FieldsDirectoriesLinks.Any(x => x.Id == _field_master.Id))
            {
                Field.Update(rest.Response.FieldsDirectoriesLinks.First(x => x.Id == _field_master.Id));
                _field_master = new FieldFormAkaDirectoryConstructorModelDB()
                {
                    Css = Field.Css,
                    Description = Field.Description,
                    Hint = Field.Hint,
                    Id = Field.Id,
                    Name = Field.Name,
                    OwnerId = Field.OwnerId,
                    Required = Field.Required,
                    DirectoryId = df.DirectoryId,
                    Directory = df.Directory,
                    Owner = df.Owner,
                    IsMultiSelect = df.IsMultiSelect,
                };
            }
        }
        else
        {
            SnackbarRepo.Add($"{_field_master.GetType().FullName}. ошибка C5CB2F55-D973-405F-B92E-144C1ABE2591", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState state = await authRepo.GetAuthenticationStateAsync();
        user = state.User.ReadCurrentUserInfo() ?? throw new Exception();

        SetFieldAction(Field);
        await base.OnInitializedAsync();
    }

    void ChildUpdates()
    {
        if (_field_master is FieldFormConstructorModelDB sf)
            FieldEditUI_ref?.Update(sf);
        else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
            FieldDirUI_ref?.Update(df);
    }

    /// <summary>
    /// Удаление поля
    /// </summary>
    protected async Task DeleteClick()
    {
        IsBusyProgress = true;
        ResponseBaseModel rest;
        if (_field_master is FieldFormConstructorModelDB sf)
            rest = await ConstructorRepo.FormFieldDelete(new() { Payload = sf.Id, SenderActionUserId = user.UserId });
        else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
            rest = await ConstructorRepo.FormFieldDirectoryDelete(new() { Payload = df.Id, SenderActionUserId = user.UserId });
        else
        {
            SnackbarRepo.Add($"{_field_master.GetType().FullName}. ошибка 1BCDEFB4-55F5-4A5A-BA61-3EAD2E9063D2", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }
     ;
        await ReloadForm();
    }

    void SetFieldAction(FieldFormBaseLowConstructorModel field)
    {
        if (field is FieldFormConstructorModelDB sf)
            _field_master = new FieldFormConstructorModelDB()
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
        else if (field is FieldFormAkaDirectoryConstructorModelDB df)
            _field_master = new FieldFormAkaDirectoryConstructorModelDB()
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
                SortIndex = df.SortIndex,
                IsMultiSelect = df.IsMultiSelect,
            };
        else
            SnackbarRepo.Add("error 81F06C12-3641-473B-A2DA-9EFC853A0709", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

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
        TResponseModel<FormConstructorModelDB> rest;
        if (_field_master is FieldFormConstructorModelDB sf)
            rest = await ConstructorRepo.FieldFormMove(new() { Payload = new() { Id = sf.Id, Direct = VerticalDirectionsEnum.Up }, SenderActionUserId = user.UserId });
        else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
            rest = await ConstructorRepo.FieldDirectoryFormMove(new() { Payload = new() { Id = df.Id, Direct = VerticalDirectionsEnum.Up }, SenderActionUserId = user.UserId });
        else
        {
            SnackbarRepo.Add("ошибка 591195A4-959D-4CDD-9410-F8984F790CBE", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка AA01EFE2-DF81-4CDC-8CAB-D2CAC6B34912 rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        Form.Reload(rest.Response);
        ReloadFieldsHandler(Form);
    }

    /// <summary>
    /// Сдвиг поля на одну позицию в сторону конца
    /// </summary>
    protected async Task MoveFieldDown()
    {
        IsBusyProgress = true;
        TResponseModel<FormConstructorModelDB> rest;
        if (_field_master is FieldFormConstructorModelDB sf)
            rest = await ConstructorRepo.FieldFormMove(new() { Payload = new() { Id = sf.Id, Direct = VerticalDirectionsEnum.Down }, SenderActionUserId = user.UserId });
        else if (_field_master is FieldFormAkaDirectoryConstructorModelDB df)
            rest = await ConstructorRepo.FieldDirectoryFormMove(new() { Payload = new() { Id = df.Id, Direct = VerticalDirectionsEnum.Down }, SenderActionUserId = user.UserId });
        else
        {
            SnackbarRepo.Add("ошибка 8768E090-BE63-4FE4-A693-7E24ED1A1876", Severity.Error, cf => cf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            IsBusyProgress = false;
            return;
        }
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);

        if (!rest.Success())
        {
            await ParentFormsPage.ReadCurrentMainProject();
            ParentFormsPage.StateHasChangedCall();
            return;
        }

        if (rest.Response is null)
        {
            SnackbarRepo.Add($"Ошибка 04BD92F1-0B55-46C5-93B3-4DACB7374565 rest.Content.Form is null", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        Form.Reload(rest.Response);
        ReloadFieldsHandler(Form);
    }
}