////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using System.IO.Compression;
using System.Reflection;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text;
using SharedLib;
using HtmlGenerator.html5;

namespace CodegeneratorLib;

/// <inheritdoc/>
public class GeneratorCSharpService(CodeGeneratorConfigModel conf, MainProjectViewModel project)
{
    Dictionary<string, string> services_di = [];
    readonly Dictionary<string, string> routes = [];
    ZipArchive archive = default!;
    TResponseModel<Stream> _result = default!;
    static string Tab => base_dom_root.TabString;

    /// <summary>
    /// Формирование данных
    /// </summary>
    public virtual async Task<TResponseModel<Stream>> GetZipArchive(StructureProjectModel dump)
    {
        _result = new();
        List<string> stat =
        [
            $"Перечислений: {dump.Enums.Length} (элементов всего: {dump.Enums.Sum(x => x.EnumItems.Length)})",
            $"Документов: {dump.Documents.Length} шт.",
            $"{base_dom_root.TabString}вкладок (всего): {dump.Documents.Sum(x => x.Tabs?.Count)}",
            $"{base_dom_root.TabString}форм (всего): {dump.Documents.SelectMany(x => x.Tabs).Sum(x => x.Forms?.Length)}",
            $"{base_dom_root.TabString}полей (всего): {dump.Documents.SelectMany(x => x.Tabs).SelectMany(x => x.Forms).Sum(x => x.SimpleFields?.Length)} [simple field`s] + {dump.Documents.SelectMany(x => x.Tabs).SelectMany(x => x.Forms).Sum(x => x.FieldsAtDirectories?.Length)} [enumerations field`s]",
            $"- ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ -",
            $"{conf.BlazorDirectoryPath}        - папка Blazor UI",
            $"{conf.EnumDirectoryPath}          - папка перечислений",
            $"{conf.AccessDataDirectoryPath}    - папка файлов сервисов backend служб доступа к данным (CRUD) и классов/моделей ответов: интерфейсы (+ реализация) доступа к контексту таблиц базы данных для использования их в UI",
            $"××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××",
            $"Вспомогательные файлы (зависимости типов): ",
            $"{base_dom_root.TabString}> {conf.DocumentsMastersDbDirectoryPath}/SelectJournalPartRequestModel.cs",
            $"{base_dom_root.TabString}> {conf.DocumentsMastersDbDirectoryPath}/IJournalUniversalService.cs",
            $"{base_dom_root.TabString}> {conf.BlazorDirectoryPath}/JournalUniversalComponent.razor (+ отдельный JournalUniversalComponent.razor.cs)",
            $"××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××",
            $"bottom-menu.Development.json  - навигация по созданным страницам" +
            $"LayerContextPartGen.cs        - разделяемый [public partial class LayerContext : DbContext] класс.",
            $"services_di.cs                - [public static class ServicesExtensionDesignerDI].[public static void BuildDesignerServicesDI(this IServiceCollection services)]"
        ];

        services_di = [];

        using MemoryStream ms = new();
        archive = new(ms, ZipArchiveMode.Create);

        await ReadmeGen(stat);
        await EnumerationsGeneration(dump.Enums);

        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> schema = await DocumentsGeneration(dump.Documents);
        if (!_result.Success())
            return _result;

        await WriteDemoBottomMenu();

        await DbContextGen(schema);
        await DbTableAccessGeneration(schema);
        await GenServicesDI();
        await WriteResources();

        string json_raw = JsonConvert.SerializeObject(dump, Formatting.Indented);
        await GenerateJsonDump(json_raw);

        archive.Dispose();

        services_di.Clear();
        routes.Clear();

        if (!_result.Success())
            return _result;

        await ms.FlushAsync();

        _result.Response = new MemoryStream(ms.ToArray());
        return _result;
    }

    private async Task WriteDemoBottomMenu()
    {
        if (routes.Count == 0)
            return;

        ZipArchiveEntry readmeEntry = archive.CreateEntry("bottom-menu.Development.json");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);

        string ds = $"{Tab}{Tab}";
        await writer.WriteLineAsync("{");
        await writer.WriteLineAsync($"{Tab}\"NavMenuConfig\": {{");
        await writer.WriteLineAsync($"{ds}\"BottomNavMenuItems\": [");

        foreach (KeyValuePair<string, string> route in routes)
        {
            await writer.WriteLineAsync($"{ds}{Tab}{{");
            await writer.WriteLineAsync($"{ds}{ds}\"Title\": \"{route.Value}\",");
            await writer.WriteLineAsync($"{ds}{ds}\"HrefNav\": \"{route.Key}\"");
            await writer.WriteLineAsync($"{ds}{Tab}}}");
            await writer.WriteLineAsync($"{ds}");
        }

