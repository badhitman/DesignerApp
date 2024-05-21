////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace SharedLib;

/// <summary>
/// Глобальные утилиты
/// </summary>
public static class GlobalUtils
{
    /// <summary>
    /// Перемешать список элементов
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Добавить информация об исключении
    /// </summary>
    public static void InjectException(this List<ResultMessage> sender, Exception ex)
    {
        sender.Add(new() { TypeMessage = ResultTypesEnum.Error, Text = ex.Message });
        if (ex.StackTrace != null)
            sender.Add(new() { TypeMessage = ResultTypesEnum.Error, Text = ex.StackTrace });
        int i = 0;
        while (ex.InnerException != null)
        {
            i++;
            sender.Add(new() { TypeMessage = ResultTypesEnum.Error, Text = $"InnerException -> {i}/ {ex.InnerException.Message}" });
            if (ex.InnerException.StackTrace != null)
                sender.Add(new() { TypeMessage = ResultTypesEnum.Error, Text = $"InnerException -> {i}/ {ex.InnerException.StackTrace}" });

            ex = ex.InnerException;
        }
    }

    /// <summary>
    /// Русская (ru-RU) CultureInfo
    /// </summary>
    public static CultureInfo RU => CultureInfo.GetCultureInfo("ru-RU");


    /// <summary>
    /// Преобразовать размер файла в читаемый вид
    /// </summary>
    public static string SizeDataAsString(long SizeFile)
    {
        if (SizeFile < 1024)
            return SizeFile.ToString() + " bytes";
        else if (SizeFile < 1024 * 1024)
            return Math.Round((double)SizeFile / 1024, 2).ToString() + " KB";
        else if (SizeFile < 1024 * 1024 * 1024)
            return Math.Round((double)SizeFile / 1024 / 1024, 2).ToString() + " MB";
        else
            return Math.Round((double)SizeFile / 1024 / 1024 / 1024, 3).ToString() + " GB";
    }

    /// <summary>
    /// Клон объекта (через сереализацию)
    /// </summary>
    public static T CreateDeepCopy<T>(T obj)
    {
        using MemoryStream ms = new();
        XmlSerializer serializer = new(obj!.GetType());
        serializer.Serialize(ms, obj);
        ms.Seek(0, SeekOrigin.Begin);
        return (T)serializer.Deserialize(ms)!;
    }


