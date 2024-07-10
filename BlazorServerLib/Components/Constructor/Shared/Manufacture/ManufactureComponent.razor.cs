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
    DocumentsManufactureComponent documents_ref = default!;

    const string DirectoryTypeName = nameof(DirectoryConstructorModelDB);
    const string DocumentSchemeConstructorTypeName = nameof(DocumentSchemeConstructorModelDB);
    const string ElementOfDirectoryConstructorTypeName = nameof(ElementOfDirectoryConstructorModelDB);

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

        TreeItemDataModel? tree_item = enumerations_ref.TreeItems.FirstOrDefault(x => x.Value?.Id == dir.Id) as TreeItemDataModel;
        ArgumentNullException.ThrowIfNull(tree_item?.Children);

        return new EnumFitModel()
        {
            SystemName = tree_item.SystemName ?? GlobalTools.TranslitToSystemName(dir.Name),
            Name = dir.Name,
            Description = dir.Description,
            EnumItems = dir.Elements.Select(e =>
            {

                tree_item = tree_item.Children.FirstOrDefault(x => x.Value?.Id == e.Id) as TreeItemDataModel;
                ArgumentNullException.ThrowIfNull(tree_item?.Children);

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

    BaseFitModel DocumentConvert(DocumentSchemeConstructorModelDB doc)
    {
        TreeItemDataModel? tree_item = documents_ref.TreeItems.FirstOrDefault(x => x.Value?.Id == doc.Id) as TreeItemDataModel;
        ArgumentNullException.ThrowIfNull(tree_item);

        return new BaseFitModel()
        {
            SystemName = tree_item.SystemName ?? GlobalTools.TranslitToSystemName(doc.Name),
            Name = doc.Name,
            Description = doc.Description,
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

    /// <inheritdoc/>
    public override void StateHasChangedCall()
    {
        enumerations_ref.StateHasChangedCall();
        documents_ref.StateHasChangedCall();
        base.StateHasChangedCall();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await ReloadProjectData();
    }
}