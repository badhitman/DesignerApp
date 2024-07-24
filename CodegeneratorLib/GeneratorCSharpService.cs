////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using System.IO.Compression;
using System.Reflection;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text;
using SharedLib;

namespace CodegeneratorLib;

/// <inheritdoc/>
public class GeneratorCSharpService(CodeGeneratorConfigModel conf, MainProjectViewModel project)
{
    static Dictionary<string, string> services_di = [];
    ZipArchive archive = default!;
    TResponseModel<Stream> _result = default!;

    /// <summary>
    /// Формирование данных
    /// </summary>
    public async Task<TResponseModel<Stream>> GetZipArchive(StructureProjectModel dump)
    {
        _result = new();
        List<string> stat =
        [
            $"Перечислений: {dump.Enums.Length} (элементов всего: {dump.Enums.Sum(x => x.EnumItems.Length)})",
            $"Документов: {dump.Documents.Length} шт.",
            $"\tвкладок (всего): {dump.Documents.Sum(x => x.Tabs?.Length)}",
            $"\tформ (всего): {dump.Documents.SelectMany(x => x.Tabs).Sum(x => x.Forms?.Length)}",
            $"\tполей (всего): {dump.Documents.SelectMany(x => x.Tabs).SelectMany(x => x.Forms).Sum(x => x.SimpleFields?.Length)} [simple field`s] + {dump.Documents.SelectMany(x => x.Tabs).SelectMany(x => x.Forms).Sum(x => x.FieldsAtDirectories?.Length)} [enumerations field`s]",
            $"- ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ - ~ -",
            $"{conf.BlazorDirectoryPath}        - папка Blazor UI",
            $"{conf.EnumDirectoryPath}          - папка перечислений",
            $"{conf.AccessDataDirectoryPath}    - папка файлов сервисов backend служб доступа к данным (CRUD) и классов/моделей ответов: интерфейсы (+ реализация) доступа к контексту таблиц базы данных для использования их в UI",
            $"××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××",
            $"LayerContextPartGen.cs    - разделяемый [public partial class LayerContext : DbContext] класс.",
            $"services_di.cs            - [public static class ServicesExtensionDesignerDI].[public static void BuildDesignerServicesDI(this IServiceCollection services)]"
        ];
        services_di = [];

        using MemoryStream ms = new();
        archive = new(ms, ZipArchiveMode.Create);

        await ReadmeGen(stat);
        await EnumerationsGeneration(dump.Enums);

        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> schema = await DocumentsGeneration(dump.Documents);
        if (!_result.Success())
            return _result;


        await DbContextGen(schema);
        await DbTableAccessGeneration(schema);
        await GenServicesDI();


        string json_raw = JsonConvert.SerializeObject(dump, Formatting.Indented);
        await GenerateJsonDump(json_raw);

        archive.Dispose();
        services_di.Clear();

        if (!_result.Success())
            return _result;

        await ms.FlushAsync();

        _result.Response = new MemoryStream(ms.ToArray());
        return _result;
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
    async Task EnumerationsGeneration(IEnumerable<EnumFitModel> enumerations)
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

            await writer.WriteLineAsync($"\tpublic enum {enum_obj.SystemName}");
            await writer.WriteLineAsync("\t{");

            if (enum_obj.EnumItems is not null)
                foreach (SortableFitModel enum_item in enum_obj.EnumItems.OrderBy(x => x.SortIndex))
                {
                    if (!is_first_item)
                        await writer.WriteLineAsync();
                    else
                        is_first_item = false;

                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync($"\t\t/// {enum_item.Name}");
                    await writer.WriteLineAsync("\t\t/// </summary>");

                    if (!string.IsNullOrWhiteSpace(enum_item.Description))
                    {
                        await writer.WriteLineAsync("\t\t/// <remarks>");
                        await writer.WriteLineAsync($"{(string.Join(Environment.NewLine, DescriptionHtmlToLinesRemark(enum_item.Description).Select(r => $"\t\t/// {r}")))}");
                        await writer.WriteLineAsync("\t\t/// </remarks>");
                    }

                    await writer.WriteLineAsync($"\t\t{enum_item.SystemName},");
                }

            await WriteEndClass(writer);
        }
    }

