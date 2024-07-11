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

    /// <summary>
    /// Формирование данных
    /// </summary>
    public async Task<Stream> GetZipArchive(StructureProjectModel dump)
    {
        List<string> stat =
        [
            $"Перечислений: {dump.Enums.Length} (элементов всего: {dump.Enums.Sum(x => x.EnumItems.Length)})",
            //$"Документов: {dump.Documents.Count()} (полей всего: {dump.Documents.Sum(x => x.PropertiesBody?.Count()) + dump.Documents.Sum(x => x.Grids?.SelectMany(y => y.Properties!).Count())})",
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

        MemoryStream ms = new();
        archive = new(ms, ZipArchiveMode.Create);

        await ReadmeGen(stat);
        await EnumsGen(dump.Enums);
        //await DocumentsShemaGen(dump.Documents);
        //await DbContextGen(dump.Documents);
        //await DbTableAccessGen(dump.Documents);
        //await GenServicesDI();

        string json_raw = JsonConvert.SerializeObject(dump, Formatting.Indented);
        await GenerateJsonDump(json_raw);

        await ms.FlushAsync();
        services_di.Clear();
        archive.Dispose();

        return new MemoryStream(ms.ToArray());
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
    /// Записать вступление файла
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="project_name">Имя проекта</param>
    /// <param name="name_space">Пространство имён проекта</param>
    /// <param name="type_name">Имя типа данных для комментария (если NULL, то документация наследуется: <inheritdoc/>)</param>
    /// <param name="using_ns">Подключаемые пространства имён (using)</param>
    static async Task WriteHead(StreamWriter writer, string project_name, string? name_space, string? type_name, IEnumerable<string>? using_ns = null)
    {
        await writer.WriteLineAsync("////////////////////////////////////////////////");
        await writer.WriteLineAsync($"// Project: {project_name} - by  © https://github.com/badhitman - @fakegov");
        await writer.WriteLineAsync("////////////////////////////////////////////////");
        await writer.WriteLineAsync();

        if (using_ns?.Any() == true)
        {
            foreach (string u in using_ns)
                await writer.WriteLineAsync($"{(u.StartsWith("using ", StringComparison.CurrentCultureIgnoreCase) ? u : $"using {u}")}{(u.EndsWith(';') ? "" : ";")}");

            await writer.WriteLineAsync();
        }
        bool ns_is_empty = string.IsNullOrWhiteSpace(name_space);
        if (!ns_is_empty)
        {
            await writer.WriteLineAsync($"namespace {name_space}");
            await writer.WriteLineAsync("{");
        }
        string ns_pref = ns_is_empty ? "" : "\t";
        if (type_name is not null)
            await writer.WriteLineAsync($"{ns_pref}/// <summary>");

        await writer.WriteLineAsync($"{ns_pref}/// {(type_name is null ? "<inheritdoc/>" : type_name)}");

        if (type_name is not null)
            await writer.WriteLineAsync($"{ns_pref}/// </summary>");
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

    async Task EnumsGen(IEnumerable<EnumFitModel> enums)
    {
        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        bool is_first_item;
        foreach (EnumFitModel enum_obj in enums)
        {
            zipEntry = archive.CreateEntry(Path.Combine(conf.EnumDirectoryPath, $"{enum_obj.SystemName}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, $"{enum_obj.Name}");

            await writer.WriteLineAsync($"\tpublic enum {enum_obj.SystemName}");
            await writer.WriteLineAsync("\t{");
            is_first_item = true;
            //HtmlDocument doc;

            if (enum_obj.EnumItems is not null)
                foreach (SortableFitModel enum_item in enum_obj.EnumItems.OrderBy(x => x.SortIndex))
                {
                    if (!is_first_item)
                    {
                        await writer.WriteLineAsync();
                    }
                    else
                    {
                        is_first_item = false;
                    }
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync($"\t\t/// {enum_item.Name}");
                    await writer.WriteLineAsync("\t\t/// </summary>");

                    //if (!string.IsNullOrWhiteSpace(enum_item.Description))
                    //{
                    //    await writer.WriteLineAsync("\t\t/// <remarks>");
                    //    doc = new HtmlDocument();
                    //    doc.LoadHtml(enum_item.Description);
                    //    string[] lines = doc.DocumentNode.InnerText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    //    await writer.WriteLineAsync("\t\t/// </summary>");
                    //}

                    await writer.WriteLineAsync($"\t\t{enum_item.SystemName},");
                }

            await WriteEnd(writer);
        }
    }

    async Task DbTableAccessGen(IEnumerable<BaseFitModel> docs)
    {
        string crud_type_name, service_type_name, response_type_name, controller_name, service_instance;
        ZipArchiveEntry zipEntry;
        StreamWriter writer;

        foreach (BaseFitModel doc_obj in docs)
        {
            #region модели ответов тела документа (rest/api)

            response_type_name = $"{doc_obj.SystemName}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, $"{doc_obj.SystemName} : Response model (single object)", ["SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : ResponseBaseModel");
            await writer.WriteLineAsync("\t{");
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Результат запроса [{doc_obj.SystemName}] (полезная нагрузка)");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\tpublic {doc_obj.SystemName}? {GlobalStaticConstants.RESULT_PROPERTY_NAME} {{ get; set; }}");
            await WriteEnd(writer);


            response_type_name = $"{doc_obj.SystemName}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, $"{doc_obj.SystemName} : Response model (collection objects)", ["SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : ResponseBaseModel");
            await writer.WriteLineAsync("\t{");
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Результат запроса [{doc_obj.SystemName}] (полезная нагрузка)");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\tpublic IEnumerable<{doc_obj.SystemName}> {GlobalStaticConstants.RESULT_PROPERTY_NAME} {{ get; set; }}");
            await WriteEnd(writer);


            response_type_name = $"{doc_obj.SystemName}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, $"{doc_obj.SystemName} : Response model (paginations collection of objects)", ["SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : FindResponseModel");
            await writer.WriteLineAsync("\t{");
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Результат запроса [{doc_obj.SystemName}] (полезная нагрузка)");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\tpublic IEnumerable<{doc_obj.SystemName}> DataRows {{ get; set; }}");
            await WriteEnd(writer);

            #endregion

            #region тело документа

            crud_type_name = $"I{doc_obj.SystemName}{GlobalStaticConstants.DATABASE_TABLE_ACESSOR_PREFIX}";
            services_di.Add(crud_type_name, crud_type_name[1..]);
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "crud_interfaces", $"{crud_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name, ["SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial interface {crud_type_name} : SharedLib.ISavingChanges");
            await writer.WriteLineAsync("\t{");

            await WriteDocumentCrudInterface(writer, doc_obj.SystemName, doc_obj.Name, true);

            crud_type_name = crud_type_name[1..];
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "crud_implementations", $"{crud_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, null, ["DbcLib", "Microsoft.EntityFrameworkCore", "SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial class {crud_type_name} : I{crud_type_name}");
            await writer.WriteLineAsync("\t{");
            await writer.WriteLineAsync("\t\treadonly DbAppContext _db_context;");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync("\t\t/// Конструктор");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\tpublic {crud_type_name}(DbAppContext set_db_context)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t_db_context = set_db_context;");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();

            await WriteDocumentCrudInterfaceImplementation(writer, doc_obj.SystemName, true);



            service_type_name = $"I{doc_obj.SystemName}{GlobalStaticConstants.SERVICE_ACESSOR_PREFIX}";
            services_di.Add(service_type_name, service_type_name[1..]);
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "service_interfaces", $"{service_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name, ["SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial interface {service_type_name}");
            await writer.WriteLineAsync("\t{");

            await WriteDocumentServicesInterface(writer, doc_obj.SystemName, doc_obj.Name, true);

            service_type_name = service_type_name[1..];
            zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "service_implementations", $"{service_type_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, null, ["SharedLib.Models"]);

            await writer.WriteLineAsync($"\tpublic partial class {service_type_name} : I{service_type_name}");
            await writer.WriteLineAsync("\t{");
            await writer.WriteLineAsync($"\t\treadonly I{crud_type_name} _crud_accessor;");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync("\t\t/// Конструктор");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\tpublic {service_type_name}(I{crud_type_name} set_crud_accessor)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t_crud_accessor = set_crud_accessor;");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();
            await WriteDocumentServicesInterfaceImplementation(writer, doc_obj.SystemName, true);

            #endregion

            #region контроллеры тела документа

            if (!string.IsNullOrWhiteSpace(conf.ControllersDirectoryPath))
            {
                service_instance = $"_{service_type_name}".ToLower();
                controller_name = $"{doc_obj.SystemName}Controller";
                zipEntry = archive.CreateEntry(Path.Combine(conf.ControllersDirectoryPath, $"{controller_name}.cs"));
                writer = new(zipEntry.Open(), Encoding.UTF8);
                await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name, ["Microsoft.AspNetCore.Mvc", "SharedLib.Models"]);
                await writer.WriteLineAsync("\t[Route(\"api/[controller]\")]");
                await writer.WriteLineAsync("\t[ApiController]");
                await writer.WriteLineAsync($"\tpublic partial class {controller_name} : ControllerBase");
                await writer.WriteLineAsync("\t{");
                await writer.WriteLineAsync($"\t\treadonly I{service_type_name} {service_instance};");
                await writer.WriteLineAsync();
                await writer.WriteLineAsync($"\t\tpublic {controller_name}(I{service_type_name} set{service_instance})");
                await writer.WriteLineAsync("\t\t{");
                await writer.WriteLineAsync($"\t\t\t{service_instance} = set{service_instance};");
                await writer.WriteLineAsync("\t\t}");
                await writer.WriteLineAsync();
                await WriteDocumentControllers(writer, service_instance, doc_obj.SystemName, $"Документ: {doc_obj.Name}", true);
            }

            #endregion

            //#region refit/rest (тело документа)

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

            //#endregion

            /*if (doc_obj.Grids is not null)
                foreach (GridFitModel grid in doc_obj.Grids)
                {

                    if (!grid.Properties?.Any() == true)
                        continue;

                    #region модели ответов табличной части документа (rest/api)

                    response_type_name = $"{grid.SystemName}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, $"{grid.SystemName} : Response model (single object)", ["SharedLib.Models"]);
                    await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : ResponseBaseModel");
                    await writer.WriteLineAsync("\t{");
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync($"\t\t/// Результат запроса [{grid.SystemName}] (полезная нагрузка)");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync($"\t\tpublic {grid.SystemName} {GlobalStaticConstants.RESULT_PROPERTY_NAME} {{ get; set; }}");
                    await WriteEnd(writer);


                    response_type_name = $"{grid.SystemName}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, $"{grid.SystemName} : Response model (collection objects)", ["SharedLib.Models"]);
                    await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : ResponseBaseModel");
                    await writer.WriteLineAsync("\t{");
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync($"\t\t/// Результат запроса [{grid.SystemName}] (полезная нагрузка)");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync($"\t\tpublic IEnumerable<{grid.SystemName}> {GlobalStaticConstants.RESULT_PROPERTY_NAME} {{ get; set; }}");
                    await WriteEnd(writer);

                    response_type_name = $"{grid.SystemName}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "response_models", $"{response_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, $"{grid.SystemName} : Response model (paginations collection of objects)", new string[] { "SharedLib.Models" });
                    await writer.WriteLineAsync($"\tpublic partial class {response_type_name} : FindResponseModel");
                    await writer.WriteLineAsync("\t{");
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync($"\t\t/// Результат запроса [{grid.SystemName}] (полезная нагрузка)");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync($"\t\tpublic IEnumerable<{grid.SystemName}> DataRows {{ get; set; }}");
                    await WriteEnd(writer);

                    #endregion

                    #region табличная часть документа

                    crud_type_name = $"I{grid.SystemName}{GlobalStaticConstants.DATABASE_TABLE_ACESSOR_PREFIX}";
                    services_di.Add(crud_type_name, crud_type_name[1..]);
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "crud_interfaces", $"{crud_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, $"Табличная часть документа: {grid.Name}", ["SharedLib.Models"]);

                    await writer.WriteLineAsync($"\tpublic partial interface {crud_type_name} : SharedLib.ISavingChanges");
                    await writer.WriteLineAsync("\t{");

                    await WriteDocumentCrudInterface(writer, $"{grid.SystemName}", grid.Name, false);

                    crud_type_name = crud_type_name[1..];
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "crud_implementations", $"{crud_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, null, ["DbcLib", "Microsoft.EntityFrameworkCore", "SharedLib.Models"]);
                    await writer.WriteLineAsync($"\tpublic partial class {crud_type_name} : I{crud_type_name}");
                    await writer.WriteLineAsync("\t{");
                    await writer.WriteLineAsync("\t\treadonly DbAppContext _db_context;");
                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync("\t\t/// Конструктор");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync($"\t\tpublic {crud_type_name}(DbAppContext set_db_context)");
                    await writer.WriteLineAsync("\t\t{");
                    await writer.WriteLineAsync("\t\t\t_db_context = set_db_context;");
                    await writer.WriteLineAsync("\t\t}");
                    await writer.WriteLineAsync();
                    await WriteDocumentCrudInterfaceImplementation(writer, $"{grid.SystemName}", false);



                    service_type_name = $"I{grid.SystemName}{GlobalStaticConstants.SERVICE_ACESSOR_PREFIX}";
                    services_di.Add(service_type_name, service_type_name[1..]);
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "service_interfaces", $"{service_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, grid.Name, ["SharedLib.Models"]);

                    await writer.WriteLineAsync($"\tpublic partial interface {service_type_name}");
                    await writer.WriteLineAsync("\t{");

                    await WriteDocumentServicesInterface(writer, grid.SystemName, grid.Name, false);

                    service_type_name = service_type_name[1..];
                    zipEntry = archive.CreateEntry(Path.Combine(conf.AccessDataDirectoryPath, "service_implementations", $"{service_type_name}.cs"));
                    writer = new(zipEntry.Open(), Encoding.UTF8);
                    await WriteHead(writer, project.Name, conf.Namespace, null, ["SharedLib.Models"]);

                    await writer.WriteLineAsync($"\tpublic partial class {service_type_name} : I{service_type_name}");
                    await writer.WriteLineAsync("\t{");
                    await writer.WriteLineAsync($"\t\treadonly I{crud_type_name} _crud_accessor;");
                    await writer.WriteLineAsync();
                    await writer.WriteLineAsync("\t\t/// <summary>");
                    await writer.WriteLineAsync("\t\t/// Конструктор");
                    await writer.WriteLineAsync("\t\t/// </summary>");
                    await writer.WriteLineAsync($"\t\tpublic {service_type_name}(I{crud_type_name} set_crud_accessor)");
                    await writer.WriteLineAsync("\t\t{");
                    await writer.WriteLineAsync("\t\t\t_crud_accessor = set_crud_accessor;");
                    await writer.WriteLineAsync("\t\t}");
                    await writer.WriteLineAsync();
                    await WriteDocumentServicesInterfaceImplementation(writer, grid.SystemName, false);

                    #endregion

                    #region контроллеры табличной части документа

                    if (!string.IsNullOrWhiteSpace(conf.ControllersDirectoryPath))
                    {
                        service_instance = $"_{service_type_name}".ToLower();
                        controller_name = $"{grid.SystemName}Controller";
                        zipEntry = archive.CreateEntry(Path.Combine(conf.ControllersDirectoryPath, $"{controller_name}.cs"));
                        writer = new(zipEntry.Open(), Encoding.UTF8);
                        await WriteHead(writer, project.Name, conf.Namespace, $"{grid.Name} (табличная часть)", ["Microsoft.AspNetCore.Mvc", "SharedLib.Models"]);
                        await writer.WriteLineAsync("\t[Route(\"api/[controller]\")]");
                        await writer.WriteLineAsync("\t[ApiController]");
                        await writer.WriteLineAsync($"\tpublic partial class {controller_name} : ControllerBase");
                        await writer.WriteLineAsync("\t{");
                        await writer.WriteLineAsync($"\t\treadonly I{service_type_name} {service_instance};");
                        await writer.WriteLineAsync();
                        await writer.WriteLineAsync($"\t\tpublic {controller_name}(I{service_type_name} set_{service_instance})");
                        await writer.WriteLineAsync("\t\t{");
                        await writer.WriteLineAsync($"\t\t\t{service_instance} = set_{service_instance};");
                        await writer.WriteLineAsync("\t\t}");
                        await writer.WriteLineAsync();
                        await WriteDocumentControllers(writer, service_instance, grid.SystemName, $"Табличная часть: {grid.Name} // для документа: {doc_obj.Name}", false);
                    }

                    #endregion

                    //#region refit табличная часть

                    //rest_service_name = $"I{grid.SystemName}RestService";
                    //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, grid.SystemName.ToLower(), $"{rest_service_name}.cs"));
                    //writer = new(enumEntry.Open(), Encoding.UTF8);
                    //await WriteHead(writer, project_info.Name, project_info.NameSpace, $"REST служба работы с API -> {project_info.Name}", new string[] { "Refit", "SharedLib.Models" });
                    //await writer.WriteLineAsync($"\tpublic interface {rest_service_name}");
                    //await writer.WriteLineAsync("\t{");
                    //await WriteRestServiceInterface(writer, grid.SystemName, doc_obj.Name, false, false, false);

                    //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, grid.SystemName.ToLower(), $"{rest_service_name[1..]}.cs"));
                    //writer = new(enumEntry.Open(), Encoding.UTF8);
                    //await WriteHead(writer, project_info.Name, project_info.NameSpace, null, ["SharedLib.Models", "Refit", "Microsoft.Extensions.Logging"]);
                    //await writer.WriteLineAsync($"\tpublic class {rest_service_name[1..]} : {rest_service_name}");
                    //await writer.WriteLineAsync("\t{");
                    //await WriteRestServiceImplementation(writer, grid.SystemName, false);

                    //rest_service_name = $"I{grid.SystemName}RefitProvider";
                    //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, grid.SystemName.ToLower(), "core", $"{rest_service_name}.cs"));
                    //writer = new(enumEntry.Open(), Encoding.UTF8);
                    //await WriteHead(writer, project_info.Name, project_info.NameSpace, $"Refit коннектор к API/{project_info.Name}", ["Refit", "SharedLib.Models"]);
                    //await writer.WriteLineAsync($"\tpublic interface {rest_service_name}");
                    //await writer.WriteLineAsync("\t{");
                    //await WriteRestServiceInterface(writer, grid.SystemName, doc_obj.Name, false, true, false);

                    //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, grid.SystemName.ToLower(), "core", $"{rest_service_name[1..]}.cs"));
                    //writer = new(enumEntry.Open(), Encoding.UTF8);
                    //await WriteHead(writer, project_info.Name, project_info.NameSpace, null, ["SharedLib.Models", "Refit", "Microsoft.Extensions.Logging"]);
                    //await writer.WriteLineAsync($"\tpublic class {rest_service_name[1..]} : {rest_service_name}");
                    //await writer.WriteLineAsync("\t{");
                    //await WriteRefitProviderImplementation(writer, grid.SystemName, false);

                    //rest_service_name = $"I{grid.SystemName}RefitService";
                    //enumEntry = archive.CreateEntry(Path.Combine(refit_client_services_dir_name, grid.SystemName.ToLower(), "core", $"{rest_service_name}.cs"));
                    //writer = new(enumEntry.Open(), Encoding.UTF8);
                    //await WriteHead(writer, project_info.Name, project_info.NameSpace, $"Refit коннектор к API/{project_info.Name}", ["Refit", "SharedLib.Models"]);
                    //await writer.WriteLineAsync("\t[Headers(\"Content-Type: application/json\")]");
                    //await writer.WriteLineAsync($"\tpublic interface {rest_service_name}");
                    //await writer.WriteLineAsync("\t{");
                    //await WriteRestServiceInterface(writer, grid.SystemName, doc_obj.Name, false, true, true);

                    //#endregion
                }*/
        }
    }

    async Task DbContextGen(IEnumerable<BaseFitModel> docs)
    {
        ZipArchiveEntry zipEntry = archive.CreateEntry("LayerContextPartGen.cs");
        StreamWriter writer = new(zipEntry.Open(), Encoding.UTF8);
        await WriteHead(writer, project.Name, "DbLayerLib", "Database context", ["Microsoft.EntityFrameworkCore", conf.Namespace]);

        await writer.WriteLineAsync("\tpublic partial class LayerContext : DbContext");
        await writer.WriteLineAsync("\t{");
        bool is_first_item = true;
        foreach (BaseFitModel doc_obj in docs)
        {
            if (!is_first_item)
                await writer.WriteLineAsync();
            else
                is_first_item = false;

            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// {doc_obj.Description}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            writer.WriteLine($"\t\tpublic DbSet<{doc_obj.SystemName}> {doc_obj.SystemName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");

            //if (doc_obj.Grids is not null)
            //    foreach (GridFitModel? grid in doc_obj.Grids)
            //    {
            //        await writer.WriteLineAsync();
            //        await writer.WriteLineAsync("\t\t/// <summary>");
            //        await writer.WriteLineAsync($"\t\t/// {grid.Name} [Табличная часть: {doc_obj.Name}]");
            //        await writer.WriteLineAsync("\t\t/// </summary>");
            //        await writer.WriteLineAsync($"\t\tpublic DbSet<{grid.SystemName}> {grid.SystemName}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX} {{ get; set; }}");
            //    }
        }

        await WriteEnd(writer);
    }

    async Task DocumentsShemaGen(IEnumerable<BaseFitModel> docs)
    {
        ZipArchiveEntry zipEntry;
        StreamWriter writer;
        string type_class_name;

        //bool is_first_item;
        foreach (BaseFitModel doc_obj in docs)
        {
            type_class_name = doc_obj.SystemName;

            zipEntry = archive.CreateEntry(Path.Combine(conf.DocumentsMastersDbDirectoryPath, $"{type_class_name}.cs"));
            writer = new(zipEntry.Open(), Encoding.UTF8);
            await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name);

            await writer.WriteLineAsync($"\tpublic partial class {doc_obj.SystemName} : SharedLib.Models.IdRemovableModel");
            await writer.WriteLineAsync("\t{");
            //is_first_item = true;


            //if (doc_obj.PropertiesBody is not null)
            //    foreach (DocumentPropertyFitModel property in doc_obj.PropertiesBody.OrderBy(x => x.SortIndex))
            //    {
            //        if (!is_first_item)
            //        {
            //            await writer.WriteLineAsync();
            //        }
            //        else
            //        {
            //            is_first_item = false;
            //        }
            //        await writer.WriteLineAsync("\t\t/// <summary>");
            //        await writer.WriteLineAsync($"\t\t/// {property.Name}");
            //        await writer.WriteLineAsync("\t\t/// </summary>");
            //        switch (property.PropertyType)
            //        {
            //            case PropertyTypesEnum.String:
            //                await writer.WriteLineAsync($"\t\tpublic string {property.SystemName} {{ get; set; }}");
            //                break;
            //            case PropertyTypesEnum.Int:
            //                await writer.WriteLineAsync($"\t\tpublic int {property.SystemName} {{ get; set; }}");
            //                break;
            //            case PropertyTypesEnum.Decimal:
            //                await writer.WriteLineAsync($"\t\tpublic decimal {property.SystemName} {{ get; set; }}");
            //                break;
            //            case PropertyTypesEnum.Bool:
            //                await writer.WriteLineAsync($"\t\tpublic bool {property.SystemName} {{ get; set; }}");
            //                break;
            //            case PropertyTypesEnum.DateTime:
            //                await writer.WriteLineAsync($"\t\tpublic DateTime {property.SystemName} {{ get; set; }}");
            //                break;
            //            case PropertyTypesEnum.SimpleEnum:
            //                await writer.WriteLineAsync($"\t\tpublic {property.PropertyTypeMetadata?.SystemName} {property.SystemName} {{ get; set; }}");
            //                break;
            //            case PropertyTypesEnum.Document:
            //                await writer.WriteLineAsync($"\t\tpublic int {property.SystemName}Id {{ get; set; }}");
            //                await writer.WriteLineAsync($"\t\tpublic {property.PropertyTypeMetadata?.SystemName} {property.SystemName} {{ get; set; }}");
            //                break;
            //            default:
            //                throw new Exception();
            //        }
            //    }

            //if (doc_obj.Grids is not null)
            //    foreach (GridFitModel grid in doc_obj.Grids)
            //    {
            //        await writer.WriteLineAsync();
            //        await writer.WriteLineAsync("\t\t/// <summary>");
            //        await writer.WriteLineAsync($"\t\t/// '{grid.Name}': Табличная часть документа");
            //        await writer.WriteLineAsync("\t\t/// </summary>");
            //        await writer.WriteLineAsync($"\t\tpublic ICollection<{grid.SystemName}> {grid.SystemName}{GlobalStaticConstants.TABLE_PROPERTY_NAME_PREFIX} {{ get; set; }}");
            //    }

            await WriteEnd(writer);

            //if (doc_obj.Grids is not null)
            //    foreach (GridFitModel grid in doc_obj.Grids)
            //    {
            //        type_class_name = $"{grid.SystemName}";


            //        zipEntry = archive.CreateEntry(Path.Combine(conf.DocumentsMastersDbDirectoryPath, $"{grid.SystemName}.cs"));
            //        writer = new(zipEntry.Open(), Encoding.UTF8);
            //        await WriteHead(writer, project.Name, conf.Namespace, doc_obj.Name);

            //        await writer.WriteLineAsync($"\tpublic partial class {grid.SystemName} : SharedLib.Models.IdRemovableModel");
            //        await writer.WriteLineAsync("\t{");

            //        await writer.WriteLineAsync("\t\t/// <summary>");
            //        await writer.WriteLineAsync($"\t\t/// (FK) {grid.Name}");
            //        await writer.WriteLineAsync("\t\t/// </summary>");
            //        await writer.WriteLineAsync($"\t\tpublic int {grid.SystemName}OwnerId {{ get; set; }}");
            //        await writer.WriteLineAsync();
            //        await writer.WriteLineAsync("\t\t/// <summary>");
            //        await writer.WriteLineAsync($"\t\t/// {grid.Name}");
            //        await writer.WriteLineAsync("\t\t/// </summary>");
            //        await writer.WriteLineAsync($"\t\tpublic {grid.SystemName} {grid.SystemName}Owner {{ get; set; }}");

            //        if (grid.Properties is not null)
            //            foreach (DocumentPropertyFitModel property in grid.Properties.OrderBy(x => x.SortIndex))
            //            {
            //                await writer.WriteLineAsync();
            //                await writer.WriteLineAsync("\t\t/// <summary>");
            //                await writer.WriteLineAsync($"\t\t/// {property.Name}");
            //                await writer.WriteLineAsync("\t\t/// </summary>");
            //                switch (property.PropertyType)
            //                {
            //                    case PropertyTypesEnum.String:
            //                        await writer.WriteLineAsync($"\t\tpublic string {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    case PropertyTypesEnum.Int:
            //                        await writer.WriteLineAsync($"\t\tpublic int {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    case PropertyTypesEnum.Decimal:
            //                        await writer.WriteLineAsync($"\t\tpublic decimal {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    case PropertyTypesEnum.Bool:
            //                        await writer.WriteLineAsync($"\t\tpublic bool {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    case PropertyTypesEnum.DateTime:
            //                        await writer.WriteLineAsync($"\t\tpublic DateTime {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    case PropertyTypesEnum.SimpleEnum:
            //                        await writer.WriteLineAsync($"\t\tpublic {property.PropertyTypeMetadata?.SystemName} {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    case PropertyTypesEnum.Document:
            //                        await writer.WriteLineAsync($"\t\tpublic {property.PropertyTypeMetadata?.SystemName} {property.SystemName} {{ get; set; }}");
            //                        break;
            //                    default:
            //                        throw new Exception();
            //                }
            //            }

            //        await WriteEnd(writer);
            //    }

        }
    }


    /// <summary>
    /// Генерация файла регистрации DI служб
    /// </summary>
    async Task GenServicesDI()
    {
        ZipArchiveEntry readmeEntry = archive.CreateEntry("services_di.cs");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        await WriteHead(writer, project.Name, null, "di services", [conf.Namespace]);
        await writer.WriteLineAsync("\tpublic static class ServicesExtensionDesignerDI");
        await writer.WriteLineAsync("\t{");
        await writer.WriteLineAsync("\t\tpublic static void BuildDesignerServicesDI(this IServiceCollection services)");
        await writer.WriteLineAsync("\t\t{");
        foreach (KeyValuePair<string, string> kvp in services_di)
            await writer.WriteLineAsync($"\t\t\tservices.AddScoped<{kvp.Key}, {kvp.Value}>();");

        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись контроллеров
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="service_instance">Имя интерфейса промежуточной службы доступа к данным</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="name">Наименование типа данных</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteDocumentControllers(StreamWriter writer, string service_instance, string type_name, string name, bool is_body_document)
    {
        string type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Добавить/создать документ: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"object_rest\">Новый объект '{name}' для записи/добавления в БД</param>");
        await writer.WriteLineAsync("\t\t[HttpPost($\"{nameof(RouteMethodsPrefixesEnum.AddSingle)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<IdResponseModel> AddAsync({type_name_gen} object_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.AddAsync(object_rest);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Добавить/создать коллекцию документов: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"objects_range_rest\">Коллекция новых объектов '{name}' для записи/добавления в БД</param>");
        await writer.WriteLineAsync("\t\t[HttpPost($\"{nameof(RouteMethodsPrefixesEnum.AddRange)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> AddRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.AddRangeAsync(objects_range_rest);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить документ по идентификатору: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта '{name}'</param>");
        await writer.WriteLineAsync("\t\t[HttpGet($\"{nameof(RouteMethodsPrefixesEnum.GetSingleById)}/{{id}}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> FirstAsync([FromRoute] int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.FirstAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить коллекцию документов по идентификаторам: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов '{name}'</param>");
        await writer.WriteLineAsync("\t\t[HttpGet($\"{nameof(RouteMethodsPrefixesEnum.GetRangeByIds)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync([FromQuery] IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.SelectAsync(ids);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        if (is_body_document)
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить порцию (пагинатор) документов: {name}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\t/// <param name=\"request\">Запрос-пагинатор для чтения документов '{name}'</param>");
            await writer.WriteLineAsync("\t\t[HttpGet($\"{nameof(RouteMethodsPrefixesEnum.GetRangePagination)}\")]");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync([FromQuery] PaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
            await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.SelectAsync(request);");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();
        }
        else
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить порцию (пагинатор) строк табличной части документа по идентификатору: {name}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\t/// <param name=\"request\">Запрос-пагинатор (+ идентификатор документа-владельца) для чтения строк табличной части документа '{name}'</param>");
            await writer.WriteLineAsync("\t\t[HttpGet($\"{nameof(RouteMethodsPrefixesEnum.GetRangeByOwnerId)}\")]");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync([FromQuery] GetByIdPaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
            await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.SelectAsync(request);");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();
        }

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить объект в БД: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"object_rest_upd\">Объект '{name}' для обновления в БД</param>");
        await writer.WriteLineAsync("\t\t[HttpPut($\"{nameof(RouteMethodsPrefixesEnum.UpdateSingle)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> UpdateAsync({type_name_gen} object_rest_upd)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.UpdateAsync(object_rest_upd);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить коллекцию документов в БД: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"objects_range_rest_upd\">Коллекция объектов '{name}' для обновления в БД</param>");
        await writer.WriteLineAsync("\t\t[HttpPut($\"{nameof(RouteMethodsPrefixesEnum.UpdateRange)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> UpdateRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest_upd)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.UpdateRangeAsync(objects_range_rest_upd);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Инвертировать маркер удаления объекта: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта '{name}' для переключения/инверсии пометки удаления</param>");
        await writer.WriteLineAsync("\t\t[HttpPatch($\"{nameof(RouteMethodsPrefixesEnum.MarkAsDeleteById)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> MarkDeleteToggleAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.MarkDeleteToggleAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить объект из БД по идентификатору: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта '{name}' для удаления из БД</param>");
        await writer.WriteLineAsync("\t\t[HttpDelete($\"{nameof(RouteMethodsPrefixesEnum.RemoveSingleById)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> RemoveAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.RemoveAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить объекты из БД по идентификаторам: {name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов '{name}' для удаления из БД</param>");
        await writer.WriteLineAsync("\t\t[HttpDelete($\"{nameof(RouteMethodsPrefixesEnum.RemoveRangeByIds)}\")]");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> RemoveRangeAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\treturn await {service_instance}.RemoveRangeAsync(ids);");
        await writer.WriteLineAsync("\t\t}");

        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись тела метода Rest/Refit сервиса
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="method_name">Имя Rest/Refit метода</param>
    /// <param name="rest_parameter_name">Имя Rest/Refit параметра</param>
    /// <param name="response_type_name">Имя Rest/Refit метода</param>
    static async Task WriteRestServiceBody(StreamWriter writer, string method_name, string rest_parameter_name, string response_type_name)
    {
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync($"\t\t\t\tApiResponse<{response_type_name}> rest = await _service.{method_name}({rest_parameter_name});");
        await writer.WriteLineAsync("\t\t\t\tif (rest.StatusCode != System.Net.HttpStatusCode.OK)");
        await writer.WriteLineAsync("\t\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\t\tresult.Message = $\"HTTP error: [code={rest.StatusCode}] {rest?.Error?.Content}\";");
        await writer.WriteLineAsync("\t\t\t\t\t_logger.LogError(result.Message);");
        await writer.WriteLineAsync("\t\t\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t\t\t}");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = rest.Content.IsSuccess;");
        await writer.WriteLineAsync("\t\t\t\tresult = rest.Content;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync($"\t\t\t\tresult.Message = $\"Exception {{nameof(_service.{method_name})}}\";");
        await writer.WriteLineAsync("\t\t\t\t_logger.LogError(ex, result.Message);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\treturn result;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync();
    }

#pragma warning disable IDE0051 // Удалите неиспользуемые закрытые члены
    /// <summary>
    /// Генерация регистратора служб Refit для клиента
    /// </summary>
    /// <param name="types">Типы для регистрации</param>
    async Task GenRefitDI(IEnumerable<string> types)
    {
        ZipArchiveEntry readmeEntry = archive.CreateEntry("refit_di.cs");
        using StreamWriter writer = new(readmeEntry.Open(), Encoding.UTF8);
        await WriteHead(writer, project.Name, null, "di refit", new string[] { conf.Namespace, "Refit", "SharedLib.Models", "SharedLib", "Microsoft.Extensions.DependencyInjection" });
        await writer.WriteLineAsync("\tpublic static class RefitExtensionDesignerDI");
        await writer.WriteLineAsync("\t{");
        await writer.WriteLineAsync("\t\tpublic static void BuildRefitServicesDI(this IServiceCollection services, ClientConfigModel conf, TimeSpan handler_lifetime)");
        await writer.WriteLineAsync("\t\t{");
        foreach (string s in types)
        {
            await writer.WriteLineAsync($"\t\t\tservices.AddRefitClient<I{s}RefitService>()");
            await writer.WriteLineAsync("\t\t\t\t.ConfigureHttpClient(c => c.BaseAddress = conf.ApiConfig.Url)");
            await writer.WriteLineAsync("\t\t\t\t.AddHttpMessageHandler<RefitHeadersDelegatingHandler>()");
            await writer.WriteLineAsync("\t\t\t\t.SetHandlerLifetime(handler_lifetime);");
            await writer.WriteLineAsync($"\t\t\tservices.AddScoped<I{s}RefitProvider, {s}RefitProvider>();");
            await writer.WriteLineAsync($"\t\t\tservices.AddScoped<I{s}RestService, {s}RestService>();");
            await writer.WriteLineAsync();
        }
        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись реализации Rest/refit сервиса
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteRestServiceImplementation(StreamWriter writer, string type_name, bool is_body_document)

    {
        await writer.WriteLineAsync($"\t\tprivate readonly I{type_name}RefitService _service;");
        await writer.WriteLineAsync($"\t\tprivate readonly ILogger<{type_name}RestService> _logger;");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync("\t\t/// Конструктор");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\tpublic {type_name}RestService(I{type_name}RefitService set_service, ILogger<{type_name}RestService> set_logger)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t_service = set_service;");
        await writer.WriteLineAsync("\t\t\t_logger = set_logger;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<IdResponseModel> AddAsync({type_name} object_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\tIdResponseModel result = new();");
        await WriteRestServiceBody(writer, "AddAsync", "object_rest", "IdResponseModel");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> AddRangeAsync(IEnumerable<{type_name}> objects_range_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new();");
        await WriteRestServiceBody(writer, "AddRangeAsync", "objects_range_rest", "ResponseBaseModel");

        string type_name_gen = $"{type_name}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> FirstAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new();");
        await WriteRestServiceBody(writer, "FirstAsync", "id", type_name_gen);

        type_name_gen = $"{type_name}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new();");
        await WriteRestServiceBody(writer, "SelectAsync", "ids", type_name_gen);

        if (is_body_document)
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync(PaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new();");
            await WriteRestServiceBody(writer, "SelectAsync", "request", type_name_gen);
        }
        else
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync(GetByIdPaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new();");
            await WriteRestServiceBody(writer, "SelectAsync", "request", type_name_gen);
        }

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> UpdateAsync({type_name_gen} object_rest_upd)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\tResponseBaseModel result = new();");
        await WriteRestServiceBody(writer, "UpdateAsync", "object_rest_upd", "ResponseBaseModel");

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> UpdateRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest_upd)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\tResponseBaseModel result = new();");
        await WriteRestServiceBody(writer, "UpdateRangeAsync", "objects_range_rest_upd", "ResponseBaseModel");

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> MarkDeleteToggleAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\tResponseBaseModel result = new();");
        await WriteRestServiceBody(writer, "MarkDeleteToggleAsync", "id", "ResponseBaseModel");

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> RemoveAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\tResponseBaseModel result = new();");
        await WriteRestServiceBody(writer, "RemoveAsync", "id", "ResponseBaseModel");

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> RemoveRangeAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync($"\t\t\tResponseBaseModel result = new();");
        await WriteRestServiceBody(writer, "RemoveRangeAsync", "ids", "ResponseBaseModel");

        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись реализации Refit провайдера
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteRefitProviderImplementation(StreamWriter writer, string type_name, bool is_body_document)
    {
        string type_name_gen = $"{type_name}";
        await writer.WriteLineAsync($"\t\tprivate readonly I{type_name_gen}RefitService _api;");
        await writer.WriteLineAsync($"\t\tprivate readonly ILogger<{type_name_gen}RefitProvider> _logger;");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync("\t\t/// Конструктор");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\tpublic {type_name_gen}RefitProvider(I{type_name}RefitService set_api, ILogger<{type_name_gen}RefitProvider> set_logger)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t_api = set_api;");
        await writer.WriteLineAsync("\t\t\t_logger = set_logger;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<IdResponseModel>> AddAsync({type_name_gen} object_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.AddAsync(object_rest);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<ResponseBaseModel>> AddRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.AddRangeAsync(objects_range_rest);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<{type_name_gen}>> FirstAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.FirstAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<{type_name_gen}>> SelectAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.SelectAsync(ids);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        if (is_body_document)
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<{type_name_gen}>> SelectAsync(PaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\treturn await _api.SelectAsync(request);");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();

        }
        else
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<{type_name_gen}>> SelectAsync(GetByIdPaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\treturn await _api.SelectAsync(request);");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();
        }

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<ResponseBaseModel>> UpdateAsync({type_name_gen} object_rest_upd)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.UpdateAsync(object_rest_upd);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<ResponseBaseModel>> UpdateRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest_upd)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.UpdateRangeAsync(objects_range_rest_upd);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<ResponseBaseModel>> MarkDeleteToggleAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.MarkDeleteToggleAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<ResponseBaseModel>> RemoveAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.RemoveAsync(id);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ApiResponse<ResponseBaseModel>> RemoveRangeAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\treturn await _api.RemoveRangeAsync(ids);");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись refit интерфейсов (3 вида)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="name">Название объекта</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    /// <param name="is_refit">refit ответы (ApiResponse) - true. rest/json ответы - false</param>
    /// <param name="add_attr">Добавить заголовок запроса и refit атрибуты маршрута запросов</param>
    /// <returns></returns>
    static async Task WriteRestServiceInterface(StreamWriter writer, string type_name, string name, bool is_body_document, bool is_refit, bool add_attr)
    {
        string type_name_gen = $"{type_name}";

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Добавить документ в БД: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Post($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.AddSingle)}}\")]");
        }
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}IdResponseModel{(is_refit ? ">" : "")}> AddAsync({type_name_gen} object_rest);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Добавить набор документов в БД: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Post($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.AddRange)}}\")]");
        }
        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}ResponseBaseModel{(is_refit ? ">" : "")}> AddRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить документ по идентификатору: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Get($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.GetSingleById)}}/{{{{id}}}}\")]");
        }
        type_name_gen = $"{type_name}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}{type_name_gen}{(is_refit ? ">" : "")}> FirstAsync(int id);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить коллекцию документов по идентификаторам: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Get($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.GetRangeByIds)}}\")]");
        }
        type_name_gen = $"{type_name}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}{type_name_gen}{(is_refit ? ">" : "")}> SelectAsync(IEnumerable<int> ids);");
        await writer.WriteLineAsync();

        if (is_body_document)
        {
            await writer.WriteLineAsync($"\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить порцию (пагинатор) документов: {name}");
            await writer.WriteLineAsync($"\t\t/// </summary>");
            if (add_attr)
            {
                await writer.WriteLineAsync($"\t\t[Get($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.GetRangePagination)}}\")]");
            }
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}{type_name_gen}{(is_refit ? ">" : "")}> SelectAsync(PaginationRequestModel request);");
            await writer.WriteLineAsync();
        }
        else
        {
            await writer.WriteLineAsync($"\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить порцию (пагинатор) строк табличной части документа по идентификатору документа: {name}");
            await writer.WriteLineAsync($"\t\t/// </summary>");
            if (add_attr)
            {
                await writer.WriteLineAsync($"\t\t[Get($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.GetRangeByOwnerId)}}\")]");
            }
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}{type_name_gen}{(is_refit ? ">" : "")}> SelectAsync(GetByIdPaginationRequestModel request);");
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить документ в БД: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Put($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.UpdateSingle)}}\")]");
        }
        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}ResponseBaseModel{(is_refit ? ">" : "")}> UpdateAsync({type_name_gen} object_rest_upd);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить коллекцию документов в БД: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Put($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.UpdateRange)}}\")]");
        }
        type_name_gen = $"{type_name}";
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}ResponseBaseModel{(is_refit ? ">" : "")}> UpdateRangeAsync(IEnumerable<{type_name_gen}> objects_range_rest_upd);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Инверсия признака \"помечен на удаление\" на противоположное: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Patch($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.MarkAsDeleteById)}}\")]");
        }
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}ResponseBaseModel{(is_refit ? ">" : "")}> MarkDeleteToggleAsync(int id);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить документ из БД по идентификатору: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Delete($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.RemoveSingleById)}}\")]");
        }
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}ResponseBaseModel{(is_refit ? ">" : "")}> RemoveAsync(int id);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync($"\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить документы из БД по идентификаторам: {name}");
        await writer.WriteLineAsync($"\t\t/// </summary>");
        if (add_attr)
        {
            await writer.WriteLineAsync($"\t\t[Delete($\"/api/{type_name.ToLower()}/{{nameof(RouteMethodsPrefixesEnum.RemoveRangeByIds)}}\")]");
        }
        await writer.WriteLineAsync($"\t\tpublic Task<{(is_refit ? "ApiResponse<" : "")}ResponseBaseModel{(is_refit ? ">" : "")}> RemoveRangeAsync(IEnumerable<int> ids);");

        await WriteEnd(writer);
    }
