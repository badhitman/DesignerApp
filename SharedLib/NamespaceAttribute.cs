////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SharedLib;

/// <summary>
/// Валидация корректности пространства имён: точка, правый слеш или левый.
/// </summary>
public partial class NamespaceAttribute : ValidationAttribute
{
    /// <summary>
    /// Режим сегментации пространства имён
    /// </summary>
    /// <remarks>
    /// Пространство имён делится на сегменты обычно точкой, но в данном решении в дополнительно можно использовать другие разделители: правый или левый слеш.
    /// Если указать null, тогда все три символа могут быть использованы как разделители пути на сегменты
    /// </remarks>
    public SegmentationsNamespaceModesEnum Mode { get; set; } = SegmentationsNamespaceModesEnum.DotOnly;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string ns && CheckNS(ns))
            return true;

        ErrorMessage = "Некорректное пространство имён (namespace). ";

        switch (Mode)
        {
            case SegmentationsNamespaceModesEnum.Any:
                ErrorMessage += $"Разрешается один из символов: {Mode.DescriptionInfo()}";
                break;
            case SegmentationsNamespaceModesEnum.DotOnly:
                ErrorMessage += "Разрешается только точка";
                break;
            case SegmentationsNamespaceModesEnum.RightSlash:
                ErrorMessage += "Разрешается только правый слеш (верх наклонен вправо)";
                break;
            case SegmentationsNamespaceModesEnum.BackSlash:
                ErrorMessage += "Разрешается только левый (обратный) слеш (верх наклонен влево)";
                break;
        }

        return false;
    }

    bool CheckNS(string ns)
    {
        if (string.IsNullOrEmpty(ns))
            return false;

        return Mode switch
        {
            SegmentationsNamespaceModesEnum.BackSlash => Split(ns, Mode).All(x => MyRegexSystemName().IsMatch(x)),
            SegmentationsNamespaceModesEnum.RightSlash => Split(ns, Mode).All(x => MyRegexSystemName().IsMatch(x)),
            SegmentationsNamespaceModesEnum.DotOnly => Split(ns, Mode).All(x => MyRegexSystemName().IsMatch(x)),
            SegmentationsNamespaceModesEnum.Any => Split(ns, Mode).All(x => MyRegexSystemName().IsMatch(x)),
            _ => throw new Exception(),
        };
    }

    /// <summary>
    /// Нарезка пространства имён на сегменты
    /// </summary>
    public static string[] Split(string nameSpace, SegmentationsNamespaceModesEnum mode)
        => mode switch
        {
            SegmentationsNamespaceModesEnum.BackSlash => nameSpace.Split('\\'),
            SegmentationsNamespaceModesEnum.RightSlash => nameSpace.Split('/'),
            SegmentationsNamespaceModesEnum.DotOnly => nameSpace.Split('.'),
            SegmentationsNamespaceModesEnum.Any => SegmentationsSymbolsMyRegex().Split(nameSpace),
            _ => throw new Exception()
        };

    /// <summary>
    /// Нормализация пространства имён: заменяет все возможные разделители на один ваш
    /// </summary>
    public static string NormalizeNameSpice(string raw_name_spice, SegmentationsNamespaceModesEnum set_segmentation_symbol)
        => SegmentationsSymbolsMyRegex().Replace(raw_name_spice, set_segmentation_symbol.DescriptionInfo());

    [GeneratedRegex(GlobalStaticConstants.SYSTEM_NAME_TEMPLATE, RegexOptions.Compiled)]
    private static partial Regex MyRegexSystemName();

    [GeneratedRegex(@"[\./\\]")]
    public static partial Regex SegmentationsSymbolsMyRegex();

    /// <summary>
    /// Режимы сегментации пространства имён: точка, правый слеш или левый.
    /// </summary>
    /// <remarks>Пространство имён делится на сегменты обычно точкой, но в данном решении в дополнительно можно использовать другие разделители: правый или левый слеш</remarks>
    public enum SegmentationsNamespaceModesEnum
    {
        /// <summary>
        /// Любой из допустимых символов
        /// </summary>
        [Description(".\\/")]
        Any,

        /// <summary>
        /// Точка - стандартный типовой разделитель пространств имён
        /// </summary>
        [Description(".")]
        DotOnly,

        /// <summary>
        /// Левый (обратный) слеш (косая черта, наклонённая сверху влево)
        /// </summary>
        [Description("\\")]
        BackSlash,

        /// <summary>
        /// Правый (стандартный) слеш (косая черта, наклонённая сверху вправо)
        /// </summary>
        [Description("/")]
        RightSlash,
    }
}