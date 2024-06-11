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
    public ConstructorFieldFormBaseLowModel FieldObject { get; set; } = default!;
    ConstructorFieldFormBaseLowModel _field_object_master = default!;

    [CascadingParameter, EditorRequired]
    Action<ConstructorFieldFormBaseLowModel, Type> StateHasChangedHandler { get; set; } = default!;

    ConstructorFormDirectoryLinkModelDB FieldObjectForDirectory
    {
        get
        {
            ConstructorFormDirectoryLinkModelDB res = new()
            {
                Id = _field_object_master.Id,
                Description = _field_object_master.Description,
                Hint = _field_object_master.Hint,
                Name = _field_object_master.Name,
                OwnerId = _field_object_master.OwnerId,
                Required = _field_object_master.Required,
                Css = _field_object_master.Css
            };

            if (_field_object_master is ConstructorFormDirectoryLinkModelDB directory_field)
            {
                res.DirectoryId = directory_field.DirectoryId;
                res.SortIndex = directory_field.SortIndex;
            }

            return res;
        }
    }

    ConstructorFieldFormModelDB FieldObjectStandard
    {
        get
        {
            ConstructorFieldFormModelDB res = new()
            {
                Id = _field_object_master.Id,
                Description = _field_object_master.Description,
                Hint = _field_object_master.Hint,
                Name = _field_object_master.Name,
                OwnerId = _field_object_master.OwnerId,
                Required = _field_object_master.Required,
                TypeField = (TypesFieldsFormsEnum)SelectedTypeFieldForAdding,
                Css = _field_object_master.Css
            };

            if (_field_object_master is ConstructorFieldFormModelDB standart_field)
            {
                res.MetadataValueType = standart_field.MetadataValueType;
                res.SortIndex = standart_field.SortIndex;
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
            StateHasChangedHandler(_field_object_master, this.GetType());
        }
    }

    string? _field_name;
    /// <summary>
    /// Имя поля формы
    /// </summary>
    public string? FieldName
    {
        get => _field_name;
        private set
        {
            _field_name = value;
            _field_object_master.Name = _field_name ?? "";
            _field_object_master.Required = _field_is_required;
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
            _field_is_required = value;
            _field_object_master.Required = _field_is_required;
            _field_object_master.Name = _field_name ?? "";
            ChildUpdates();
            StateHasChangedHandler(_field_object_master, this.GetType());
        }
    }

    void ChildUpdates()
    {
        FieldDirUI?.Update(_field_object_master);
        FieldTextUI?.Update(_field_object_master);
        FieldProgramCalculationDouble?.Update(_field_object_master);
    }

    /// <inheritdoc/>
    public void Update(ConstructorFieldFormBaseLowModel field)
    {
        _field_object_master.Update(field);
        StateHasChanged();
    }

    /// <inheritdoc/>
    public void SetTypeField(int field_type = 0)
    {
        SelectedTypeFieldForAdding = field_type;
        if (field_type == 0)
        {
            _field_object_master = (ConstructorFieldFormBaseLowModel)EntryDescriptionModel.Build("");
            _field_is_required = false;
            _field_name = "";
        }

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _field_object_master = GlobalUtils.CreateDeepCopy(FieldObject);
    }
}