#pragma warning restore IDE0051 // Удалите неиспользуемые закрытые члены

    /// <summary>
    /// Записать реализации интерфейса промежуточной службы доступа к данным
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteDocumentServicesInterfaceImplementation(StreamWriter writer, string type_name, bool is_body_document)
    {
        string type_name_gen;

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<IdResponseModel> AddAsync({type_name} obj_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tIdResponseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync($"\t\t\t\tawait _crud_accessor.AddAsync(obj_rest);");
        await writer.WriteLineAsync($"\t\t\t\tresult.Id = obj_rest.Id;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> AddRangeAsync(IEnumerable<{type_name}> obj_range_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync($"\t\t\t\tawait _crud_accessor.AddRangeAsync(obj_range_rest);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}?> FirstAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new() {{ IsSuccess = true }};");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync($"\t\t\t\tresult.{GlobalStaticConstants.RESULT_PROPERTY_NAME} = await _crud_accessor.FirstAsync(id);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        type_name_gen = $"{type_name}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}";
        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new() {{ IsSuccess = true }};");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync($"\t\t\t\tresult.{GlobalStaticConstants.RESULT_PROPERTY_NAME} = await _crud_accessor.SelectAsync(ids);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        if (is_body_document)
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync(PaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
            await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new() {{ IsSuccess = true }};");
            await writer.WriteLineAsync("\t\t\ttry");
            await writer.WriteLineAsync("\t\t\t{");
            await writer.WriteLineAsync("\t\t\t\tresult = await _crud_accessor.SelectAsync(request);");
            await writer.WriteLineAsync("\t\t\t}");
            await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
            await writer.WriteLineAsync("\t\t\t{");
            await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
            await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
            await writer.WriteLineAsync("\t\t\t}");

            await writer.WriteLineAsync($"\t\t\treturn result;");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();
        }
        else
        {
            type_name_gen = $"{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}";
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name_gen}> SelectAsync(GetByIdPaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
            await writer.WriteLineAsync($"\t\t\t{type_name_gen} result = new() {{ IsSuccess = true }};");
            await writer.WriteLineAsync("\t\t\ttry");
            await writer.WriteLineAsync("\t\t\t{");
            await writer.WriteLineAsync("\t\t\t\tresult = await _crud_accessor.SelectAsync(request);");
            await writer.WriteLineAsync("\t\t\t}");
            await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
            await writer.WriteLineAsync("\t\t\t{");
            await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
            await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
            await writer.WriteLineAsync("\t\t\t}");

            await writer.WriteLineAsync($"\t\t\treturn result;");
            await writer.WriteLineAsync("\t\t}");
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> UpdateAsync({type_name} obj_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync($"\t\t\t\tawait _crud_accessor.UpdateAsync(obj_rest);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> UpdateRangeAsync(IEnumerable<{type_name}> obj_range_rest)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tawait _crud_accessor.UpdateRangeAsync(obj_range_rest);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> MarkDeleteToggleAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tawait _crud_accessor.MarkDeleteToggleAsync(id);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> RemoveAsync(int id)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tawait _crud_accessor.RemoveRangeAsync(new int[] { id });");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
        await writer.WriteLineAsync($"\t\tpublic async Task<ResponseBaseModel> RemoveRangeAsync(IEnumerable<int> ids)");
        await writer.WriteLineAsync("\t\t{");
        await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
        await writer.WriteLineAsync("\t\t\tResponseBaseModel result = new() { IsSuccess = true };");
        await writer.WriteLineAsync("\t\t\ttry");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tawait _crud_accessor.RemoveRangeAsync(ids);");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\tcatch (Exception ex)");
        await writer.WriteLineAsync("\t\t\t{");
        await writer.WriteLineAsync("\t\t\t\tresult.IsSuccess = false;");
        await writer.WriteLineAsync("\t\t\t\tresult.Message = ex.Message;");
        await writer.WriteLineAsync("\t\t\t}");
        await writer.WriteLineAsync("\t\t\treturn result;");
        await writer.WriteLineAsync("\t\t}");

        await WriteEnd(writer);
    }

    /// <summary>
    /// Записать интерфейс промежуточной службы доступа к данным
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="doc_obj_name">Наименование документа (для документации)</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteDocumentServicesInterface(StreamWriter writer, string type_name, string doc_obj_name, bool is_body_document)
    {
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Создать новый объект{(is_body_document ? "" : " строки (табличной части)")} документа (запись БД): {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest\">Объект добавления в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<IdResponseModel> AddAsync({type_name} obj_rest);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Создать перечень новых объектов{(is_body_document ? "" : " строк табличной части")} документа: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest_range\">Объекты добавления в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<ResponseBaseModel> AddRangeAsync(IEnumerable<{type_name}> obj_rest_range);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Прочитать {(is_body_document ? "документ" : "строку табличной части документа")}: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.SINGLE_REPONSE_MODEL_PREFIX}?> FirstAsync(int id);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить (набор){(is_body_document ? "" : " строк табличной части")} документов: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.MULTI_REPONSE_MODEL_PREFIX}> SelectAsync(IEnumerable<int> ids);");
        await writer.WriteLineAsync();

        if (is_body_document)
        {
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить (набор) строк табличной части документа: {doc_obj_name}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\t/// <param name=\"request\">Пагинация запроса</param>");
            await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(PaginationRequestModel request);");
            await writer.WriteLineAsync();
        }
        else
        {
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить (набор) строк табличной части документа: {doc_obj_name}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\t/// <param name=\"request\">Пагинация запроса</param>");
            await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(GetByIdPaginationRequestModel request);");
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить объект{(is_body_document ? "" : " строки табличной части")} документа: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest\">Объект обновления в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<ResponseBaseModel> UpdateAsync({type_name} obj_rest);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить перечень объектов{(is_body_document ? "/документов" : " строк табличной части документа")}: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_range_rest\">Объекты обновления в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<ResponseBaseModel> UpdateRangeAsync(IEnumerable<{type_name}> obj_range_rest);");
        await writer.WriteLineAsync();


        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Инверсия признака удаления{(is_body_document ? "" : " строки табличной части")} документа: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<ResponseBaseModel> MarkDeleteToggleAsync(int id);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить {(is_body_document ? "документ" : "строку табличной части")}: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<ResponseBaseModel> RemoveAsync(int id);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить перечень{(is_body_document ? "" : " строк табличной части")} документов: {doc_obj_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<ResponseBaseModel> RemoveRangeAsync(IEnumerable<int> ids);");

        await WriteEnd(writer);
    }

    /// <summary>
    /// Запись CRUD реализаций интерфейсов служб непосредственного доступа к данным (к таблицам БД)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteDocumentCrudInterfaceImplementation(StreamWriter writer, string type_name, bool is_body_document)
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

        if (is_body_document)
        {
            await writer.WriteLineAsync("\t\t/// <inheritdoc/>");
            await writer.WriteLineAsync($"\t\tpublic async Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(PaginationRequestModel request)");
            await writer.WriteLineAsync("\t\t{");
            await writer.WriteLineAsync("\t\t\t//// TODO: Проверить сгенерированный код");
            await writer.WriteLineAsync($"\t\t\tIQueryable<{type_name}>? query = _db_context.{type_name}{GlobalStaticConstants.CONTEXT_DATA_SET_PREFIX}.AsQueryable();");
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
        }
        else
        {
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
        }

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

    /// <summary>
    /// Запись CRUD интерфейсов служб непосредственного доступа к данным (к таблицам БД)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    /// <param name="type_name">Имя типа данных (SystemName)</param>
    /// <param name="doc_name">Имя документа для комментариев</param>
    /// <param name="is_body_document">Если тело документа - true. Если табличная часть - false</param>
    static async Task WriteDocumentCrudInterface(StreamWriter writer, string type_name, string doc_name, bool is_body_document)
    {
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Создать новый объект{(is_body_document ? "" : " строки (табличной части)")} документа (запись БД): {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest\">Объект добавления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task AddAsync({type_name} obj_rest, bool auto_save = true);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Создать перечень новых объектов{(is_body_document ? "" : " строк табличной части")} документа: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest_range\">Объекты добавления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task AddRangeAsync(IEnumerable<{type_name}> obj_rest_range, bool auto_save = true);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Прочитать {(is_body_document ? "документ" : "строку табличной части документа")}: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<{type_name}?> FirstAsync(int id);");
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Получить (набор){(is_body_document ? "" : " строк табличной части")} документов: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов</param>");
        await writer.WriteLineAsync($"\t\tpublic Task<IEnumerable<{type_name}>> SelectAsync(IEnumerable<int> ids);");
        await writer.WriteLineAsync();

        if (is_body_document)
        {
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить (страницу){(is_body_document ? "" : " строк табличной части")} документов: {doc_name}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\t/// <param name=\"pagination_request\">Запрос-пагинатор</param>");
            await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(PaginationRequestModel pagination_request);");
            await writer.WriteLineAsync();
        }
        else
        {
            await writer.WriteLineAsync("\t\t/// <summary>");
            await writer.WriteLineAsync($"\t\t/// Получить (набор) строк табличной части документа: {doc_name}");
            await writer.WriteLineAsync("\t\t/// </summary>");
            await writer.WriteLineAsync($"\t\t/// <param name=\"pagination_request\">Запрос-пагинатор</param>");
            await writer.WriteLineAsync($"\t\tpublic Task<{type_name}{GlobalStaticConstants.PAGINATION_REPONSE_MODEL_PREFIX}> SelectAsync(GetByIdPaginationRequestModel pagination_request);");
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить объект{(is_body_document ? "" : " строки табличной части")} документа: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest\">Объект обновления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task UpdateAsync({type_name} obj_rest, bool auto_save = true);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Обновить перечень объектов{(is_body_document ? "/документов" : " строк табличной части документа")}: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"obj_rest_range\">Объекты обновления в БД</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task UpdateRangeAsync(IEnumerable<{type_name}> obj_rest_range, bool auto_save = true);");
        await writer.WriteLineAsync();


        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Инверсия признака удаления{(is_body_document ? "" : " строки табличной части")} документа: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task MarkDeleteToggleAsync(int id, bool auto_save = true);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить {(is_body_document ? "документ" : "строку табличной части")}: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"id\">Идентификатор объекта</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task RemoveAsync(int id, bool auto_save = true);");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("\t\t/// <summary>");
        await writer.WriteLineAsync($"\t\t/// Удалить перечень{(is_body_document ? "" : " строк табличной части")} документов: {doc_name}");
        await writer.WriteLineAsync("\t\t/// </summary>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"ids\">Идентификаторы объектов</param>");
        await writer.WriteLineAsync($"\t\t/// <param name=\"auto_save\">Автоматически/сразу сохранить изменения в БД</param>");
        await writer.WriteLineAsync($"\t\tpublic Task RemoveRangeAsync(IEnumerable<int> ids, bool auto_save = true);");
        await WriteEnd(writer);
    }
}