    /// <summary>
    /// Документы (бизнес-сущности)
    /// </summary>
    async Task<Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>> DocumentsGeneration(IEnumerable<DocumentFitModel> docs)
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
            await writer.WriteLineAsync($"\tpublic partial class {kvp.Key.TypeName} : SharedLib.IdSwitchableModel");
            await writer.WriteLineAsync("\t{");

            foreach (EntrySchemaTypeModel schema_obj in kvp.Value)
            {
                if (!is_first_item)
                    await writer.WriteLineAsync();
                else
                    is_first_item = false;

                await writer.WriteLineAsync($"\t\t/// <summary>");
                await writer.WriteLineAsync($"\t\t/// [tab: {schema_obj.Tab.Name} `{schema_obj.Tab.SystemName}`][form: {schema_obj.Form.Name} `{schema_obj.Form.SystemName}`]");
                await writer.WriteLineAsync($"\t\t/// </summary>");

                if (schema_obj.IsTable)
                    await writer.WriteLineAsync($"\t\tpublic List<{schema_obj.TypeName}>? {schema_obj.TypeName}DataMultiSet {{ get; set; }}");
                else
                    await writer.WriteLineAsync($"\t\tpublic {schema_obj.TypeName}? {schema_obj.TypeName}DataSingleSet {{ get; set; }}");

            }

