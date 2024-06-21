﻿using Microsoft.AspNetCore.Components;
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

    ConstructorFormDirectoryLinkModelDB? FieldObjectForDirectory
    {
        get
        {
            if (_field_object_master is null)
                return null;

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

    ConstructorFieldFormModelDB? FieldObjectStandard
    {
        get
        {
            if (_field_object_master is null)
                return null;

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

            if (_field_object_master is ConstructorFieldFormModelDB standard_field)
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

    string? _field_name;
    /// <summary>
    /// Имя поля формы
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
            _field_object_master = (ConstructorFieldFormBaseLowModel)EntryDescriptionModel.Build("");
            _field_is_required = false;
            _field_name = "";
        }

        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _field_object_master = GlobalTools.CreateDeepCopy(FieldObject);
    }
}