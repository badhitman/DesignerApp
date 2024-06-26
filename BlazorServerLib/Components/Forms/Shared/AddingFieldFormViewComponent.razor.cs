////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Adding field form view
/// </summary>
public partial class AddingFieldFormViewComponent : ComponentBase
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public ConstructorFieldFormBaseLowModel? FieldObject { get; set; }
    ConstructorFieldFormBaseLowModel? _field_object_master;

    [CascadingParameter, EditorRequired]
    Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    LinkDirectoryToFormConstructorModelDB FieldObjectForDirectory
    {
        get
        {
            if (_field_object_master is null)
                return new() { SystemName = "", Name = "", OwnerId = Form.Id };

            LinkDirectoryToFormConstructorModelDB res = new()
            {
                SystemName = _field_object_master.SystemName,
                Id = _field_object_master.Id,
                Description = _field_object_master.Description,
                Hint = _field_object_master.Hint,
                Name = _field_object_master.Name,
                OwnerId = _field_object_master.OwnerId,
                Required = _field_object_master.Required,
                Css = _field_object_master.Css
            };

            if (_field_object_master is LinkDirectoryToFormConstructorModelDB directory_field)
            {
                res.DirectoryId = directory_field.DirectoryId;
                res.SortIndex = directory_field.SortIndex;
            }

            return res;
        }
    }

    FieldFormConstructorModelDB FieldObjectStandard
    {
        get
        {
            if (_field_object_master is null)
                return new() { SystemName = "", Name = "", OwnerId = Form.Id, TypeField = (TypesFieldsFormsEnum)SelectedTypeFieldForAdding };

            FieldFormConstructorModelDB res = new()
            {
                SystemName = _field_object_master.SystemName,
                Id = _field_object_master.Id,
                Description = _field_object_master.Description,
                Hint = _field_object_master.Hint,
                Name = _field_object_master.Name,
                OwnerId = _field_object_master.OwnerId,
                Required = _field_object_master.Required,
                TypeField = (TypesFieldsFormsEnum)SelectedTypeFieldForAdding,
                Css = _field_object_master.Css
            };

            if (_field_object_master is FieldFormConstructorModelDB standard_field)
            {
                res.MetadataValueType = standard_field.MetadataValueType;
                res.SortIndex = standard_field.SortIndex;
            }

            return res;
        }
    }

    /// <inheritdoc/>
    public ProgramCalculationFieldFormUIComponent? FieldProgramCalculationDouble;
    /// <inheritdoc/>
    public DirectoryFieldFormUIComponent? FieldDirUI;
    /// <inheritdoc/>
    public TextFieldFormUIComponent? FieldTextUI;
    /// <inheritdoc/>
    public GeneratorFieldFormUIComponent? FieldGeneratorUI;

    int _selected_type_field;
    /// <summary>
    /// Выбранный тип поля (TypesFieldsFormsEnum + Списки/Справочники)
    /// </summary>
    public int SelectedTypeFieldForAdding
    {
        get => _selected_type_field;
        private set
        {
            _selected_type_field = value;

            _field_object_master = int.MaxValue == _selected_type_field
            ? FieldObjectForDirectory
            : FieldObjectStandard;

            if (_field_object_master is null)
                throw new Exception("Поле формы не инициализировано");

            StateHasChangedHandler(_field_object_master, this.GetType());
        }
    }

    string? _field_system_name;
    /// <summary>
    /// Системное имя поля
    /// </summary>
    public string? FieldSystemName
    {
        get => _field_system_name;
        private set
        {
            if (_field_object_master is null)
                throw new Exception("Поле формы не инициализировано");

            _field_system_name = value;
            _field_object_master.Name = _field_name ?? "";
            _field_object_master.SystemName = _field_system_name ?? "";
            _field_object_master.Required = FieldIsRequired;
            ChildUpdates();

            StateHasChangedHandler(_field_object_master, this.GetType());
        }
    }

    string? _field_name;
    /// <summary>
    /// Название поля формы
    /// </summary>
    public string? FieldName
    {
        get => _field_name;
        private set
        {
            if (_field_object_master is null)
                throw new Exception("Поле формы не инициализировано");

            _field_name = value;
            _field_object_master.Name = _field_name ?? "";
            _field_object_master.Required = FieldIsRequired;
            _field_object_master.SystemName = _field_system_name ?? "";
            ChildUpdates();

            StateHasChangedHandler(_field_object_master, this.GetType());
        }
    }

    bool _field_is_required;
    /// <summary>
    /// Признак [required] для поля формы
    /// </summary>
    public bool FieldIsRequired
    {
        get => _field_is_required;
        private set
        {
            if (_field_object_master is null)
                throw new Exception("Поле формы не инициализировано");

            _field_is_required = value;
            _field_object_master.Required = _field_is_required;
            _field_object_master.Name = _field_name ?? "";
            _field_object_master.SystemName = _field_system_name ?? "";
            ChildUpdates();
            StateHasChangedHandler(_field_object_master, this.GetType());
        }
    }

    void ChildUpdates()
    {
        if (_field_object_master is null)
            throw new Exception("Поле формы не инициализировано");

        FieldDirUI?.Update(_field_object_master);
        FieldTextUI?.Update(_field_object_master);
        FieldProgramCalculationDouble?.Update(_field_object_master);
    }

    static string GetStyleClassForOption(TypesFieldsFormsEnum tf)
    {
        return tf switch
        {
            TypesFieldsFormsEnum.Text => "text-danger-emphasis",
            TypesFieldsFormsEnum.ProgramCalculationDouble => "text-primary",
            TypesFieldsFormsEnum.Generator => "text-info",
            _ => ""
        };
    }

    /// <inheritdoc/>
    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        if (_field_object_master is null)
            throw new Exception("Поле формы не инициализировано");

        _field_object_master.Update(field);
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void SetTypeField(int field_type = 0)
    {
        SelectedTypeFieldForAdding = field_type;
        if (field_type == 0)
        {
            if (FieldObject is ConstructorFieldFormBaseModel && FieldTextUI is not null)
                FieldTextUI.FieldParameter = "";

            _field_object_master = null;
            _field_is_required = false;
            _field_system_name = "";
            _field_name = "";
        }

        StateHasChanged();
    }
}