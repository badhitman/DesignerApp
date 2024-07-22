using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;
using BlazorLib;
using CodegeneratorLib;

namespace BlazorWebLib.Components.Constructor.Shared.Manufacture;

/// <summary>
/// Manufacture
/// </summary>
public partial class ManufactureComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    IConstructorService ConstructorRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IManufactureService ManufactureRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;


    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }


    ConfigManufactureComponent _conf = default!;

    EnumerationsManufactureComponent enumerations_ref = default!;
    DocumentsManufactureComponent documents_ref = default!;

    /// <summary>
    /// DirectoryConstructorModelDB
    /// </summary>
    public static readonly string DirectoryTypeName = nameof(DirectoryConstructorModelDB);
    /// <summary>
    /// ElementOfDirectoryConstructorModelDB
    /// </summary>
    public static readonly string ElementOfDirectoryConstructorTypeName = nameof(ElementOfDirectoryConstructorModelDB);

    /// <summary>
    /// DocumentSchemeConstructorModelDB
    /// </summary>
    public static readonly string DocumentSchemeConstructorTypeName = nameof(DocumentSchemeConstructorModelDB);
    /// <summary>
    /// FieldFormConstructorModelDB
    /// </summary>
    public static readonly string FieldFormConstructorTypeName = nameof(FieldFormConstructorModelDB);
    /// <summary>
    /// FieldFormAkaDirectoryConstructorModelDB
    /// </summary>
    public static readonly string FieldFormAkaDirectoryConstructorTypeName = nameof(FieldFormAkaDirectoryConstructorModelDB);

    /// <summary>
    /// Текущий проект
    /// </summary>
    public ProjectConstructorModelDB CurrentProject { get; private set; } = default!;
    /// <summary>
    /// Manufacture
    /// </summary>
    public ManageManufactureModelDB Manufacture { get; private set; } = default!;

    TResponseModel<Stream>? downloadSource;
    readonly List<string> _errors = [];
    /// <summary>
    /// Build tree doneAction
    /// </summary>
    public void TreeBuildDoneAction(IEnumerable<TreeItemDataModel> treeItems)
    {
        foreach (TreeItemDataModel tree_item in treeItems)
        {
            if (!string.IsNullOrEmpty(tree_item.ErrorMessage))
                _errors.Add(tree_item.ErrorMessage);

            if (tree_item.Children is not null)
                TreeBuildDoneAction(tree_item.Children.Cast<TreeItemDataModel>());
        }
    }

    EnumFitModel EnumConvert(DirectoryConstructorModelDB dir)
    {
        ArgumentNullException.ThrowIfNull(dir.Elements);

        TreeItemDataModel dir_tree_item = (TreeItemDataModel)enumerations_ref.TreeItems.First(x => x.Value?.Id == dir.Id);

        return new EnumFitModel()
        {
            SystemName = dir_tree_item.SystemName ?? GlobalTools.TranslitToSystemName(dir.Name),
            Name = dir.Name,
            Description = dir.Description,
            EnumItems = dir.Elements.Count < 1 ? [] : dir.Elements.Select(e =>
            {
                TreeItemDataModel tree_item = (TreeItemDataModel)dir_tree_item.Children!.First(x => x.Value?.Id == e.Id);

                return new SortableFitModel()
                {
                    SystemName = tree_item.SystemName ?? GlobalTools.TranslitToSystemName(e.Name),
                    Name = e.Name,
                    SortIndex = e.SortIndex,
                    Description = e.Description,
                };
            }).ToArray()
        };
    }

    DocumentFitModel DocumentConvert(DocumentSchemeConstructorModelDB doc)
    {
        TreeItemDataModel document_tree_item = (TreeItemDataModel)documents_ref.TreeItems.First(x => x.Value?.Id == doc.Id);
        ArgumentNullException.ThrowIfNull(document_tree_item.Children);

        TabFitModel TabConvert(TabOfDocumentSchemeConstructorModelDB tab)
        {
            ArgumentNullException.ThrowIfNull(tab.JoinsForms);

            TreeItemDataModel tab_tree_item = (TreeItemDataModel)document_tree_item.Children.First(x => x.Value?.Id == tab.Id);
            ArgumentNullException.ThrowIfNull(tab_tree_item.Children);

            FormFitModel FormConvert(TabJoinDocumentSchemeConstructorModelDB joinForm)
            {
                ArgumentNullException.ThrowIfNull(joinForm.Form);

                TreeItemDataModel form_tree_item = (TreeItemDataModel)tab_tree_item.Children.First(x => x.Value?.Id == joinForm.Form.Id);
                ArgumentNullException.ThrowIfNull(form_tree_item.Children);

                IEnumerable<TreeItemDataModel> fieldsNodes = form_tree_item
                        .Children.Cast<TreeItemDataModel>();

                FieldFitModel FieldConvert(FieldFormConstructorModelDB field)
                {
                    TreeItemDataModel field_tree_item = fieldsNodes
                        .First(x => x.Value?.Id == field.Id && x.Qualification == FieldFormConstructorTypeName);

                    return new FieldFitModel()
                    {
                        Name = field.Name,
                        SortIndex = field.SortIndex,
                        Css = field.Css,
                        Description = field.Description,
                        Hint = field.Hint,
                        MetadataValueType = field.MetadataValueType,
                        Required = field.Required,
                        TypeField = field.TypeField,
                        SystemName = field_tree_item.SystemName ?? GlobalTools.TranslitToSystemName(field.Name),
                    };
                }

                FieldAkaDirectoryFitModel FieldAkaDirectoryConvert(FieldFormAkaDirectoryConstructorModelDB field)
                {
                    TreeItemDataModel field_dir_tree_item = fieldsNodes
                        .First(x => x.Value?.Id == field.Id && x.Qualification == FieldFormAkaDirectoryConstructorTypeName);

                    ArgumentNullException.ThrowIfNull(field_dir_tree_item);

                    return new FieldAkaDirectoryFitModel()
                    {
                        DirectorySystemName = enumerations_ref.TreeItems.Cast<TreeItemDataModel>().First(x => x.Value!.Id == field.Id).SystemName ?? GlobalTools.TranslitToSystemName(field.Name),

                        Name = field.Name,
                        SortIndex = field.SortIndex,
                        SystemName = field_dir_tree_item.SystemName ?? GlobalTools.TranslitToSystemName(field.Name),
                        Css = field.Css,
                        Description = field.Description,
                        Hint = field.Hint,
                        Required = field.Required,
                    };
                }

                return new FormFitModel()
                {
                    Name = joinForm.Form.Name,
                    Css = joinForm.Form.Css,
                    Description = joinForm.Form.Description,
                    SortIndex = joinForm.SortIndex,
                    SystemName = form_tree_item.SystemName ?? GlobalTools.TranslitToSystemName(joinForm.Form.Name),
                    IsTable = joinForm.IsTable,

                    SimpleFields = joinForm.Form.Fields is null ? null : [.. joinForm.Form.Fields.Select(FieldConvert)],
                    FieldsAtDirectories = joinForm.Form.FieldsDirectoriesLinks is null ? null : [.. joinForm.Form.FieldsDirectoriesLinks.Select(FieldAkaDirectoryConvert)],

                    JoinName = joinForm.Name,
                };
            }

            return new TabFitModel()
            {
                Name = tab.Name,
                Description = tab.Description,
                SortIndex = tab.SortIndex,
                SystemName = tab_tree_item.SystemName ?? GlobalTools.TranslitToSystemName(tab.Name),
                Forms = [.. tab.JoinsForms.Select(FormConvert)],
            };
        }


        return new DocumentFitModel()
        {
            SystemName = document_tree_item.SystemName ?? GlobalTools.TranslitToSystemName(doc.Name),
            Name = doc.Name,
            Description = doc.Description,
            Tabs = [.. doc.Tabs!.Select(TabConvert)]
        };
    }

    /// <summary>
    /// Reload project data
    /// </summary>
    public async Task ReloadProjectData()
    {
        IsBusyProgress = true;
        ProjectConstructorModelDB? rest_project = await ConstructorRepo.ReadProject(ParentFormsPage.MainProject!.Id);
        CurrentProject = rest_project ?? throw new Exception();

        TResponseModel<ManageManufactureModelDB> rest_manufacture = await ManufactureRepo.ReadManufactureConfig(ParentFormsPage.MainProject.Id);
        if (!rest_manufacture.Success())
            SnackbarRepo.ShowMessagesResponse(rest_manufacture.Messages);
        Manufacture = rest_manufacture.Response ?? throw new Exception();
        IsBusyProgress = false;
    }

    async Task Download()
    {
        ArgumentNullException.ThrowIfNull(CurrentProject.Directories);
        ArgumentNullException.ThrowIfNull(CurrentProject.Documents);
        ArgumentNullException.ThrowIfNull(ParentFormsPage.MainProject);

        CodeGeneratorConfigModel conf_gen = Manufacture;
        GeneratorCSharpService gen = new(conf_gen, ParentFormsPage.MainProject);

        StructureProjectModel struct_project = new()
        {
            Enums = [.. CurrentProject.Directories.Select(EnumConvert)],
            Documents = [.. CurrentProject.Documents.Select(DocumentConvert)],
        };

        var _err = struct_project
            .Enums
            .GroupBy(e => e.SystemName)
            .Select(x => new { SystemName = x.Key, Count = x.Count() })
            .Where(x => x.Count > 1)
            .ToArray();

        if (_err.Length != 0)
        {
            SnackbarRepo.Add($"Существуют конфликты имён перечислений: {string.Join(";", _err.Select(x => $"{x.SystemName} - {x.Count}"))};", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        _err = struct_project
            .Documents
            .GroupBy(e => e.SystemName)
            .Select(x => new { SystemName = x.Key, Count = x.Count() })
            .Where(x => x.Count > 1)
            .ToArray();

        if (_err.Length != 0)
        {
            SnackbarRepo.Add($"Существуют конфликты имён документов: {string.Join(";", _err.Select(x => $"{x.SystemName} - {x.Count}"))};", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        downloadSource = await gen.GetZipArchive(struct_project);
        if (!downloadSource.Success())
        {
            SnackbarRepo.ShowMessagesResponse(downloadSource.Messages);
            return;
        }
        ArgumentNullException.ThrowIfNull(downloadSource.Response);
        string fileName = $"project-{CurrentProject.Id}-codebase-{DateTime.Now}.zip";

        using DotNetStreamReference streamRef = new(stream: downloadSource.Response);
        await JsRuntimeRepo.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        downloadSource.Response.Close();
    }

    /// <inheritdoc/>
    public override void StateHasChangedCall()
    {
        enumerations_ref.ReloadTree();
        enumerations_ref.StateHasChangedCall();

        documents_ref.ReloadTree();
        documents_ref.StateHasChangedCall();
        base.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadProjectData();
    }

    /// <inheritdoc/>
    public static string? CheckName(string name)
    {
        if (!string.IsNullOrEmpty(name) && name[..1] != name[..1].ToUpper())
            return "<span class=\"text-danger font-monospace\">Первый символ лучше сделать прописным</span>";

        return null;
    }
}