    #region
    /// <summary>
    /// Вычислить Хеш строки
    /// </summary>
    public static string CalculateHashString(string password)
    {
        byte[] bytes = new UTF8Encoding().GetBytes(password);
        byte[] hashBytes = System.Security.Cryptography.MD5.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Генерация пароля
    /// </summary>
    /// <param name="length">Длинна пароля</param>
    public static string CreatePassword(int length) => new PasswordGenerator(minimumLengthPassword: length, maximumLengthPassword: length).Generate();
    #endregion

    /// <summary>
    /// Транслит словарь
    /// </summary>
    public static readonly Dictionary<char, string> TranslitMap = new() { {'q', "й"}, {'w', "ц"}, {'e', "у"}, {'r', "к"}, {'t', "е"}, {'y', "н"},
        {'u', "г"}, {'i', "ш"}, {'o', "щ"}, {'p', "з"}, {'[', "х"}, {']', "ъ"}, { 'a', "ф"}, {'s', "ы"}, {'d', "в"}, {'f', "а"}, {'g', "п"},
        {'h', "р"}, {'j', "о"}, {'k', "л"}, {'l', "д"}, {';', "ж"}, {'\'', "э"}, { 'z', "я"}, {'x', "ч"}, {'c', "с"}, {'v', "м"}, {'b', "и"},
        {'n', "т"}, {'m', "ь"}, {',', "б"}, {'.', "ю"}, { 'Q', "Й"}, {'W', "Ц"}, {'E', "У"}, {'R', "К"}, {'T', "Е"}, {'Y', "Н"}, {'U', "Г"},
        {'I', "Ш"}, {'O', "Щ"}, {'P', "З"}, { 'A', "Ф"}, {'S', "Ы"}, {'D', "В"}, {'F', "А"}, {'G', "П"}, {'H', "Р"}, {'J', "О"}, {'K', "Л"},
        {'L', "Д"}, {'Z', "Я"}, {'X', "Ч"}, {'C', "С"}, {'V', "М"}, {'B', "И"}, {'N', "Т"}, {'M', "Ь"},

        {'й', "q"}, {'ц', "w"}, {'у', "e"}, {'к', "r"}, {'е', "t"}, {'н', "y"},
        {'г', "u"}, {'ш', "i"}, {'щ', "o"}, {'з', "p"}, {'х', "["}, {'ъ', "]"}, { 'ф', "a"}, {'ы', "s"}, {'в', "d"}, {'а', "f"}, {'п', "g"},
        {'р', "h"}, {'о', "j"}, {'л', "k"}, {'д', "l"}, {'ж', ";"}, {'э', "\'"}, { 'я', "z"}, {'ч', "x"}, {'с', "c"}, {'м', "v"}, {'и', "b"},
        {'т', "n"}, {'ь', "m"}, {'б', ","}, {'ю', "."}, { 'Й', "Q"}, {'Ц', "W"}, {'У', "E"}, {'К', "R"}, {'Е', "T"}, {'Н', "Y"}, {'Г', "U"},
        {'Ш', "I"}, {'Щ', "O"}, {'З', "P"}, { 'Ф', "A"}, {'Ы', "S"}, {'В', "D"}, {'А', "F"}, {'П', "G"}, {'Р', "H"}, {'О', "J"}, {'Л', "K"},
        {'Д', "L"}, {'Я', "Z"}, {'Ч', "X"}, {'С', "C"}, {'М', "V"}, {'И', "B"}, {'Т', "N"}, {'Ь', "M"}
    };

    /// <summary>
    /// Транслит строки
    /// </summary>
    public static string GetTranslitString(string inc_data)
    {
        StringBuilder res = new();
        foreach (char kc in inc_data)
        {
            if (!TranslitMap.TryGetValue(kc, out string? tc) || string.IsNullOrEmpty(tc))
                break;
            res.Append(tc);
        }

        return res.ToString();
    }

    /// <summary>
    /// файл является изображением
    /// </summary>
    public static bool IsImageFile(string file_tag)
    {
        return file_tag.EndsWith("GIF", StringComparison.OrdinalIgnoreCase) || file_tag.EndsWith("JPEG", StringComparison.OrdinalIgnoreCase) || file_tag.EndsWith("PNG", StringComparison.OrdinalIgnoreCase) || file_tag.EndsWith("BMP", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Получить значение свойства анонимного типа (приведённого к object)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">объект анонимного типа</param>
    /// <param name="property_name">Имя свойства, которое требуется получить</param>
    /// <returns>Значение свойства (или null)</returns>
    public static T? GetPropertyValue<T>(this object obj, string property_name)
    {
        return (T?)obj.GetType().GetProperty(property_name)?.GetValue(obj, null);
    }

    /// <summary>
    /// Получить значение атрибута Description
    /// </summary>
    public static string DescriptionInfo(this Enum enumValue)
    {
        foreach (FieldInfo field in enumValue.GetType().GetFields())
        {
            var descriptionAttribute = field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            if (descriptionAttribute != null && field.Name.Equals(enumValue.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return descriptionAttribute.Description;
            }
        }

        return enumValue.ToString();
    }

    /// <summary>
    /// Получить тип по его имени
    /// </summary>
    public static Type? GetType(string strFullyQualifiedName)
    {
        Type? type = Type.GetType(strFullyQualifiedName);
        if (type is not null)
            return type;
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = asm.GetType(strFullyQualifiedName);
            if (type != null)
                return type;
        }
        return null;
    }

    /// <summary>
    /// Является ли объект коллекцией элементов
    /// </summary>
    public static bool IsEnumerableType(Type type) => type != typeof(string) && type.GetInterfaces().Any(s => s.Name.StartsWith("IEnumerable"));

    /// <summary>
    /// Проверка валидности JSON строки к типу данных
    /// </summary>
    public static bool ValidateJson<T>(string json_src)
    {
        if (string.IsNullOrWhiteSpace(json_src))
            return false;

        try
        {
            _ = JsonConvert.DeserializeObject<T>(json_src);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Соответствия типов данных с расширениями файлов
    /// </summary>
    public static readonly Dictionary<string, string[]> ContentTypes = new()
    {
        {"text/html", new[]{"html", "htm", "shtml"}},
        {"text/css", new[]{"css"}},
        {"text/xml", new[]{"xml"}},
        {"image/gif", new[]{"gif"}},
        {"image/jpeg", new[]{"jpeg", "jpg"}},
        {"application/x-javascript", new[]{"js"}},
        {"application/atom+xml", new[]{"atom"}},
        {"application/rss+xml", new[]{"rss"}},
        {"text/mathml", new[]{"mml"}},
        {"text/plain", new[]{"txt"}},
        {"text/vnd.sun.j2me.app-descriptor", new[]{"jad"}},
        {"text/vnd.wap.wml", new[]{"wml"}},
        {"text/x-component", new[]{"htc"}},
        {"image/png", new[]{"png"}},
        {"image/tiff", new[]{"tif", "tiff"}},
        {"image/vnd.wap.wbmp", new[]{"wbmp"}},
        {"image/x-icon", new[]{"ico"}},
        {"image/x-jng", new[]{"jng"}},
        {"image/x-ms-bmp", new[]{"bmp"}},
        {"image/svg+xml", new[]{"svg"}},
        {"image/webp", new[]{"webp"}},
        {"application/java-archive", new[]{"jar", "war", "ear"}},
        {"application/mac-binhex40", new[]{"hqx"}},
        {"application/msword", new[]{"doc"}},
        {"application/pdf", new[]{"pdf"}},
        {"application/postscript", new[]{"ps", "eps", "ai"}},
        {"application/rtf", new[]{"rtf"}},
        {"application/vnd.ms-excel", new[]{"xls"}},
        {"application/vnd.ms-powerpoint", new[]{"ppt"}},
        {"application/vnd.wap.wmlc", new[]{"wmlc"}},
        {"application/vnd.google-earth.kml+xml", new[]{"kml"}},
        {"application/vnd.google-earth.kmz", new[]{"kmz"}},
        {"application/x-7z-compressed", new[]{"7z"}},
        {"application/x-cocoa", new[]{"cco"}},
        {"application/x-java-archive-diff", new[]{"jardiff"}},
        {"application/x-java-jnlp-file", new[]{"jnlp"}},
        {"application/x-makeself", new[]{"run"}},
        {"application/x-perl", new[]{"pl", "pm"}},
        {"application/x-pilot", new[]{"prc", "pdb"}},
        {"application/x-rar-compressed", new[]{"rar"}},
        {"application/x-redhat-package-manager", new[]{"rpm"}},
        {"application/x-sea", new[]{"sea"}},
        {"application/x-shockwave-flash", new[]{"swf"}},
        {"application/x-stuffit", new[]{"sit"}},
        {"application/x-tcl", new[]{"tcl", "tk"}},
        {"application/x-x509-ca-cert", new[]{"der", "pem", "crt"}},
        {"application/x-xpinstall", new[]{"xpi"}},
        {"application/xhtml+xml", new[]{"xhtml"}},
        {"application/zip", new[]{"zip"}},
        {"application/octet-stream", new[]{"bin", "exe", "dll","deb","dmg","eot","iso", "img","msi", "msp", "msm"}},
        {"audio/midi", new[]{"mid", "midi", "kar"}},
        {"audio/mpeg", new[]{"mp3"}},
        {"audio/ogg", new[]{"ogg"}},
        {"audio/x-realaudio", new[]{"ra"}},
        {"video/3gpp", new[]{"3gpp", "3gp"}},
        {"video/mpeg", new[]{"mpeg", "mpg"}},
        {"video/quicktime", new[]{"mov"}},
        {"video/x-flv", new[]{"flv"}},
        {"video/x-mng", new[]{"mng"}},
        {"video/x-ms-asf", new[]{"asx", "asf"}},
        {"video/x-ms-wmv", new[]{"wmv"}},
        {"video/x-msvideo", new[]{"avi"}},
        {"video/mp4", new[]{"m4v", "mp4"} }
    };
}