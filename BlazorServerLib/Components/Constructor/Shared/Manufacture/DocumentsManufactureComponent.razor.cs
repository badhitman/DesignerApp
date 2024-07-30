////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Manufacture;

/// <summary>
/// DocumentsManufactureComponent
/// </summary>
public partial class DocumentsManufactureComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ManufactureComponent ManufactureParentView { get; set; }


    const string icon_doc = Icons.Material.Filled.BusinessCenter;
    const string icon_tab_of_doc = Icons.Material.Filled.Tab;
    const string icon_form_of_tab = Icons.Material.Filled.DynamicForm;
    const string icon_field_of_form = Icons.Material.Filled.DragHandle;

    /// <summary>
    /// имя типа данных: табы/вкладки документов
    /// </summary>
    const string type_name_tab_of_document = nameof(TabOfDocumentSchemeConstructorModelDB);
    /// <summary>
    /// имя типа данных: формы
    /// </summary>
    const string type_name_form_of_tab = nameof(FormConstructorModelDB);

    const string type_name_base_field_of_form = nameof(FieldFormBaseLowConstructorModel);

    /// <summary>
    /// Дерево/структура
    /// </summary>
    public List<TreeItemData<EntryTagModel>> TreeItems { get; private set; } = [];
    MudTreeView<EntryTagModel>? TreeView_ref;

    static Color GetColor(string? icon)
    => icon switch
    {
        icon_form_of_tab => Color.Info,
        icon_tab_of_doc => Color.Primary,
        icon_field_of_form => Color.Success,
        _ => Color.Default
    };

    static readonly TypesFieldsFormsEnum[] skip_fields = [TypesFieldsFormsEnum.Generator, TypesFieldsFormsEnum.ProgramCalculationDouble];

    /// <summary>
    /// Перезагрузить дерево элементов
    /// </summary>
    public void ReloadTree()
    {
        TreeItemDataModel FieldToTreeItem(FieldFormBaseLowConstructorModel field, int doc_id, int tab_id, int form_id)
        {
            EntryTagModel et = new()
            {
                Id = field.Id,
                Name = field.Name,
                Tag = $"{ManufactureComponent.DocumentSchemeConstructorTypeName}#{doc_id} {type_name_tab_of_document}#{tab_id} {type_name_form_of_tab}#{form_id} {type_name_base_field_of_form}"
            };

            TreeItemDataModel _res = new(et, icon_field_of_form)
            {
                SystemName = ManufactureParentView.ParentFormsPage.SystemNamesManufacture.GetSystemName(field.Id, et.Tag, field.GetType().Name),
                Tooltip = "Поле внутри формы",
                Qualification = field.GetType().Name
            };

            if (field is FieldFormConstructorModelDB ff)
            {
                _res.Information = $"<span class='badge text-bg-light text-wrap'>{ff.TypeField.DescriptionInfo()}</span>";

                if (skip_fields.Contains(ff.TypeField) || Enum.TryParse(ff.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor, PropsTypesMDFieldsEnum.None.ToString())?.ToString(), out PropsTypesMDFieldsEnum _mode) && _mode == PropsTypesMDFieldsEnum.Template)
                {
                    _res.IsDisabled = true;
                    _res.Tooltip = "Поле формы не будет выгружено: данный тип поля не поддерживается для выгрузки";
                }
                else
                {
                    string? descriptor = ff.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Descriptor)?.ToString();
                    DeclarationAbstraction? _d = DeclarationAbstraction.GetHandlerService(descriptor ?? "");
                    string? parameter = ff.GetValueObjectOfMetadata(MetadataExtensionsFormFieldsEnum.Parameter)?.ToString();
                    _res.Information = $"{_res.Information} <b>{_d?.Name ?? descriptor}</b> <u>{parameter}</u>";

                    if (!_res.IsDisabled && _d is not null && !_d.AllowCallWithoutParameters && string.IsNullOrEmpty(parameter))
                        _res.ErrorMessage = "Не указаны параметры";
                }
            }
            else if (field is FieldFormAkaDirectoryConstructorModelDB df)
                _res.Information = $"<span class='badge bg-success text-wrap'>Справочник/Список</span> {df.Directory?.Name}";
            else
            {
                string msg = "ошибка 60F310E5-52C0-4E13-AD2C-10D9B09B6DD8";
                SnackbarRepo.Add(msg, Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                _res.Information = msg;
            }

            return _res;
        }

        TreeItemDataModel FormToTreeItem(FormConstructorModelDB form, int doc_id, int tab_id)
        {
            EntryTagModel et = new()
            {
                Id = form.Id,
                Name = form.Name,
                Tag = $"{ManufactureComponent.DocumentSchemeConstructorTypeName}#{doc_id} {type_name_tab_of_document}#{tab_id} {type_name_form_of_tab}",
            };

            return new TreeItemDataModel(et, icon_form_of_tab)
            {
                SystemName = ManufactureParentView.ParentFormsPage.SystemNamesManufacture.GetSystemName(form.Id, et.Tag),
                Tooltip = "Форма, размещённая внутри таба/вкладки",
                Children = [.. form.AllFields.Select(field => FieldToTreeItem(field, doc_id, tab_id, form.Id))],
                ErrorMessage = form.AllFields.Length == 0 ? $"Форма '{form.Name}' пустая - нет ни одного поля" : null
            };
        }

        TreeItemDataModel TabToTreeItem(TabOfDocumentSchemeConstructorModelDB tab, int doc_id)
        {
            EntryTagModel et = new()
            {
                Id = tab.Id,
                Name = tab.Name,
                Tag = $"{ManufactureComponent.DocumentSchemeConstructorTypeName}#{doc_id} {type_name_tab_of_document}"
            };

            return new TreeItemDataModel(et, icon_tab_of_doc)
            {
                SystemName = ManufactureParentView.ParentFormsPage.SystemNamesManufacture.GetSystemName(tab.Id, et.Tag),
                Tooltip = "Вкладка/Таб документа",
                Children = [.. tab.JoinsForms!.Select(x => FormToTreeItem(x.Form!, doc_id, tab.Id))],
                ErrorMessage = tab.JoinsForms!.Count == 0 ? $"Таб/Вкладка '{tab.Name}' пустая - нет ни одной формы" : null
            };
        }

        TreeItems.Clear();
        ManufactureParentView
        .CurrentProject
        .Documents!.ForEach(doc =>
        {
            TreeItems.Add(new TreeItemDataModel(new EntryTagModel() { Name = doc.Name, Id = doc.Id, Tag = ManufactureComponent.DocumentSchemeConstructorTypeName }, icon_doc)
            {
                SystemName = ManufactureParentView.ParentFormsPage.SystemNamesManufacture.GetSystemName(doc.Id, ManufactureComponent.DocumentSchemeConstructorTypeName),
                Tooltip = "Документ (схема данных бизнес-сущности)",
                Children = [.. doc.Tabs!.Select(y => TabToTreeItem(y, doc.Id))],
                ErrorMessage = doc.Tabs!.Count == 0 ? $"Документ '{doc.Name}' пустой - не имеет вкладок/табов" : null
            });
        });
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ReloadTree();
        ManufactureParentView.TreeBuildDoneAction(TreeItems.Cast<TreeItemDataModel>());
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (TreeView_ref is not null && firstRender)
            await TreeView_ref.ExpandAllAsync();
    }
}