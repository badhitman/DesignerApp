////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using System.IO.Compression;
using System.Reflection;
using SharedLib.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text;
using SharedLib;

namespace BlazorLib;

/// <inheritdoc/>
public class GeneratorCSharpService(CodeGeneratorConfigModel conf, MainProjectViewModel project)
{
    static Dictionary<string, string> services_di = [];
    ZipArchive archive = default!;
    TResponseModel<Stream> _result = default!;

    #region main
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
            $"{conf.EnumDirectoryPath} - папка перечислений",
            $"",
            $"{conf.AccessDataDirectoryPath} - папка файлов сервисов backend служб доступа к данным (CRUD) и классов/моделей ответов",
            $"\t> crud_interfaces: интерфейсы низкоуровневого доступа к контексту таблиц базы данных",
            $"\t\t> crud_implementations: реализация интерфейсов crud_interfaces",
            $"······················································",
            $"\t> service_interfaces: интерфейсы функционального/промежуточного (между контроллером и низкоуровневым DB доступом) доступа к данным",
            $"\t\t> service_implementations: реализация интерфейсов service_interfaces",
            $"······················································",
            $"\tresponse_models: модели ответов контроллеров, которые в свою очередь получают их от функциональных/промежуточных служб доступа",
            $"××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××",
            $"",
            $"LayerContextPartGen.cs - разделяемый [public partial class LayerContext : DbContext] класс.",
            //$"refit_di.cs - [public static class RefitExtensionDesignerDI].[public static void BuildRefitServicesDI(this IServiceCollection services, ClientConfigModel conf, TimeSpan handler_lifetime)]",
            $"services_di.cs - [public static class ServicesExtensionDesignerDI].[public static void BuildDesignerServicesDI(this IServiceCollection services)]"
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
        await DbTableAccessGen(schema);
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

    async Task EnumerationsGeneration(IEnumerable<EnumFitModel> enumerations)
    {
        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        bool is_first_item;
        EntryTypeModel type_entry;

        foreach (EnumFitModel enum_obj in enumerations)
        {
            type_entry = GetZipEntryNameForEnumeration(enum_obj);
            zipEntry = archive.CreateEntry(type_entry.FullEntryName);
            writer = new(zipEntry.Open(), Encoding.UTF8);

            await WriteHead(writer, [$"{enum_obj.Name}"], enum_obj.Description);

            await writer.WriteLineAsync($"\tpublic enum {enum_obj.SystemName}");
            await writer.WriteLineAsync("\t{");
            is_first_item = true;

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

            await WriteEnd(writer);
        }
    }

