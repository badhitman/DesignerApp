using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared.Manufacture;

public partial class DocumentsManufactureComponent : ComponentBase
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ManufactureComponent ManufactureParentView { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required SystemNameEntryModel[] SystemNamesManufacture { get; set; }


    List<TreeItemData<EntryTagModel>> TreeItems { get; set; } = [];

    const string icon_doc = Icons.Material.Filled.BusinessCenter;
    const string icon_tab_of_doc = Icons.Material.Filled.Tab;
    const string icon_form_of_tab = Icons.Material.Filled.DynamicForm;
    const string icon_field_of_form = Icons.Material.Filled.DragHandle;

    /// <summary>
    /// имя типа данных: документы
    /// </summary>
    const string type_name_document = nameof(DocumentSchemeConstructorModelDB);
    /// <summary>
    /// имя типа данных: табы/вкладки документов
    /// </summary>
    const string type_name_tab_of_document = nameof(TabOfDocumentSchemeConstructorModelDB);
    /// <summary>
    /// имя типа данных: формы
    /// </summary>
    const string type_name_form_of_tab = nameof(FormConstructorModelDB);
    /// <summary>
    /// имя типа данных: поля формы
    /// </summary>
    const string type_name_field_of_form = nameof(FieldFormConstructorModelDB);

    const string type_name_base_field_of_form = nameof(FieldFormBaseLowConstructorModel);

    MudTreeView<EntryTagModel>? TreeView_ref;

    static Color GetColor(string? icon)
    => icon switch
    {
        icon_form_of_tab => Color.Info,
        icon_tab_of_doc => Color.Primary,
        icon_field_of_form => Color.Success,
        _ => Color.Default
    };

    static TypesFieldsFormsEnum[] skip_fields = [TypesFieldsFormsEnum.Generator, TypesFieldsFormsEnum.ProgramCalculationDouble];

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        TreeItemDataModel FieldToTreeItem(FieldFormBaseLowConstructorModel field, int doc_id, int tab_id, int form_id)
        {
            EntryTagModel et = new()
            {
                Id = field.Id,
                Name = field.Name,
                Tag = $"{type_name_document}#{doc_id} {type_name_tab_of_document}#{tab_id} {type_name_form_of_tab}#{form_id} {type_name_base_field_of_form}"
            };

            TreeItemDataModel _res = new(et, icon_field_of_form)
            {
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
            else if (field is LinkDirectoryToFormConstructorModelDB df)
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
                Tag = $"{type_name_document}#{doc_id} {type_name_tab_of_document}#{tab_id} {type_name_form_of_tab}"
            };

            return new TreeItemDataModel(et, icon_form_of_tab)
            {
                Qualification = type_name_form_of_tab,
                Tooltip = "Форма, размещённая внутри таба/вкладки",
                Children = [.. form.AllFields.Select(field => FieldToTreeItem(field, doc_id, tab_id, form.Id))]
            };
        }

        TreeItemDataModel TabToTreeItem(TabOfDocumentSchemeConstructorModelDB tab, int doc_id)
        {
            EntryTagModel et = new()
            {
                Id = tab.Id,
                Name = tab.Name,
                Tag = $"{type_name_document}#{doc_id} {type_name_tab_of_document}"
            };

            return new TreeItemDataModel(et, icon_tab_of_doc)
            {
                Qualification = type_name_document,
                Tooltip = "Вкладка/Таб документа",
                Children = [.. tab.JoinsForms!.Select(x => FormToTreeItem(x.Form!, doc_id, tab.Id))]
            };
        }

        ManufactureParentView
        .CurrentProject
        .Documents!.ForEach(x =>
        {
            TreeItems.Add(new TreeItemDataModel(new EntryTagModel() { Name = x.Name, Id = x.Id, Tag = type_name_document }, icon_doc)
            {
                Qualification = type_name_tab_of_document,
                Tooltip = "Документ (схема данных бизнес-сущности)",
                Children = [.. x.Pages!.Select(y => TabToTreeItem(y, x.Id))]
            });
        });
        ManufactureParentView.TreeBuildDoneAction(TreeItems.Select(x => (TreeItemDataModel)x));
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (TreeView_ref is not null && firstRender)
            await TreeView_ref.ExpandAllAsync();
    }
}