            await WriteEndClass(writer);
        }
        return schema;
    }

    /// <summary>
    /// Схема данных (модели)
    /// </summary>
    async Task<Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>> WriteModelsSchema(IEnumerable<DocumentFitModel> docs)
    {
        ZipArchiveEntry zipEntry;
        EntryDocumentTypeModel doc_entry;
        EntrySchemaTypeModel form_type_entry;
        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> schema_data = [];
        BlazorCodeGenerator blazorCode = new();
        StreamWriter writer;

        foreach (DocumentFitModel doc_obj in docs)
        {
            doc_entry = GetDocumentObjectZipEntry(doc_obj);
            blazorCode.Set(doc_entry);

            zipEntry = archive.CreateEntry(doc_entry.BlazorFullEntryName());
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await writer.WriteLineAsync(blazorCode.GetView());

            if (conf.BlazorSplitFiles)
            {
                await writer.DisposeAsync();

                zipEntry = archive.CreateEntry($"{doc_entry.BlazorFullEntryName()}.cs");
                writer = new(zipEntry.Open(), Encoding.UTF8);
                await writer.WriteAsync(blazorCode.GetCode(!conf.BlazorSplitFiles));
            }
            else
            {
                await writer.WriteLineAsync();
                await writer.WriteAsync(blazorCode.GetCode(!conf.BlazorSplitFiles));
            }

            await writer.DisposeAsync();
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

                    await writer.WriteLineAsync($"\tpublic partial class {form_type_entry.TypeName}");
                    await writer.WriteLineAsync("\t{");
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync("\t\t/// Идентификатор/Key");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync("\t\t[Key]");
                    await writer.WriteLineAsync("\t\tpublic int Id { get; set; }");

                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync("\t\t/// [FK: Документ]");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync("\t\tpublic int DocumentId { get; set; }");
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync("\t\t/// Документ");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync($"\t\tpublic {doc_entry.TypeName}? Document {{ get; set; }}");

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

                                await WriteHeadClass(writer, [tab_obj.Name], tab_obj.Description, ["System.ComponentModel.DataAnnotations"]);
                                await writer.WriteLineAsync($"\tpublic partial class {field.DirectorySystemName}Multiple{form_type_entry.TypeName}");
                                await writer.WriteLineAsync("\t{");
                                await writer.WriteLineAsync("\t\t/// <summary>");
                                await writer.WriteLineAsync("\t\t/// Идентификатор/Key");
                                await writer.WriteLineAsync("\t\t/// </summary>");
                                await writer.WriteLineAsync("\t\t[Key]");
                                await writer.WriteLineAsync("\t\tpublic int Id { get; set; }");

                                await writer.WriteLineAsync();
                                await writer.WriteLineAsync("\t\t/// <summary>");
                                await writer.WriteLineAsync("\t\t/// [FK: Форма в табе]");
                                await writer.WriteLineAsync("\t\t/// </summary>");
                                await writer.WriteLineAsync("\t\tpublic int OwnerId { get; set; }");
                                await writer.WriteLineAsync("\t\t/// <summary>");
                                await writer.WriteLineAsync("\t\t/// Форма в табе");
                                await writer.WriteLineAsync("\t\t/// </summary>");
                                await writer.WriteLineAsync($"\t\tpublic {form_type_entry.TypeName}? Owner {{ get; set; }}");

                                await writer.WriteLineAsync();
                                await writer.WriteLineAsync("\t\t/// <summary>");
                                await writer.WriteLineAsync("\t\t/// ");
                                await writer.WriteLineAsync("\t\t/// </summary>");
                                await writer.WriteLineAsync($"\t\tpublic {field.DirectorySystemName} {field.SystemName} {{ get; set; }}");

                                await WriteEndClass(writer);
                            }
                        }
                    }

                    schema_inc.Add(form_type_entry);

                    string blazorComponentEntryName = form_type_entry.BlazorFormFullEntryName();
                    if (!formsCache.Contains(blazorComponentEntryName))
                    {
                        blazorCode.Set(form_type_entry);

                        zipEntry = archive.CreateEntry(form_type_entry.BlazorFormFullEntryName());
                        writer = new(zipEntry.Open(), Encoding.UTF8);
                        await writer.WriteLineAsync(blazorCode.GetView());

                        if (conf.BlazorSplitFiles)
                        {
                            await writer.DisposeAsync();
                            zipEntry = archive.CreateEntry($"{form_type_entry.BlazorFormFullEntryName()}.cs");
                            writer = new(zipEntry.Open(), Encoding.UTF8);
                            await writer.WriteAsync(blazorCode.GetCode(!conf.BlazorSplitFiles));
                        }
                        else
                        {
                            await writer.WriteLineAsync();
                            await writer.WriteAsync(blazorCode.GetCode(!conf.BlazorSplitFiles));
                        }

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
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// {field.Name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\tpublic{(field.Required ? " required" : "")} {field.TypeData}{(field.Required ? "" : "?")} {field.SystemName} {{ get; set; }}");
    }
    static async Task WriteField(FieldAkaDirectoryFitModel field, EntrySchemaTypeModel form_type_entry, StreamWriter writer)
    {
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// {field.Name}");
        await writer.WriteLineAsync("\t\t/// </summary>");

        if (field.IsMultiSelect)
            await writer.WriteLineAsync($"\t\tpublic List<{field.DirectorySystemName}Multiple{form_type_entry.Form.SystemName}{form_type_entry.Tab.SystemName}{form_type_entry.Document.SystemName}>? {field.SystemName} {{ get; set; }}");
        else
            await writer.WriteLineAsync($"\t\tpublic{(field.Required ? " required" : "")} {field.DirectorySystemName}{(field.Required ? "" : "?")} {field.SystemName} {{ get; set; }}");
    }

    async Task DbContextGen(Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> docs)
    {
        ZipArchiveEntry zipEntry = archive.CreateEntry("LayerContextPartGen.cs");
        StreamWriter _writer = new(zipEntry.Open(), Encoding.UTF8);
        await WriteHeadClass(writer: _writer, summary_text: ["Database context"], using_ns: ["Microsoft.EntityFrameworkCore"]);

        await _writer.WriteLineAsync("\tpublic partial class LayerContext : DbContext");
        await _writer.WriteLineAsync("\t{");
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
                await _writer.WriteLineAsync("\t\t/// <remarks>");
                await _writer.WriteLineAsync(string.Join(Environment.NewLine, DescriptionHtmlToLinesRemark(doc_obj.Key.Document.Description).Select(x => $"\t\t/// {x}")));
                await _writer.WriteLineAsync("\t\t/// </remarks>");
            }
            _writer.WriteLine($"\t\tpublic DbSet<{doc_obj.Key.TypeName}> {doc_obj.Key.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");

            bool is_first_schema_item = true;
            await _writer.WriteLineAsync();
            await _writer.WriteLineAsync($"#region schema for [{doc_obj.Key.TypeName}]");
            foreach (EntrySchemaTypeModel schema in doc_obj.Value)
            {
                if (!is_first_schema_item)
                    await _writer.WriteLineAsync();
                else
                    is_first_schema_item = false;

                await _writer.WriteLineAsync(sb.UseSummaryText([$"[{schema.Document.Name}]->[{schema.Tab.Name}]->[{schema.Form.Name}]"]).SummaryGet);
                _writer.WriteLine($"\t\tpublic DbSet<{schema.TypeName}> {schema.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");
            }
            await _writer.WriteLineAsync("#endregion");
        }

        await WriteEndClass(_writer);
    }

    async Task DbTableAccessGeneration(Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> docs)
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

            await writer.WriteLineAsync($"\tpublic partial interface I{type_entry.TypeName}");
            await writer.WriteLineAsync("\t{");

            List<ServiceMethodBuilder> builder = await WriteInterface(writer, doc_obj);

            zipEntry = archive.CreateEntry(type_entry.FullEntryName());
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHeadClass(writer, [doc_obj.Key.Document.Name], null, ["Microsoft.EntityFrameworkCore", "SharedLib"]);

            await writer.WriteLineAsync($"\tpublic partial class {type_entry.TypeName}(IDbContextFactory<LayerContext> appDbFactory) : I{type_entry.TypeName}");
            await writer.WriteLineAsync("\t{");

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
    static async Task<List<ServiceMethodBuilder>> WriteInterface(StreamWriter writer, KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> doc_obj)
    {
        List<ServiceMethodBuilder> builders_history = [];
        ServiceMethodBuilder builder = new() { TabulationsSpiceSize = 2 };

        #region main
        writer.WriteLine("\t\t#region main");
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

        builders_history.Add(builder
           .UseSummaryText($"Обновить перечень объектов: '{doc_obj.Key.Document.Name}'")
           .UseParameter(new("obj_range", $"IEnumerable<{doc_obj.Key.TypeName}>", "Объекты обновления в БД"))
           .UsePayload($"_db_context.Update(obj_range);")
           .AddPayload("await _db_context.SaveChangesAsync();")
           .Extract<ServiceMethodBuilder>()
           .WriteSignatureMethod(writer, "UpdateAsync"));
        writer.WriteLine();

        builders_history.Add(builder
           .UseSummaryText($"Удалить перечень объектов: '{doc_obj.Key.Document.Name}'")
           .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
           .UsePayload($"await {db_set_name}.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();")
           .Extract<ServiceMethodBuilder>()
           .WriteSignatureMethod(writer, "RemoveAsync"));

        builders_history.Add(builder
           .UseSummaryText($"Инверсия признака деактивации объекта: '{doc_obj.Key.Document.Name}'")
           .UseParameter(new("ids", "IEnumerable<int>", "Идентификаторы объектов"))
           .AddParameter(new("set_mark", "bool", "Признак, который следует установить"))
           .UsePayload($"await {db_set_name}.Where(x => ids.Contains(x.Id)).ExecuteUpdateAsync(b => b.SetProperty(u => u.IsDisabled, set_mark));")
           .Extract<ServiceMethodBuilder>()
           .WriteSignatureMethod(writer, "MarkDeleteToggleAsync"));

        writer.WriteLine("\t\t#endregion");
        #endregion

        #region ext tables parts
        EntrySchemaTypeModel[] tables = doc_obj.Value.Where(x => x.IsTable).ToArray();
        if (tables.Length != 0)
        {
            writer.WriteLine();
            writer.WriteLine("\t\t#region tables parts");

            foreach (EntrySchemaTypeModel table_schema in tables)
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

            writer.WriteLine("\t\t#endregion");
        }
        #endregion

        await WriteEndClass(writer);
        return builders_history;
    }

    /// <summary>
    /// Генерация файла регистрации DI служб
    /// </summary>
    async Task GenServicesDI()
    {
        ZipArchiveEntry readmeEntry = archive.CreateEntry("services_di.cs");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        await WriteHeadClass(writer, [project.Name], "di services", ["Microsoft.Extensions.DependencyInjection"]);
        await writer.WriteLineAsync("\tpublic static class ServicesExtensionDesignerDI");
        await writer.WriteLineAsync("\t{");
        await writer.WriteLineAsync("\t\t/// Регистрация сервисов");
        await writer.WriteLineAsync("\t\tpublic static void BuildDesignerServicesDI(this IServiceCollection services)");
        await writer.WriteLineAsync("\t\t{");
        foreach (KeyValuePair<string, string> kvp in services_di)
            await writer.WriteLineAsync($"\t\t\tservices.AddScoped<{kvp.Key}, {kvp.Value}>();");
        await writer.WriteLineAsync("\t\t}");

        await WriteEndClass(writer);
    }
    #endregion

    #region tools
    /// <summary>
    /// Перечисления/Списки: Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    EntryTypeModel GetZipEntryNameForEnumeration(EnumFitModel enum_obj)
        => new(enum_obj.SystemName, conf.EnumDirectoryPath);

    /// <summary>
    /// Документы (схемы данных): Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    EntryDocumentTypeModel GetDocumentObjectZipEntry(DocumentFitModel doc_obj)
        => new(doc_obj, conf.DocumentsMastersDbDirectoryPath, conf.BlazorDirectoryPath);

    /// <summary>
    /// Схема данных: Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    EntrySchemaTypeModel GetSchemaZipEntry(FormFitModel form_obj, TabFitModel tab_obj, DocumentFitModel doc_obj)
        => new(form_obj, tab_obj, doc_obj, conf.DocumentsMastersDbDirectoryPath, conf.BlazorDirectoryPath, "schema");

    EntryTypeModel GetZipEntryNameForDbTableAccess(EntryDocumentTypeModel doc_obj)
        => new($"{doc_obj.TypeName}{GlobalStaticConstants.DATABASE_TABLE_ACESSOR_PREFIX}", conf.AccessDataDirectoryPath);


    /// <summary>
    /// HTML строку в обычную/нормальную (без тегов).
    /// например: для добавления в remarks
    /// </summary>
    static string[] DescriptionHtmlToLinesRemark(string html_description)
    {
        if (string.IsNullOrWhiteSpace(html_description))
            return [];

        HtmlDocument doc = new();
        doc.LoadHtml(html_description
            .Replace("&nbsp;", " ")
            .Replace("  ", " ")
            .Replace("</p><p>", $"</p>{Environment.NewLine}<p>")
            .Replace("</br>", $"</br>{Environment.NewLine}")
            );
        return doc.DocumentNode.InnerText.Split(new string[] { Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }


    /// <summary>
    /// Записать вступление файла (для класса)
    /// </summary>
    async Task WriteHeadClass(StreamWriter writer, IEnumerable<string>? summary_text = null, string? description = null, IEnumerable<string>? using_ns = null)
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
            await writer.WriteLineAsync($"\t/// <summary>");

        await writer.WriteLineAsync($"{(summary_text?.Any() != true ? "\t/// <inheritdoc/>" : string.Join(Environment.NewLine, summary_text.Select(s => $"\t/// {s.Trim()}")))}");

        if (summary_text?.Any() == true)
            await writer.WriteLineAsync($"\t/// </summary>");

        if (!string.IsNullOrWhiteSpace(description))
        {
            await writer.WriteLineAsync($"/// <remarks>");
            await writer.WriteLineAsync($"{string.Join(Environment.NewLine, DescriptionHtmlToLinesRemark(description).Select(r => $"/// {r.Trim()}"))}");
            await writer.WriteLineAsync($"/// </remarks>");
        }
    }

    /// <summary>
    /// Запись финальной части файла и закрытие потока записи (для класса)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    static async Task WriteEndClass(StreamWriter writer)
    {
        await writer.WriteLineAsync("\t}");
        await writer.WriteAsync("}");
        await writer.DisposeAsync();
    }
    #endregion    
}