    async Task<Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>> DocumentsGeneration(IEnumerable<DocumentFitModel> docs)
    {
        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>? schema = await WriteSchema(docs);
        if (!_result.Success())
            return schema;

        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        bool is_first_item = true;

        foreach (KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> kvp in schema)
        {
            zipEntry = archive.CreateEntry(kvp.Key.FullEntryName);
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, [kvp.Key.Name], kvp.Key.Description);
            await writer.WriteLineAsync($"\tpublic partial class {kvp.Key.TypeName} : SharedLib.IdSwitchableModel");
            await writer.WriteLineAsync("\t{");

            foreach (EntrySchemaTypeModel schema_obj in kvp.Value)
            {
                if (!is_first_item)
                    await writer.WriteLineAsync();
                else
                    is_first_item = false;

                await writer.WriteLineAsync($"\t\t/// <summary>");
                await writer.WriteLineAsync($"\t\t/// [tab: {schema_obj.Tab.Name}][form: {schema_obj.Form.Name}]");
                await writer.WriteLineAsync($"\t\t/// </summary>");

                if (schema_obj.IsTable)
                    await writer.WriteLineAsync($"\t\tpublic List<{schema_obj.TypeName}>? {schema_obj.TypeName}DataMultiSet {{ get; set; }}");
                else
                    await writer.WriteLineAsync($"\t\tpublic {schema_obj.TypeName}? {schema_obj.TypeName}DataSingleSet {{ get; set; }}");

            }

            await WriteEnd(writer);
        }
        return schema;
    }

    async Task<Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>>> WriteSchema(IEnumerable<DocumentFitModel> docs)
    {
        ZipArchiveEntry zipEntry;
        EntryDocumentTypeModel doc_entry;
        EntrySchemaTypeModel type_entry;
        Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> schema_data = [];

        foreach (DocumentFitModel doc_obj in docs)
        {
            doc_entry = GetDocumentZipEntry(doc_obj);
            List<EntrySchemaTypeModel> schema_inc = [];
            foreach (TabFitModel tab_obj in doc_obj.Tabs)
                foreach (FormFitModel form_obj in tab_obj.Forms)
                {
                    type_entry = GetSchemaZipEntry(form_obj, tab_obj, doc_obj);
                    EntrySchemaTypeModel? _check = schema_inc.FirstOrDefault(x => x.TypeName.Equals(type_entry.TypeName));
                    if (_check is not null)
                    {
                        _result.AddError($"Ошибка генерации схемы данных `{type_entry.TypeName}` [{type_entry.FullEntryName}] для документа '{doc_obj.Name}' ({doc_obj.SystemName}). ");
                        continue;
                    }


                    zipEntry = archive.CreateEntry(type_entry.FullEntryName);

                    using StreamWriter writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, [tab_obj.Name], tab_obj.Description, ["System.ComponentModel.DataAnnotations"]);

                    await writer.WriteLineAsync($"\tpublic partial class {tab_obj.SystemName}{doc_obj.SystemName}");
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

                    if (form_obj.SimpleFields is not null)
                        foreach (FieldFitModel _f in form_obj.SimpleFields)
                        {
                            await WriteField(_f, writer);
                        }
                    if (form_obj.FieldsAtDirectories is not null)
                        foreach (FieldAkaDirectoryFitModel _f in form_obj.FieldsAtDirectories)
                        {
                            await WriteField(_f, writer);
                        }

                    await WriteEnd(writer);
                    schema_inc.Add(type_entry);
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

    static async Task WriteField(FieldAkaDirectoryFitModel field, StreamWriter writer)
    {
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// {field.Name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\tpublic{(field.Required ? " required" : "")} {field.DirectorySystemName}{(field.Required ? "" : "?")} {field.SystemName} {{ get; set; }}");
    }


    async Task DbContextGen(Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> docs)
    {
        ZipArchiveEntry zipEntry = archive.CreateEntry("LayerContextPartGen.cs");
        StreamWriter _writer = new(zipEntry.Open(), Encoding.UTF8);
        await WriteHead(writer: _writer, summary_text: ["Database context"], using_ns: ["Microsoft.EntityFrameworkCore", conf.Namespace]);

        await _writer.WriteLineAsync("\tpublic partial class LayerContext : DbContext");
        await _writer.WriteLineAsync("\t{");
        bool is_first_document_item = true;
        foreach (KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> doc_obj in docs)
        {
            if (!is_first_document_item)
                await _writer.WriteLineAsync();
            else
                is_first_document_item = false;

            await _writer.WriteLineAsync("\t\t/// <summary>");
            await _writer.WriteLineAsync($"\t\t/// {doc_obj.Key.Name}");
            await _writer.WriteLineAsync("\t\t/// </summary>");

            if (!string.IsNullOrWhiteSpace(doc_obj.Key.Description))
            {
                await _writer.WriteLineAsync("\t\t/// <remarks>");
                await _writer.WriteLineAsync(string.Join("\n", DescriptionHtmlToLinesRemark(doc_obj.Key.Description).Select(x => $"\t\t/// {x}")));
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

                await _writer.WriteLineAsync("\t\t/// <summary>");
                await _writer.WriteLineAsync($"\t\t/// [{schema.Document.Name}]->[{schema.Tab.Name}]->[{schema.Form.Name}]");
                await _writer.WriteLineAsync("\t\t/// </summary>");
                _writer.WriteLine($"\t\tpublic DbSet<{schema.TypeName}> {schema.TypeName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");
            }
            await _writer.WriteLineAsync("#endregion");
        }

        await WriteEnd(_writer);
    }

    Task DbTableAccessGen(Dictionary<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> docs)
    {
        string crud_type_name, service_type_name, response_type_name, controller_name, service_instance;
        ZipArchiveEntry zipEntry;
        StreamWriter writer;

        foreach (KeyValuePair<EntryDocumentTypeModel, List<EntrySchemaTypeModel>> doc_obj in docs)
        {
            #region модели ответов тела документа (rest/api)

            response_type_name = $"{doc_obj.Key.TypeName}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, $"{doc_obj.SystemName} : Response model (single object)", ["SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : ResponseBaseModel");
            //await writer.WriteLineAsync("\t{");
            //await writer.WriteLineAsync("\t\t/// <summary>");
            //await writer.WriteLineAsync($"\t\t/// Результат запроса [{doc_obj.SystemName}] (полезная нагрузка)");
            //await writer.WriteLineAsync("\t\t/// </summary>");
            //await writer.WriteLineAsync($"\t\tpublic {doc_obj.SystemName}? {GlobalStaticConstants.RESULT_PROPERTY_NAME} {{ get; set; }}");
            //await WriteEnd(writer);


            //response_type_name = $"{doc_obj.SystemName}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, $"{doc_obj.SystemName} : Response model (collection objects)", ["SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : ResponseBaseModel");
            //await writer.WriteLineAsync("\t{");
            //await writer.WriteLineAsync("\t\t/// <summary>");
            //await writer.WriteLineAsync($"\t\t/// Результат запроса [{doc_obj.SystemName}] (полезная нагрузка)");
            //await writer.WriteLineAsync("\t\t/// </summary>");
            //await writer.WriteLineAsync($"\t\tpublic IEnumerable<{doc_obj.SystemName}> {GlobalStaticConstants.RESULT_PROPERTY_NAME} {{ get; set; }}");
            //await WriteEnd(writer);


            //response_type_name = $"{doc_obj.SystemName}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, $"{doc_obj.SystemName} : Response model (paginations collection of objects)", ["SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : FindResponseModel");
            //await writer.WriteLineAsync("\t{");
            //await writer.WriteLineAsync("\t\t/// <summary>");
            //await writer.WriteLineAsync($"\t\t/// Результат запроса [{doc_obj.SystemName}] (полезная нагрузка)");
            //await writer.WriteLineAsync("\t\t/// </summary>");
            //await writer.WriteLineAsync($"\t\tpublic IEnumerable<{doc_obj.SystemName}> DataRows {{ get; set; }}");
            //await WriteEnd(writer);

            #endregion

            #region тело документа

            //crud_type_name = $"I{doc_obj.SystemName}{GlobalStaticConstants.DATABASE_TABLE_ACESSOR_PREFIX}";
            //services_di.Add(crud_type_name, crud_type_name[1..]);
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "crud_interfaces", $"{crud_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name, ["SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial interface {crud_type_name} : SharedLib.ISavingChanges");
            //await writer.WriteLineAsync("\t{");

            //await WriteDocumentCrudInterface(writer, doc_obj.SystemName, doc_obj.Name, true);

            //crud_type_name = crud_type_name[1..];
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "crud_implementations", $"{crud_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, null, ["DbcLib", "Microsoft.EntityFrameworkCore", "SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial class {crud_type_name} : I{crud_type_name}");
            //await writer.WriteLineAsync("\t{");
            //await writer.WriteLineAsync("\t\treadonly DbAppContext _db_context;");
            //await writer.WriteLineAsync();
            //await writer.WriteLineAsync("\t\t/// <summary>");
            //await writer.WriteLineAsync("\t\t/// Конструктор");
            //await writer.WriteLineAsync("\t\t/// </summary>");
            //await writer.WriteLineAsync($"\t\tpublic {crud_type_name}(DbAppContext set_db_context)");
            //await writer.WriteLineAsync("\t\t{");
            //await writer.WriteLineAsync("\t\t\t_db_context = set_db_context;");
            //await writer.WriteLineAsync("\t\t}");
            //await writer.WriteLineAsync();

            //await WriteDocumentCrudInterfaceImplementation(writer, doc_obj.SystemName, true);


            //service_type_name = $"I{doc_obj.SystemName}{GlobalStaticConstants.SERVICE_ACESSOR_PREFIX}";
            //services_di.Add(service_type_name, service_type_name[1..]);
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "service_interfaces", $"{service_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name, ["SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial interface {service_type_name}");
            //await writer.WriteLineAsync("\t{");

            //await WriteDocumentServicesInterface(writer, doc_obj.SystemName, doc_obj.Name, true);

            //service_type_name = service_type_name[1..];
            //zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "service_implementations", $"{service_type_name}.cs"));
            //writer = new(zipEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project.Name, conf.Namespace, null, ["SharedLib.Models"]);

            //await writer.WriteLineAsync($"\tpublic partial class {service_type_name} : I{service_type_name}");
            //await writer.WriteLineAsync("\t{");
            //await writer.WriteLineAsync($"\t\treadonly I{crud_type_name} _crud_accessor;");
            //await writer.WriteLineAsync();
            //await writer.WriteLineAsync("\t\t/// <summary>");
            //await writer.WriteLineAsync("\t\t/// Конструктор");
            //await writer.WriteLineAsync("\t\t/// </summary>");
            //await writer.WriteLineAsync($"\t\tpublic {service_type_name}(I{crud_type_name} set_crud_accessor)");
            //await writer.WriteLineAsync("\t\t{");
            //await writer.WriteLineAsync("\t\t\t_crud_accessor = set_crud_accessor;");
            //await writer.WriteLineAsync("\t\t}");
            //await writer.WriteLineAsync();
            //await WriteDocumentServicesInterfaceImplementation(writer, doc_obj.SystemName, true);

            #endregion

            #region контроллеры тела документа

            //if (!string.IsNullOrWhiteSpace(conf.ControllersDirectoryPath))
            //{
            //    service_instance = $"_{service_type_name}".ToLower();
            //    controller_name = $"{doc_obj.SystemName}Controller";
            //    zipEntry = archive.CreateEntry(Path.Combine(conf.ControllersDirectoryPath, $"{controller_name}.cs"));
            //    writer = new(zipEntry.Open(), Encoding.UTF8);
            //    await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name, ["Microsoft.AspNetCore.Mvc", "SharedLib.Models"]);
            //    await writer.WriteLineAsync("\t[Route(\"api/[controller]\")]");
            //    await writer.WriteLineAsync("\t[ApiController]");
            //    await writer.WriteLineAsync($"\tpublic partial class {controller_name} : ControllerBase");
            //    await writer.WriteLineAsync("\t{");
            //    await writer.WriteLineAsync($"\t\treadonly I{service_type_name} {service_instance};");
            //    await writer.WriteLineAsync();
            //    await writer.WriteLineAsync($"\t\tpublic {controller_name}(I{service_type_name} set{service_instance})");
            //    await writer.WriteLineAsync("\t\t{");
            //    await writer.WriteLineAsync($"\t\t\t{service_instance} = set{service_instance};");
            //    await writer.WriteLineAsync("\t\t}");
            //    await writer.WriteLineAsync();
            //    await WriteDocumentControllers(writer, service_instance, doc_obj.SystemName, $"Документ: {doc_obj.Name}", true);
            //}

            #endregion

            #region refit/rest (тело документа)

            //rest_service_name = $"I{doc_obj.SystemName}RestService";
            //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, doc_obj.SystemName.ToLower(), $"{rest_service_name}.cs"));
            //writer = new(enumEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project_info.Name, project_info.NameSpace, $"REST служба работы с API -> {project_info.Name}", ["Refit", "SharedLib.Models"]);
            //await writer.WriteLineAsync($"\tpublic interface {rest_service_name}");
            //await writer.WriteLineAsync("\t{");
            //await WriteRestServiceInterface(writer, doc_obj.SystemName, doc_obj.Name, true, false, false);

            //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, doc_obj.SystemName.ToLower(), $"{rest_service_name[1..]}.cs"));
            //writer = new(enumEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project_info.Name, project_info.NameSpace, null, ["SharedLib.Models", "Refit", "Microsoft.Extensions.Logging"]);
            //await writer.WriteLineAsync($"\tpublic class {rest_service_name[1..]} : {rest_service_name}");
            //await writer.WriteLineAsync("\t{");
            //await WriteRestServiceImplementation(writer, doc_obj.SystemName, true);

            //rest_service_name = $"I{doc_obj.SystemName}RefitProvider";
            //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, doc_obj.SystemName.ToLower(), "core", $"{rest_service_name}.cs"));
            //writer = new(enumEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project_info.Name, project_info.NameSpace, $"Refit коннектор к API/{project_info.Name}", ["Refit", "SharedLib.Models"]);
            //await writer.WriteLineAsync($"\tpublic interface {rest_service_name}");
            //await writer.WriteLineAsync("\t{");
            //await WriteRestServiceInterface(writer, doc_obj.SystemName, doc_obj.Name, true, true, false);

            //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, doc_obj.SystemName.ToLower(), "core", $"{rest_service_name[1..]}.cs"));
            //writer = new(enumEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project_info.Name, project_info.NameSpace, null, ["SharedLib.Models", "Refit", "Microsoft.Extensions.Logging"]);
            //await writer.WriteLineAsync($"\tpublic class {rest_service_name[1..]} : {rest_service_name}");
            //await writer.WriteLineAsync("\t{");
            //await WriteRefitProviderImplementation(writer, doc_obj.SystemName, true);

            //rest_service_name = $"I{doc_obj.SystemName}RefitService";
            //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, doc_obj.SystemName.ToLower(), "core", $"{rest_service_name}.cs"));
            //writer = new(enumEntry.Open(), Encoding.UTF8);
            //await WriteHead(writer, project_info.Name, project_info.NameSpace, $"Refit коннектор к API/{project_info.Name}", ["Refit", "SharedLib.Models"]);
            //await writer.WriteLineAsync("\t[Headers(\"Content-Type: application/json\")]");
            //await writer.WriteLineAsync($"\tpublic interface {rest_service_name}");
            //await writer.WriteLineAsync("\t{");
            //await WriteRestServiceInterface(writer, doc_obj.SystemName, doc_obj.Name, true, true, true);

            #endregion
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Генерация файла регистрации DI служб
    /// </summary>
    Task GenServicesDI()
    {
        //ZipArchiveEntry readmeEntry = archive.CreateEntry("services_di.cs");
        //using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        //await WriteHead(writer, project.Name, null, "di services", [conf.Namespace]);
        //await writer.WriteLineAsync("\tpublic static class ServicesExtensionDesignerDI");
        //await writer.WriteLineAsync("\t{");
        //await writer.WriteLineAsync("\t\tpublic static void BuildDesignerServicesDI(this IServiceCollection services)");
        //await writer.WriteLineAsync("\t\t{");
        //foreach (KeyValuePair<string, string> kvp in services_di)
        //    await writer.WriteLineAsync($"\t\t\tservices.AddScoped<{kvp.Key}, {kvp.Value}>();");

        //await WriteEnd(writer);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Запись CRUD интерфейсов служб непосредственного доступа к данным (к таблицам БД)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="doc_name">Имя документа для комментариев</param>
    static async Task WriteDocumentCrudInterface(StreamWriter writer, string type_name, string doc_name)
    {
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Создать новый (запись БД): {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest\">Объект добавления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task AddAsync({type_name} obj_rest, bool auto_save = true);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Создать перечень новых объектов: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest_range\">Объекты добавления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task AddRangeAsync(IEnumerable<{type_name}> obj_rest_range, bool auto_save = true);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Прочитать: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<{type_name}?> FirstAsync(int id);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить (набор): {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<IEnumerable<{type_name}>> SelectAsync(IEnumerable<int> ids);");
        await writer.WriteLineAsync();

        //if (is_body_document)
        //{
        //    await writer.WriteLineAsync("\t\t/// <summary>");
        //    await writer.WriteLineAsync($"\t\t/// Получить (страницу){(is_body_document ? "" : " строк табличной части")} документов: {doc_name}");
        //    await writer.WriteLineAsync("\t\t/// </summary>");
        //    await writer.WriteLineAsync($"\t\t/// <param name=\"pagination_request\">Запрос-пагинатор</param>");
        //    await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(PaginationRequestModel pagination_request);");
        //    await writer.WriteLineAsync();
        //}
        //else
        //{
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить (набор) строк табличной части документа: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"pagination_request\">Запрос-пагинатор</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(GetByIdPaginationRequestModel pagination_request);");
        await writer.WriteLineAsync();
        //}

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить объект документа: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest\">Объект обновления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task UpdateAsync({type_name} obj_rest, bool auto_save = true);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить перечень объектов: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest_range\">Объекты обновления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task UpdateRangeAsync(IEnumerable<{type_name}> obj_rest_range, bool auto_save = true);");
        await writer.WriteLineAsync();


        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Инверсия признака удаления: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task MarkDeleteToggleAsync(int id, bool auto_save = true);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task RemoveAsync(int id, bool auto_save = true);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить перечень: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task RemoveRangeAsync(IEnumerable<int> ids, bool auto_save = true);");
        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись CRUD реализаций интерфейсов служб непосредственного доступа к данным (к таблицам БД)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    static async Task WriteDocumentCrudInterfaceImplementation(StreamWriter writer, string type_name)
    {
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task AddAsync({type_name} obj_rest, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\tawait _db_context.AddAsync(obj_rest);");
        await writer.WriteLineAsync("\t\t\tif (auto_save)");
        await writer.WriteLineAsync("\t\t\t\tawait SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task AddRangeAsync(IEnumerable<{type_name}> obj_range_rest, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\tawait _db_context.AddRangeAsync(obj_range_rest);");
        await writer.WriteLineAsync("\t\t\tif (auto_save)");
        await writer.WriteLineAsync("\t\t\t\tawait SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name}?> FirstAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await _db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.FindAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<IEnumerable<{type_name}>> SelectAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await _db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.Where(x => ids.Contains(x.Id)).ToArrayAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        //if (is_body_document)
        //{
        //    await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        //    await writer.WriteLineAsync($"\t\tpublic async Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(PaginationRequestModel request)");
        //    await writer.WriteLineAsync("\t\t{");
        //    await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        //    await writer.WriteLineAsync($"\t\t\tIQueryable<{type_name}>? query = _db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.AsQueryable();");
        //    await writer.WriteLineAsync($"\t\t\t{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX} result = new()");
        //    await writer.WriteLineAsync("\t\t\t{");
        //    await writer.WriteLineAsync("\t\t\t\tPagination = new PaginationResponseModel(request)");
        //    await writer.WriteLineAsync("\t\t\t\t{");
        //    await writer.WriteLineAsync("\t\t\t\t\tTotalRowsCount = await query.CountAsync()");
        //    await writer.WriteLineAsync("\t\t\t\t}");
        //    await writer.WriteLineAsync("\t\t\t};");

        //    await writer.WriteLineAsync($"\t\t\tswitch (result.Pagination.SortBy)");
        //    await writer.WriteLineAsync("\t\t\t{");
        //    await writer.WriteLineAsync("\t\t\t\tdefault:");
        //    await writer.WriteLineAsync("\t\t\t\t\tquery = result.Pagination.SortingDirection == VerticalDirectionsEnum.Up");
        //    await writer.WriteLineAsync("\t\t\t\t\t\t? query.OrderByDescending(x => x.Id)");
        //    await writer.WriteLineAsync("\t\t\t\t\t\t: query.OrderBy(x => x.Id);");
        //    await writer.WriteLineAsync("\t\t\t\t\tbreak;");
        //    await writer.WriteLineAsync("\t\t\t}");

        //    await writer.WriteLineAsync($"\t\t\tquery = query.Skip((result.Pagination.PageNum - 1) * result.Pagination.PageSize).Take(result.Pagination.PageSize);");
        //    await writer.WriteLineAsync("\t\t\tresult.DataRows = await query.ToArrayAsync();");
        //    await writer.WriteLineAsync("\t\t\treturn result;");
        //    await writer.WriteLineAsync("\t\t}");
        //    await writer.WriteLineAsync();
        //}
        //else
        //{
        string fk_owner_property_name = $"{type_name}OwnerId";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(GetByIdPaginationRequestModel request)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\tIQueryable<{type_name}>? query = _db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.Where(x => x.{fk_owner_property_name} == request.FilterId).AsQueryable();");
        await writer.WriteLineAsync($"\t\t\t{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX} result = new()");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tPagination = new PaginationResponseModel(request)");
        await writer.WriteLineAsync("\t\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\t\tTotalRowsCount = await query.CountAsync()");
        await writer.WriteLineAsync("\t\t\t\t}");
        await writer.WriteLineAsync("\t\t\t};");

        await writer.WriteLineAsync($"\t\t\tswitch (result.Pagination.SortBy)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tdefault:");
        await writer.WriteLineAsync("\t\t\t\t\tquery = result.Pagination.SortingDirection == VerticalDirectionsEnum.Up");
        await writer.WriteLineAsync("\t\t\t\t\t\t? query.OrderByDescending(x => x.Id)");
        await writer.WriteLineAsync("\t\t\t\t\t\t: query.OrderBy(x => x.Id);");
        await writer.WriteLineAsync("\t\t\t\t\tbreak;");
        await writer.WriteLineAsync("\t\t\t}");

        await writer.WriteLineAsync($"\t\t\tquery = query.Skip((result.Pagination.PageNum - 1) * result.Pagination.PageSize).Take(result.Pagination.PageSize);");
        await writer.WriteLineAsync("\t\t\tresult.DataRows = await query.ToArrayAsync();");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();
        //}

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task UpdateAsync({type_name} obj_rest, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\t_db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.Update(obj_rest);");
        await writer.WriteLineAsync("\t\t\tif (auto_save)");
        await writer.WriteLineAsync("\t\t\t\tawait SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task UpdateRangeAsync(IEnumerable<{type_name}> obj_range_rest, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\t_db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.UpdateRange(obj_range_rest);");
        await writer.WriteLineAsync("\t\t\tif (auto_save)");
        await writer.WriteLineAsync("\t\t\t\tawait SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task MarkDeleteToggleAsync(int id, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        string db_set_name = $"_db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}";
        await writer.WriteLineAsync($"\t\t\t{type_name} db_{type_name}_object = await {db_set_name}.FindAsync(id);");
        await writer.WriteLineAsync($"\t\t\tdb_{type_name}_object.IsDeleted = !db_{type_name}_object.IsDeleted;");
        await writer.WriteLineAsync($"\t\t\t{db_set_name}.Update(db_{type_name}_object);");
        await writer.WriteLineAsync("\t\t\tif (auto_save)");
        await writer.WriteLineAsync("\t\t\t\tawait SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task RemoveAsync(int id, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tawait RemoveRangeAsync(new int[] { id }, auto_save);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task RemoveRangeAsync(IEnumerable<int> ids, bool auto_save = true)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\t{db_set_name}.RemoveRange({db_set_name}.Where(x => ids.Contains(x.Id)));");
        await writer.WriteLineAsync("\t\t\tif (auto_save)");
        await writer.WriteLineAsync("\t\t\t\tawait SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync("\t\tpublic async Task<int> SaveChangesAsync(Dictionary<string, string?>? cashe_upd = null)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await _db_context.SaveChangesAsync();");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await WriteEnd(writer);
    }


    #region system
    /// <summary>
    /// Перечисления/Списки: Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    EntryTypeModel GetZipEntryNameForEnumeration(EnumFitModel enum_obj)
        => new(enum_obj.SystemName, conf.EnumDirectoryPath);

    /// <summary>
    /// Документы (схемы данных): Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    EntryDocumentTypeModel GetDocumentZipEntry(DocumentFitModel doc_obj)
        => new(doc_obj, conf.DocumentsMastersDbDirectoryPath);

    /// <summary>
    /// Схема данных: Путь относительно корня архива, указывающий имя создаваемой записи.
    /// </summary>
    EntrySchemaTypeModel GetSchemaZipEntry(FormFitModel form_obj, TabFitModel tab_obj, DocumentFitModel doc_obj)
        => new(form_obj, tab_obj, doc_obj, conf.DocumentsMastersDbDirectoryPath, "schema");

    /// <summary>
    /// HTML строку в обычную/нормальную (без тегов).
    /// например: для добавления в remarks
    /// </summary>
    static string[] DescriptionHtmlToLinesRemark(string html_description)
    {
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
    /// Записать вступление файла
    /// </summary>
    async Task WriteHead(StreamWriter writer, IEnumerable<string>? summary_text = null, string? description = null, IEnumerable<string>? using_ns = null)
    {
        await writer.WriteLineAsync("////////////////////////////////////////////////");
        await writer.WriteLineAsync($"// Project: {project.Name} - by  © https://github.com/badhitman - @fakegov");
        await writer.WriteLineAsync("////////////////////////////////////////////////");
        await writer.WriteLineAsync();

        if (using_ns?.Any() == true)
        {
            foreach (string u in using_ns)
                await writer.WriteLineAsync($"{(u.StartsWith("using ", StringComparison.CurrentCultureIgnoreCase) ? u : $"using {u}")}{(u.EndsWith(';') ? "" : ";")}");

            await writer.WriteLineAsync();
        }

        if (!string.IsNullOrWhiteSpace(conf.Namespace))
        {
            await writer.WriteLineAsync($"namespace {conf.Namespace}");
            await writer.WriteLineAsync("{");
        }
        string ns_pref = string.IsNullOrWhiteSpace(conf.Namespace) ? "" : "\t";

        if (summary_text?.Any() == true)
            await writer.WriteLineAsync($"{ns_pref}/// <summary>");

        await writer.WriteLineAsync($"{(summary_text?.Any() != true ? "<inheritdoc/>" : string.Join(Environment.NewLine, summary_text.Select(s => $"{ns_pref}/// {s.Trim()}")))}");

        if (summary_text?.Any() == true)
            await writer.WriteLineAsync($"{ns_pref}/// </summary>");

        if (!string.IsNullOrWhiteSpace(description))
        {
            await writer.WriteLineAsync($"{ns_pref}/// <remarks>");
            await writer.WriteLineAsync($"{string.Join(Environment.NewLine, DescriptionHtmlToLinesRemark(description).Select(r => $"{ns_pref}/// {r.Trim()}"))}");
            await writer.WriteLineAsync($"{ns_pref}/// </remarks>");
        }
    }

    /// <summary>
    /// Запись финальной части файла и закрытие потока записи
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    static async Task WriteEnd(StreamWriter writer)
    {
        await writer.WriteLineAsync("\t}");
        await writer.WriteAsync("}");
        await writer.FlushAsync();
        writer.Close();
        await writer.DisposeAsync();
    }
    #endregion    
}