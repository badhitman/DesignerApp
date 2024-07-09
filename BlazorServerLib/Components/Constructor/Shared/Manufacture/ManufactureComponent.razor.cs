using BlazorWebLib.Components.Constructor.Pages;
using Microsoft.AspNetCore.Components;
using SharedLib.Models;
using MudBlazor;
using SharedLib;
using BlazorLib;

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


    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorPage ParentFormsPage { get; set; }


    ConfigManufactureComponent _conf = default!;

    EnumerationsManufactureComponent enumerations_ref = default!;

    const string DirectoryTypeName = nameof(DirectoryConstructorModelDB);
    const string DocumentSchemeConstructor = nameof(DocumentSchemeConstructorModelDB);

    /// <summary>
    /// Текущий проект
    /// </summary>
    public ProjectConstructorModelDB CurrentProject { get; private set; } = default!;
    /// <summary>
    /// Manufacture
    /// </summary>
    public ManageManufactureModelDB Manufacture { get; private set; } = default!;

    readonly List<string> _errors = [];
    /// <summary>
    /// Build еree вoneAction
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

        string get_sn = ParentFormsPage
        .SystemNamesManufacture
        .FirstOrDefault(x => x.TypeDataName == DirectoryTypeName)?.SystemName ?? GlobalTools.TranslitToSystemName(dir.Name);

        return new EnumFitModel()
        {
            SystemName = get_sn,
            Name = dir.Name,
            Description = dir.Description,
            EnumItems = dir.Elements.Select(e => new SortableFitModel()
            {
                Name = e.Name,
                SortIndex = e.SortIndex,
                Description = e.Description,
            }).ToArray()
        };
    }

    DocumentFitModel DocumentConvert(DocumentSchemeConstructorModelDB doc)
    {
        string get_sn = ParentFormsPage
        .SystemNamesManufacture
        .FirstOrDefault(x => x.TypeDataName == DocumentSchemeConstructor)?.SystemName ?? GlobalTools.TranslitToSystemName(doc.Name);

        return new DocumentFitModel()
        {
            SystemName = get_sn,
            Name = doc.Name,
            Description = doc.Description,
        };
    }

    void Download()
    {
        ArgumentNullException.ThrowIfNull(CurrentProject.Directories);
        ArgumentNullException.ThrowIfNull(CurrentProject.Documents);
        ArgumentNullException.ThrowIfNull(ParentFormsPage.MainProject);


        StructureProjectModel struct_project = new()
        {
            Enums = CurrentProject.Directories.Select(EnumConvert),
            Documents = CurrentProject.Documents.Select(DocumentConvert)
        };

        CodeGeneratorConfigModel conf_gen = Manufacture;

        GeneratorCSharpService gen = new(conf_gen, ParentFormsPage.MainProject);
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

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadProjectData();
    }
}