        await writer.WriteLineAsync($"{ds}]");
        await writer.WriteLineAsync($"{Tab}}}");
        await writer.WriteLineAsync("}");
    }

    #region help
    async Task ReadmeGen(IEnumerable<string> stat)
    {
        string app_version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        ZipArchiveEntry readmeEntry = archive.CreateEntry("Readme.txt");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        await writer.WriteLineAsync($"Генератор C# комплекта - ver: {app_version} (by © https://github.com/badhitman - @fakegov)");
        await writer.WriteLineAsync($"'{project.Name}' `{conf.Namespace}`");
        await writer.WriteLineAsync($"============ {DateTime.Now} ============");
        await writer.WriteLineAsync();
        //
        foreach (string row_line in stat)
            await writer.WriteLineAsync(row_line);
    }

    async Task WriteResources()
    {
        using (Stream s = archive.CreateEntry($"{conf.DocumentsMastersDbDirectoryPath}/IJournalUniversalService.cs").Open())
            await s.WriteAsync(Properties.Resources.IJournalUniversalService_cs);

        using (Stream s = archive.CreateEntry($"{conf.DocumentsMastersDbDirectoryPath}/SelectJournalPartRequestModel.cs").Open())
            await s.WriteAsync(Properties.Resources.SelectJournalPartRequestModel_cs);

        using (Stream s = archive.CreateEntry($"{conf.BlazorDirectoryPath}/JournalUniversalComponent.razor").Open())
            await s.WriteAsync(Properties.Resources.JournalUniversalComponent_razor);

        using (Stream s = archive.CreateEntry($"{conf.BlazorDirectoryPath}/JournalUniversalComponent.razor.cs").Open())
            await s.WriteAsync(Properties.Resources.JournalUniversalComponent_razor_cs);
    }

    /// <summary>
    /// Сгенерировать дамп данных в формате JSON
    /// </summary>
    /// <param name="json_raw">json данные для записи</param>
    async Task GenerateJsonDump(string json_raw)
    {
        ZipArchiveEntry readmeEntry = archive.CreateEntry("dump.json");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        await writer.WriteLineAsync(json_raw);
    }
    #endregion

    #region backend
    /// <summary>
    /// Справочники/перечисления
    /// </summary>
    public virtual async Task EnumerationsGeneration(IEnumerable<EnumFitModel> enumerations)
    {
        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        EntryTypeModel type_entry;

        foreach (EnumFitModel enum_obj in enumerations)
        {
            bool is_first_item = true;
            type_entry = GetZipEntryNameForEnumeration(enum_obj);
            zipEntry = archive.CreateEntry(type_entry.FullEntryName());
            writer = new(zipEntry.Open(), Encoding.UTF8);

            await WriteHeadClass(writer, [$"{enum_obj.Name}"], enum_obj.Description);

            await writer.WriteLineAsync($"{Tab}public enum {enum_obj.SystemName}");
            await writer.WriteLineAsync($"{Tab}{{");

            if (enum_obj.EnumItems is not null)
                foreach (SortableFitModel enum_item in enum_obj.EnumItems.OrderBy(x => x.SortIndex))
                {
                    if (!is_first_item)
                        await writer.WriteLineAsync();
                    else
                        is_first_item = false;

                    await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// {enum_item.Name}");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");

                    if (!string.IsNullOrWhiteSpace(enum_item.Description))
                    {
                        await writer.WriteLineAsync($"{Tab}{Tab}/// <remarks>");
                        await writer.WriteLineAsync($"{(string.Join(Environment.NewLine, GlobalTools.DescriptionHtmlToLinesRemark(enum_item.Description).Select(r => $"{Tab}{Tab}/// {r}")))}");
                        await writer.WriteLineAsync($"{Tab}{Tab}/// </remarks>");
                    }

                    await writer.WriteLineAsync($"{Tab}{Tab}{enum_item.SystemName},");
                }

            await WriteEndClass(writer);
        }
    }

    /// <summary>
    /// Документы (бизнес-сущности)
    /// </summary>
    public virtual async Task<Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>> DocumentsGeneration(IEnumerable<DocumentFitModel> docs)
    {
        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>? schema = await WriteModelsSchema(docs);
        if (!_result.Success())
            return schema;

        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        bool is_first_item = true;

        foreach (KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> kvp in schema)
        {
            zipEntry = archive.CreateEntry(kvp.Key.FullEntryName());
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHeadClass(writer, [$"Документ: '{kvp.Key.Document.Name}'"], kvp.Key.Document.Description);
            await writer.WriteLineAsync($"{Tab}public partial class {kvp.Key.TypeName} : SharedLib.IdSwitchableModel");
            await writer.WriteLineAsync($"{Tab}{{");

            foreach (EntrySchemaTypeModel schema_obj in kvp.Value)
            {
                if (!is_first_item)
                    await writer.WriteLineAsync();
                else
                    is_first_item = false;

                await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                await writer.WriteLineAsync($"{Tab}{Tab}/// [tab: {schema_obj.Tab.Name} `{schema_obj.Tab.SystemName}`][form: {schema_obj.Form.Name} `{schema_obj.Form.SystemName}`]");
                await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");

                if (schema_obj.IsTable)
                    await writer.WriteLineAsync($"{Tab}{Tab}public List<{schema_obj.TypeName}>? {schema_obj.TypeName}DataMultiSet {{ get; set; }}");
                else
                    await writer.WriteLineAsync($"{Tab}{Tab}public {schema_obj.TypeName}? {schema_obj.TypeName}DataSingleSet {{ get; set; }}");

            }

            await WriteEndClass(writer);
        }
        return schema;
    }

    /// <summary>
    /// Схема данных (модели)
    /// </summary>
    public virtual async Task<Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>> WriteModelsSchema(IEnumerable<DocumentFitModel> docs)
    {
        ZipArchiveEntry zipEntry;
        EntryDocumentTypeModel doc_entry;
        EntrySchemaTypeModel form_type_entry;
        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> schema_data = [];
        BlazorCodeGenerator blazorCode = new() { Config = conf };
        StreamWriter writer;

        foreach (DocumentFitModel doc_obj in docs)
        {
            doc_entry = GetDocumentObjectZipEntry(doc_obj);

            #region edit document
            blazorCode.SetEditDocument(doc_entry,
                [new ParameterComponentModel("Id", "int", "Идентификатор объекта-документа. Если null - тогда создание нового")
                {
                    IsCascading = false,
                    IsSupplyParameterFromQuery = false,
                    ParameterMode = ParameterModes.Nullable
                }]);

            zipEntry = archive.CreateEntry(doc_entry.BlazorFormFullEntryName());
            writer = new(zipEntry.Open(), Encoding.UTF8);
            string _route = $"/{GlobalTools.PascalToKebabCase(doc_obj.SystemName)}/edit/";

            routes.Add(_route, doc_obj.Name);

            await writer.WriteLineAsync(blazorCode.GetView([$"@page \"{_route}{{int?:Id}}\""]));

            if (conf.BlazorSplitFiles)
            {
                await writer.DisposeAsync();

                zipEntry = archive.CreateEntry($"{doc_entry.BlazorFormFullEntryName()}.cs");
                writer = new(zipEntry.Open(), Encoding.UTF8);
            }
            else
                await writer.WriteLineAsync();

            await writer.WriteAsync(blazorCode.GetCode(!conf.BlazorSplitFiles));

            await writer.DisposeAsync();
            #endregion

            HashSet<string> formsCache = [];

            List<EntrySchemaTypeModel> schema_inc = [];
            foreach (TabFitModel tab_obj in doc_obj.Tabs)
                foreach (FormFitModel form_obj in tab_obj.Forms)
                {
                    form_type_entry = GetSchemaZipEntry(form_obj, tab_obj, doc_obj);
                    EntrySchemaTypeModel? _check = schema_inc.FirstOrDefault(x => x.TypeName.Equals(form_type_entry.TypeName));
                    if (_check is not null)
                    {
                        _result.AddError($"Ошибка генерации схемы данных `{form_type_entry.TypeName}` [{form_type_entry.FullEntryName}] для документа '{doc_obj.Name}' ({doc_obj.SystemName}). ");
                        continue;
                    }

                    zipEntry = archive.CreateEntry(form_type_entry.FullEntryName());
                    writer = new(zipEntry.Open(), Encoding.UTF8);

                    await WriteHeadClass(writer, [tab_obj.Name], tab_obj.Description, ["System.ComponentModel.DataAnnotations"]);

                    await writer.WriteLineAsync($"{Tab}public partial class {form_type_entry.TypeName}");
                    await writer.WriteLineAsync($"{Tab}{{");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// Идентификатор/Key");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}[Key]");
                    await writer.WriteLineAsync($"{Tab}{Tab}public int Id {{ get; set; }}");

                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// [FK: Документ]");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}public int DocumentId {{ get; set; }}");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// Документ");
                    await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                    await writer.WriteLineAsync($"{Tab}{Tab}public {doc_entry.TypeName}? Document {{ get; set; }}");

                    if (form_obj.SimpleFields?.Length > 0)
                        foreach (FieldFitModel _f in form_obj.SimpleFields)
                            await WriteField(_f, writer);

                    if (form_obj.FieldsAtDirectories?.Length > 0)
                    {
                        foreach (FieldAkaDirectoryFitModel _f in form_obj.FieldsAtDirectories)
                            await WriteField(_f, form_type_entry, writer);

                        await WriteEndClass(writer);

                        FieldAkaDirectoryFitModel[] _fields_multi_select = form_obj
                            .FieldsAtDirectories
                            .Where(x => x.IsMultiSelect)
                            .ToArray();

                        if (_fields_multi_select.Length != 0)
                        {
                            foreach (FieldAkaDirectoryFitModel field in _fields_multi_select)
                            {
                                zipEntry = archive.CreateEntry(form_type_entry.FullEntryName($"{field.DirectorySystemName}Multiple"));
                                writer = new(zipEntry.Open(), Encoding.UTF8);
                                //
                                await WriteHeadClass(writer, [$"[doc: '{doc_obj.Name}' `{doc_obj.SystemName}`] [tab: '{tab_obj.Name}' `{tab_obj.SystemName}`] [form: '{form_obj.Name}' `{form_obj.SystemName}`] [field: '{field.Name}' `{field.SystemName}`]"], null, ["System.ComponentModel.DataAnnotations", "Microsoft.EntityFrameworkCore"]);
                                await writer.WriteLineAsync($"{Tab}[Index(nameof(OwnerId), nameof({field.SystemName}), IsUnique = true)]");
                                await writer.WriteLineAsync($"{Tab}public partial class {field.DirectorySystemName}Multiple{form_type_entry.TypeName}");
                                await writer.WriteLineAsync($"{Tab}{{");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// Идентификатор/Key");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}[Key]");
                                await writer.WriteLineAsync($"{Tab}{Tab}public int Id {{ get; set; }}");

                                await writer.WriteLineAsync();
                                await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// [FK: Форма в табе]");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}public int OwnerId {{ get; set; }}");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// Форма в табе");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}public {form_type_entry.TypeName}? Owner {{ get; set; }}");

                                await writer.WriteLineAsync();
                                await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// {field.Name}");
                                await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                                await writer.WriteLineAsync($"{Tab}{Tab}public {field.DirectorySystemName} {field.SystemName} {{ get; set; }}");

                                await WriteEndClass(writer);
                            }
                        }
                    }

                    schema_inc.Add(form_type_entry);

                    string blazorComponentEntryName = form_type_entry.BlazorFormFullEntryName();
                    if (!formsCache.Contains(blazorComponentEntryName))
                    {
                        blazorCode.SetForm(form_type_entry);

                        zipEntry = archive.CreateEntry(form_type_entry.BlazorFormFullEntryName());
                        writer = new(zipEntry.Open(), Encoding.UTF8);
                        await writer.WriteLineAsync(blazorCode.GetView());

                        if (conf.BlazorSplitFiles)
                        {
                            await writer.DisposeAsync();
                            zipEntry = archive.CreateEntry($"{form_type_entry.BlazorFormFullEntryName()}.cs");
                            writer = new(zipEntry.Open(), Encoding.UTF8);
                        }
                        else
                            await writer.WriteLineAsync();

                        await writer.WriteAsync(blazorCode.GetCode(!conf.BlazorSplitFiles));
                        await writer.DisposeAsync();

                        formsCache.Add(blazorComponentEntryName);
                    }
                }

            schema_data.Add(doc_entry, schema_inc);
        }

        return schema_data;
    }
    static async Task WriteField(FieldFitModel field, StreamWriter writer)
    {
        await writer.WriteLineAsync();
        await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
        await writer.WriteLineAsync($"{Tab}{Tab}/// {field.Name}");
        await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
        await writer.WriteLineAsync($"{Tab}{Tab}public{(field.Required ? " required" : "")} {field.TypeData}{(field.Required ? "" : "?")} {field.SystemName} {{ get; set; }}");
    }
    static async Task WriteField(FieldAkaDirectoryFitModel field, EntrySchemaTypeModel form_type_entry, StreamWriter writer)
    {
        await writer.WriteLineAsync();
        await writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
        await writer.WriteLineAsync($"{Tab}{Tab}/// {field.Name}");
        await writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");

        if (field.IsMultiSelect)
            await writer.WriteLineAsync($"{Tab}{Tab}public List<{field.DirectorySystemName}Multiple{form_type_entry.Form.SystemName}{form_type_entry.Tab.SystemName}{form_type_entry.Document.SystemName}>? {field.SystemName} {{ get; set; }}");
        else
            await writer.WriteLineAsync($"{Tab}{Tab}public{(field.Required ? " required" : "")} {field.DirectorySystemName}{(field.Required ? "" : "?")} {field.SystemName} {{ get; set; }}");
    }

    /// <summary>
    /// DbContext
    /// </summary>
    public virtual async Task DbContextGen(Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> docs)
    {
        ZipArchiveEntry zipEntry = archive.CreateEntry("LayerContextPartGen.cs");
        StreamWriter _writer = new(zipEntry.Open(), Encoding.UTF8);
        await WriteHeadClass(writer: _writer, summary_text: ["Database context"], using_ns: ["Microsoft.EntityFrameworkCore"]);

        await _writer.WriteLineAsync($"{Tab}public partial class LayerContext : DbContext");
        await _writer.WriteLineAsync($"{Tab}{{");
        bool is_first_document_item = true;
        foreach (KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> doc_obj in docs)
        {
            if (!is_first_document_item)
                await _writer.WriteLineAsync();
            else
                is_first_document_item = false;

            ServiceMethodBuilder sb = new() { TabulationsSpiceSize = 2 };

            await _writer.WriteLineAsync(sb.UseSummaryText([$"{doc_obj.Key.Document.Name}"]).SummaryGet);
            if (!string.IsNullOrWhiteSpace(doc_obj.Key.Document.Description))
            {
                await _writer.WriteLineAsync($"{Tab}{Tab}/// <remarks>");
                await _writer.WriteLineAsync(string.Join(Environment.NewLine, GlobalTools.DescriptionHtmlToLinesRemark(doc_obj.Key.Document.Description).Select(x => $"{Tab}{Tab}/// {x}")));
                await _writer.WriteLineAsync($"{Tab}{Tab}/// </remarks>");
            }
            _writer.WriteLine($"{Tab}{Tab}public DbSet<{doc_obj.Key.TypeName}> {doc_obj.Key.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");

            bool is_first_schema_item = true;
            await _writer.WriteLineAsync();
            await _writer.WriteLineAsync("#region schema");
            foreach (EntrySchemaTypeModel schema in doc_obj.Value)
            {
                if (!is_first_schema_item)
                    await _writer.WriteLineAsync();
                else
                    is_first_schema_item = false;

                await _writer.WriteLineAsync(sb.UseSummaryText([schema.Route]).SummaryGet);
                await _writer.WriteLineAsync($"{Tab}{Tab}public DbSet<{schema.TypeName}> {schema.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");

                IEnumerable<FieldAkaDirectoryFitModel>? fieldsAtDirectories = schema
                        .Form
                        .FieldsAtDirectories?
                        .Where(x => x.IsMultiSelect);

                if (fieldsAtDirectories?.Any() == true)
                {
                    await _writer.WriteLineAsync();
                    foreach (FieldAkaDirectoryFitModel? _fd in fieldsAtDirectories)
                    {
                        await _writer.WriteLineAsync($"{Tab}{Tab}/// <summary>");
                        await _writer.WriteLineAsync($"{Tab}{Tab}/// MULTISELECT ENUMERATIONS: {schema.Route} ['{_fd.Name}' `{_fd.SystemName}`]");
                        await _writer.WriteLineAsync($"{Tab}{Tab}/// </summary>");
                        await _writer.WriteLineAsync($"{Tab}{Tab}public DbSet<{_fd.DirectorySystemName}Multiple{schema.TypeName}> {_fd.DirectorySystemName}Multiple{schema.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");
                    }
                }
            }
            await _writer.WriteLineAsync("#endregion");
        }

        await WriteEndClass(_writer);
    }

    /// <summary>
    /// Службы
    /// </summary>
    public virtual async Task DbTableAccessGeneration(Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> docs)
    {
        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        EntryTypeModel type_entry;

        foreach (KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> doc_obj in docs)
        {
            type_entry = GetZipEntryNameForDbTableAccess(doc_obj.Key);

            services_di.Add($"I{type_entry.TypeName}", type_entry.TypeName);
            zipEntry = archive.CreateEntry(type_entry.FullEntryName("I"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHeadClass(writer, [doc_obj.Key.Document.Name], null, ["SharedLib"]);

            await writer.WriteLineAsync($"{Tab}public partial interface I{type_entry.TypeName}");
            await writer.WriteLineAsync($"{Tab}{{");

            List<ServiceMethodBuilder> builder = await WriteInterface(writer, doc_obj);

            zipEntry = archive.CreateEntry(type_entry.FullEntryName());
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHeadClass(writer, [doc_obj.Key.Document.Name], null, ["Microsoft.EntityFrameworkCore", "SharedLib"]);

            await writer.WriteLineAsync($"{Tab}public partial class {type_entry.TypeName}(IDbContextFactory<LayerContext> appDbFactory) : I{type_entry.TypeName}");
            await writer.WriteLineAsync($"{Tab}{{");

            int i = builder.Count;
            builder.ForEach(sb =>
            {
                sb.WriteImplementation(writer);
                if (--i > 0)
                    writer.WriteLine();
            });

            await WriteEndClass(writer);
        }
    }

    /// <summary>
    /// Запись CRUD интерфейсов служб непосредственного доступа к данным (к таблицам БД)
    /// </summary>
    public virtual async Task<List<ServiceMethodBuilder>> WriteInterface(StreamWriter writer, KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> doc_obj)
    {
        List<ServiceMethodBuilder> builders_history = [];
        ServiceMethodBuilder builder = new() { TabulationsSpiceSize = 2 };

        #region main
        writer.WriteLine($"{Tab}{Tab}#region main");
        string db_set_name = $"_db_context.{doc_obj.Key.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}";

        builders_history.Add(builder
            .UseSummaryText($"Создать перечень новых объектов: '{doc_obj.Key.Document.Name}'")
            .UseParameter(new("obj_range", $"IEnumerable<{doc_obj.Key.TypeName}>", "Объекты добавления в БД"))
            .UsePayload(["await _db_context.AddRangeAsync(obj_range);", "await _db_context.SaveChangesAsync();"])
            .Extract<ServiceMethodBuilder>()
            .WriteSignatureMethod(writer, "AddAsync"));
        writer.WriteLine();

        builders_history.Add(builder
            .UseSummaryText($"Прочитать перечень объектов: '{doc_obj.Key.Document.Name}'")
            .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
            .UsePayload($"return await {db_set_name}.Where(x => ids.Contains(x.Id)).ToListAsync();")
            .Extract<ServiceMethodBuilder>()
            .WriteSignatureMethod(writer, "ReadAsync", $"List<{doc_obj.Key.TypeName}>"));
        writer.WriteLine();

        builders_history.Add(builder
            .UseSummaryText($"Получить (порцию/pagination) объектов: '{doc_obj.Key.Document.Name}'")
            .UseParameter(new("pagination_request", "PaginationRequestModel", "Запрос-пагинатор"))
            .AddPaginationPayload(doc_obj.Key.TypeName, db_set_name)
            .AddPayload("return result;")
            .Extract<ServiceMethodBuilder>()
            .WriteSignatureMethod(writer, "SelectAsync", $"TPaginationResponseModel<{doc_obj.Key.TypeName}>"));
        writer.WriteLine();

        //builders_history.Add(builder
        //   .UseSummaryText($"Обновить перечень объектов: '{doc_obj.Key.Document.Name}'")
        //   .UseParameter(new("obj_range", $"IEnumerable<{doc_obj.Key.TypeName}>", "Объекты обновления в БД"))
        //   .UsePayload($"_db_context.Update(obj_range);")
        //   .AddPayload("await _db_context.SaveChangesAsync();")
        //   .Extract<ServiceMethodBuilder>()
        //   .WriteSignatureMethod(writer, "UpdateAsync"));
        //writer.WriteLine();

        builders_history.Add(builder
           .UseSummaryText($"Удалить перечень объектов: '{doc_obj.Key.Document.Name}'")
           .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
           .UsePayload($"await {db_set_name}.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();")
           .Extract<ServiceMethodBuilder>()
           .WriteSignatureMethod(writer, "RemoveAsync"));
        writer.WriteLine();

        builders_history.Add(builder
           .UseSummaryText($"Инверсия признака деактивации объекта: '{doc_obj.Key.Document.Name}'")
           .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
           .AddParameter(new("set_mark", "bool", "Признак, который следует установить"))
           .UsePayload($"await {db_set_name}.Where(x => ids.Contains(x.Id)).ExecuteUpdateAsync(b => b.SetProperty(u => u.IsDisabled, set_mark));")
           .Extract<ServiceMethodBuilder>()
           .WriteSignatureMethod(writer, "MarkDeleteToggleAsync"));

        writer.WriteLine($"{Tab}{Tab}#endregion");
        #endregion

        EntrySchemaTypeModel[] ext_source;
        #region ext part: tables
        ext_source = doc_obj.Value.Where(x => x.IsTable).ToArray();
        if (ext_source.Length != 0)
        {
            writer.WriteLine();
            writer.WriteLine($"{Tab}{Tab}#region tables");

            foreach (EntrySchemaTypeModel table_schema in ext_source)
            {
                db_set_name = $"_db_context.{table_schema.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}";

                builders_history.Add(builder
                    .UseSummaryText($"Создать перечень новых объектов: '{table_schema.Tab.Name}' - '{table_schema.Form.Name}'")
                    .UseParameter(new("obj_range", $"IEnumerable<{table_schema.TypeName}>", "Объекты добавления в БД"))
                    .UsePayload(["await _db_context.AddRangeAsync(obj_range);", "await _db_context.SaveChangesAsync();"])
                    .Extract<ServiceMethodBuilder>()
                    .WriteSignatureMethod(writer, $"Add{table_schema.TypeName}Async"));
                writer.WriteLine();

                builders_history.Add(builder
                    .UseSummaryText($"Прочитать перечень объектов: '{table_schema.Tab.Name}' - '{table_schema.Form.Name}'")
                    .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
                    .UsePayload($"return await {db_set_name}.Where(x => ids.Contains(x.Id)).ToListAsync();")
                    .Extract<ServiceMethodBuilder>()
                    .WriteSignatureMethod(writer, $"Read{table_schema.TypeName}Async", $"List<{table_schema.TypeName}>"));
                writer.WriteLine();

                builders_history.Add(builder
                    .UseSummaryText($"Получить (порцию/pagination) объектов: '{table_schema.Tab.Name}' - '{table_schema.Form.Name}'")
                    .UseParameter(new("pagination_request", "PaginationRequestModel", "Запрос-пагинатор"))
                    .AddPaginationPayload(table_schema.TypeName, db_set_name)
                    .AddPayload("return result;")
                    .Extract<ServiceMethodBuilder>()
                    .WriteSignatureMethod(writer, $"Select{table_schema.TypeName}Async", $"TPaginationResponseModel<{table_schema.TypeName}>"));
                writer.WriteLine();

                builders_history.Add(builder
                   .UseSummaryText($"Обновить перечень объектов: '{table_schema.Tab.Name}' - '{table_schema.Form.Name}'")
                   .UseParameter(new("obj_range", $"IEnumerable<{table_schema.TypeName}>", "Объекты обновления в БД"))
                   .UsePayload($"_db_context.Update(obj_range);")
                   .AddPayload("await _db_context.SaveChangesAsync();")
                   .Extract<ServiceMethodBuilder>()
                   .WriteSignatureMethod(writer, $"Update{table_schema.TypeName}Async"));
                writer.WriteLine();

                builders_history.Add(builder
                   .UseSummaryText($"Удалить перечень объектов: '{table_schema.Tab.Name}' - '{table_schema.Form.Name}'")
                   .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
                   .UsePayload($"await {db_set_name}.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();")
                   .Extract<ServiceMethodBuilder>()
                   .WriteSignatureMethod(writer, $"Remove{table_schema.TypeName}Async"));
            }

            writer.WriteLine($"{Tab}{Tab}#endregion");
        }
        #endregion

        /*#region ext part: enumerations multiselect
        ext_source = doc_obj.Value.Where(x => x.Form.FieldsAtDirectories?.Any(y => y.IsMultiSelect) == true).ToArray();
        if (ext_source.Length != 0)
        {
            writer.WriteLine();
            writer.WriteLine($"{_tab}{_tab}#region enumerations multiselect");

            foreach (EntrySchemaTypeModel _schema in ext_source)
                foreach (FieldAkaDirectoryFitModel _field in _schema.Form.FieldsAtDirectories!.Where(x => x.IsMultiSelect))
                {
                    //db_set_name = $"_db_context.{_field.SystemName}Multiple{_schema.Form.SystemName}{_schema.Tab.SystemName}{_schema.Document.SystemName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}";

                    //builders_history.Add(builder
                    //.UseSummaryText($"Прочитать перечень объектов: '{_schema.Tab.Name}' - '{_schema.Form.Name}'")
                    //.UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
                    //.UsePayload($"return await {db_set_name}.Where(x => ids.Contains(x.Id)).ToListAsync();")
                    //.Extract<ServiceMethodBuilder>()
                    //.WriteSignatureMethod(writer, $"Read{_schema.TypeName}Async", $"List<{_schema.TypeName}>"));
                    //writer.WriteLine();
                }

            writer.WriteLine($"{_tab}{_tab}#endregion");
        }
        #endregion*/

        await WriteEndClass(writer);
        return builders_history;
    }

    /// <summary>
    /// Генерация файла регистрации DI служб
    /// </summary>
    public virtual async Task GenServicesDI()
    {
        ZipArchiveEntry readmeEntry = archive.CreateEntry("services_di.cs");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        await WriteHeadClass(writer, [project.Name], "di services", ["Microsoft.Extensions.DependencyInjection"]);
        await writer.WriteLineAsync($"{Tab}public static class ServicesExtensionDesignerDI");
        await writer.WriteLineAsync($"{Tab}{{");
        await writer.WriteLineAsync($"{Tab}{Tab}/// Регистрация сервисов");
        await writer.WriteLineAsync($"{Tab}{Tab}public static void BuildDesignerServicesDI(this IServiceCollection services)");
        await writer.WriteLineAsync($"{Tab}{Tab}{{");
        foreach (KeyValuePair<string, string> kvp in services_di)
            await writer.WriteLineAsync($"{Tab}{Tab}{Tab}services.AddScoped<{kvp.Key}, {kvp.Value}>();");
        await writer.WriteLineAsync($"{Tab}{Tab}}}");

        await WriteEndClass(writer);
    }
    #endregion

    #region tools
    /// <summary>
    /// Перечисления/Списки: Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    public virtual EntryTypeModel GetZipEntryNameForEnumeration(EnumFitModel enum_obj)
        => new(enum_obj.SystemName, conf.EnumDirectoryPath);

    /// <summary>
    /// Документы (схемы данных): Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    public virtual EntryDocumentTypeModel GetDocumentObjectZipEntry(DocumentFitModel doc_obj)
        => new(doc_obj, conf.DocumentsMastersDbDirectoryPath, conf.BlazorDirectoryPath);

    /// <summary>
    /// Схема данных: Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    public virtual EntrySchemaTypeModel GetSchemaZipEntry(FormFitModel form_obj, TabFitModel tab_obj, DocumentFitModel doc_obj)
        => new(form_obj, tab_obj, doc_obj, conf.DocumentsMastersDbDirectoryPath, conf.BlazorDirectoryPath, "schema");

    /// <summary>
    /// DB TableAccess
    /// </summary>
    public virtual EntryTypeModel GetZipEntryNameForDbTableAccess(EntryDocumentTypeModel doc_obj)
        => new($"{doc_obj.TypeName}{GlobalStaticConstants.DATABASE_TABLE_ACESSOR_PREFIX}", conf.AccessDataDirectoryPath);



    /// <summary>
    /// Записать вступление файла (для класса)
    /// </summary>
    public virtual async Task WriteHeadClass(StreamWriter writer, IEnumerable<string>? summary_text = null, string? description = null, IEnumerable<string>? using_ns = null)
    {
        await writer.WriteLineAsync("////////////////////////////////////////////////");
        await writer.WriteLineAsync($"// Project: '{project.Name}' by  © https://github.com/badhitman - @fakegov");
        await writer.WriteLineAsync("////////////////////////////////////////////////");
        await writer.WriteLineAsync();

        if (using_ns?.Any() == true)
        {
            foreach (string u in using_ns)
                await writer.WriteLineAsync($"{(u.StartsWith("using ") ? u : $"using {u}")}{(u.EndsWith(';') ? "" : ";")}");

            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync($"namespace {conf.Namespace}");
        await writer.WriteLineAsync("{");

        if (summary_text?.Any() == true)
            await writer.WriteLineAsync($"{Tab}/// <summary>");

        await writer.WriteLineAsync($"{(summary_text?.Any() != true ? $"{Tab}/// <inheritdoc/>" : string.Join(Environment.NewLine, summary_text.Select(s => $"{Tab}/// {s.Trim()}")))}");

        if (summary_text?.Any() == true)
            await writer.WriteLineAsync($"{Tab}/// </summary>");

        if (!string.IsNullOrWhiteSpace(description))
        {
            await writer.WriteLineAsync($"{Tab}/// <remarks>");
            await writer.WriteLineAsync($"{Tab}{string.Join(Environment.NewLine, GlobalTools.DescriptionHtmlToLinesRemark(description).Select(r => $"/// {r.Trim()}"))}");
            await writer.WriteLineAsync($"{Tab}/// </remarks>");
        }
    }

    /// <summary>
    /// Запись финальной части файла и закрытие потока записи (для класса)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    public virtual async Task WriteEndClass(StreamWriter writer)
    {
        await writer.WriteLineAsync($"{Tab}}}");
        await writer.WriteAsync("}");
        await writer.DisposeAsync();
    }
    #endregion    
}