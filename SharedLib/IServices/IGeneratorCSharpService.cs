////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SharedLib.Models;
using System.IO.Compression;

namespace SharedLib.Services;

/// <summary>
/// Генератор C# комплекта
/// </summary>
public interface IGeneratorCSharpService
{
    /// <summary>
    /// Получить данные файла
    /// </summary>
    public Task<Stream> GetFullStream(StructureProjectModel dump, CodeGeneratorConfigModel conf);

    /// <summary>
    /// Readme.txt - генерация файла
    /// </summary>
    /// <param name="archive">Объект архива для записи данных</param>
    /// <param name="project_info">Информация о проекте</param>
    /// <param name="write_lines">Строки дя записи в readme</param>
    public Task ReadmeGen(ZipArchive archive, IEnumerable<string> write_lines, NameSpacedModel project_info);

    /// <summary>
    /// Генерация перечислений
    /// </summary>
    /// <param name="enums">Данные перечислений для записи</param>
    /// <param name="archive">Объект архива для записи данных</param>
    /// <param name="dir">Путь (папка) для размещения файлов перечислений</param>
    /// <param name="project_info">Информация о проекте</param>
    public Task EnumsGen(IEnumerable<EnumFitModel> enums, ZipArchive archive, string dir, NameSpacedModel project_info);

    /// <summary>
    /// Генерация документов (типов/классов/моделей)
    /// </summary>
    /// <param name="docs">Данные документов для записи</param>
    /// <param name="archive">Объект архива для записи данных</param>
    /// <param name="dir">Путь (папка) для размещения файлов документов</param>
    /// <param name="project_info">Информация о проекте</param>
    public Task DocumentsShemaGen(IEnumerable<DocumentFitModel> docs, ZipArchive archive, string dir, NameSpacedModel project_info);

    /// <summary>
    /// Генерация контекста
    /// </summary>
    /// <param name="docs">Перечень документов</param>
    /// <param name="archive">Объект архива для записи данных</param>
    /// <param name="project_info">Информация о проекте</param>
    public Task DbContextGen(IEnumerable<DocumentFitModel> docs, ZipArchive archive, NameSpacedModel project_info);

    /// <summary>
    /// Генерация точек доступа к таблицам БД (серверные интерфейсы служб и их реализация. контроллеры. refit - клиентская инфраструктура сервисов)
    /// </summary>
    /// <param name="docs">Перечень документов</param>
    /// <param name="archive">Объект архива для записи данных</param>
    /// <param name="dir">Путь (папка) для размещения файлов</param>
    /// <param name="project_info">Информация о проекте</param>
    /// <param name="controllers_directory_path">Папка размещения файлов api/rest контроллеров</param>
    public Task DbTableAccessGen(IEnumerable<DocumentFitModel> docs, ZipArchive archive, string dir, NameSpacedModel project_info, string controllers_directory_path);
}
