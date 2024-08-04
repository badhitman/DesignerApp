namespace SharedLib;

public partial interface IJournalUniversalService
{
    /// <summary>
    /// EnumConvert
    /// </summary>
    public static EnumFitModel EnumConvert(DirectoryConstructorModelDB dir, List<SystemNameEntryModel> systemNamesManufacture)
    {
        ArgumentNullException.ThrowIfNull(dir.Elements);
        //
        return new EnumFitModel()
        {
            SystemName = systemNamesManufacture.GetSystemName(dir.Id, dir.GetType().Name) ?? GlobalTools.TranslitToSystemName(dir.Name),
            Name = dir.Name,
            Description = dir.Description,
            EnumItems = dir.Elements.Count < 1 ? [] : dir.Elements.Select(e =>
            {
                return new SortableFitModel()
                {
                    SystemName = systemNamesManufacture.GetSystemName(e.Id, e.GetType().Name, null) ?? GlobalTools.TranslitToSystemName(e.Name),
                    Name = e.Name,
                    SortIndex = e.SortIndex,
                    Description = e.Description,
                };
            }).ToArray()
        };
    }

    /// <summary>
    /// DocumentConvert
    /// </summary>
    public static DocumentFitModel DocumentConvert(DocumentSchemeConstructorModelDB doc, List<SystemNameEntryModel> systemNamesManufacture)
    {
        ArgumentNullException.ThrowIfNull(doc.Tabs);

        TabFitModel TabConvert(TabOfDocumentSchemeConstructorModelDB tab)
        {
            ArgumentNullException.ThrowIfNull(tab.JoinsForms);
            FormFitModel FormConvert(FormToTabJoinConstructorModelDB joinForm)
            {
                ArgumentNullException.ThrowIfNull(joinForm.Form);
                FieldFitModel FieldConvert(FieldFormConstructorModelDB field)
                {
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
                        SystemName = systemNamesManufacture.GetSystemName(field.Id, $"{doc.GetType().Name}#{doc.Id} {tab.GetType().Name}#{tab.Id} {joinForm.Form.GetType().Name}#{joinForm.Form.Id} {nameof(FieldFormBaseLowConstructorModel)}", field.GetType().Name) ?? GlobalTools.TranslitToSystemName(field.Name),
                    };
                }

                FieldAkaDirectoryFitModel FieldAkaDirectoryConvert(FieldFormAkaDirectoryConstructorModelDB field)
                {
                    ArgumentNullException.ThrowIfNull(field.Directory?.Elements);
                    return new FieldAkaDirectoryFitModel()
                    {
                        DirectorySystemName = systemNamesManufacture.GetSystemName(field.Directory.Id, $"", field.GetType().Name) ?? GlobalTools.TranslitToSystemName(field.Directory!.Name),
                        Items = [.. field.Directory.Elements.Cast<EntryModel>()],
                        Name = field.Name,
                        SortIndex = field.SortIndex,
                        SystemName = systemNamesManufacture.GetSystemName(field.Id, $"{doc.GetType().Name}#{doc.Id} {tab.GetType().Name}#{tab.Id} {joinForm.Form.GetType().Name}#{joinForm.Form.Id} {nameof(FieldFormBaseLowConstructorModel)}", field.GetType().Name) ?? GlobalTools.TranslitToSystemName(field.Name),
                        Css = field.Css,
                        Description = field.Description,
                        Hint = field.Hint,
                        Required = field.Required,
                        IsMultiSelect = field.IsMultiSelect,
                    };
                }

                return new FormFitModel()
                {
                    Name = joinForm.Form.Name,
                    Css = joinForm.Form.Css,
                    Description = joinForm.Form.Description,
                    SortIndex = joinForm.SortIndex,
                    SystemName = systemNamesManufacture.GetSystemName(joinForm.Form.Id, $"{doc.GetType().Name}#{doc.Id} {tab.GetType().Name}#{tab.Id} {joinForm.Form.GetType().Name}") ?? GlobalTools.TranslitToSystemName(joinForm.Form.Name), // form_tree_item.SystemName,
                    IsTable = joinForm.IsTable,
                    Title = !joinForm.ShowTitle ? null : string.IsNullOrWhiteSpace(joinForm.Name) ? joinForm.Form.Name : joinForm.Name,
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
                SystemName = systemNamesManufacture.GetSystemName(tab.Id, $"{doc.GetType().Name}#{doc.Id} {tab.GetType().Name}") ?? GlobalTools.TranslitToSystemName(tab.Name), // tab_tree_item.SystemName,
                Forms = [.. tab.JoinsForms.Select(FormConvert)],
            };
        }

        return new DocumentFitModel()
        {
            SystemName = systemNamesManufacture.GetSystemName(doc.Id, doc.GetType().Name) ?? GlobalTools.TranslitToSystemName(doc.Name),
            Name = doc.Name,
            Description = doc.Description,
            Tabs = [.. doc.Tabs!.Select(TabConvert)]
        };
    }

    /// <summary>
    /// Получить все свои документы:
    /// </summary>
    /// <remarks>
    /// имя документа и идентификатор проекта, которому принадлежит этот документ (а так же - имя проекта в Tag)
    /// </remarks>
    public Task<EntryAltTagModel[]> GetMyDocumentsSchemas();

    /// <summary>
    /// Найти схемы документов по имени (или номеру)
    /// </summary>
    public Task<TResponseModel<DocumentSchemeConstructorModelDB[]?>> FindDocumentSchemes(string document_name_or_id, int? projectId);

    /// <summary>
    /// Прочитать данные сессии ограниченые одной вкладкой/табом
    /// </summary>
    public Task<ValueDataForSessionOfDocumentModelDB[]> ReadSessionTabValues(int tabId, int sessionId);
}