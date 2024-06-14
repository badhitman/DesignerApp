using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// Декларативная модель
/// </summary>
public abstract class DeclarationAbstraction
{
    /// <summary>
    /// Название
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Описание
    /// </summary>
    public abstract string? About { get; }

    /// <summary>
    /// Все программные калькуляции
    /// </summary>
    public static IEnumerable<EntryAltDescriptionModel> CommandsAsEntries<T>()
    {
        Type _current_type = typeof(T);
        IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => _current_type.IsAssignableFrom(p) && _current_type != p && !p.IsAbstract && !p.IsInterface);

        return types.Select(x =>
        {
            if (Activator.CreateInstance(x) is not T obj)
                throw new Exception("error 919F8FF2-B902-4112-8680-67352F369F0C");

            if (obj is not DeclarationAbstraction _set)
                throw new Exception("error EF8D4F4A-F578-44C6-B78C-BA7685662938");

            return new EntryAltDescriptionModel() { Id = x.Name, Name = _set.Name, Description = _set.About };
        });
    }

    static readonly Dictionary<string, CommandsAsEntriesModel> ParseCalculationsAsEntriesCache = new();

    /// <summary>
    /// Кэш деклараций
    /// </summary>
    protected static readonly Dictionary<string, DeclarationAbstraction> _calc_cache = new();
    /// <summary>
    /// Получить службу-обработчик
    /// </summary>
    /// <param name="type_name">имя калькулятора</param>
    /// <exception cref="Exception">Ошибка в имени калькулятора или требуемый калькулятор находится в NS отличном от основного</exception>
    public static DeclarationAbstraction? GetHandlerService(string type_name)
    {
        if (string.IsNullOrWhiteSpace(type_name))
            return null;

        DeclarationAbstraction? _c;
        lock (_calc_cache)
        {
            if (_calc_cache.TryGetValue(type_name, out DeclarationAbstraction? _vcc))
                return _vcc;

            Type? t = GlobalTools.GetType($"{typeof(DeclarationAbstraction).Namespace}.{type_name}");
            if (t is null)
                return null;

            _c = Activator.CreateInstance(t) as DeclarationAbstraction;
            if (_c is not null)
                _calc_cache.Add(type_name, _c);
        }

        return _c;
    }

    /// <summary>
    /// Парсить строку JSON в команду
    /// </summary>
    public static CommandsAsEntriesModel? ParseCommandsAsEntries(string json_data)
    {
        if (string.IsNullOrWhiteSpace(json_data))
            return null;

        lock (ParseCalculationsAsEntriesCache)
        {
            if (ParseCalculationsAsEntriesCache.TryGetValue(json_data, out CommandsAsEntriesModel? c))
                return c;

            try
            {
                Dictionary<string, string>? kvp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_data);
                if (kvp is not null)
                {
                    string[]? pd = JsonConvert.DeserializeObject<string[]>(kvp[nameof(MetadataExtensionsFormFieldsEnum.Parameter)]);
                    if (pd is not null)
                    {
                        CommandsAsEntriesModel _v = new()
                        {
                            CommandName = kvp[nameof(MetadataExtensionsFormFieldsEnum.Descriptor)],
                            Options = pd
                        };
                        ParseCalculationsAsEntriesCache.Add(json_data, _v);
                        return _v;
                    }
                }
            }
            catch
            {

            }
        }

        return null;
    }
}