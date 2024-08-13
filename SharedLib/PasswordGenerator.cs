using System.Security.Cryptography;

namespace SharedLib;

/// <summary>
/// Генерация пароля
/// </summary>
public class PasswordGenerator
{
    /// <inheritdoc/>
    public int MinimumLengthPassword { get; private set; }
    /// <inheritdoc/>
    public int MaximumLengthPassword { get; private set; }
    /// <inheritdoc/>
    public int MinimumLowerCaseChars { get; private set; }
    /// <inheritdoc/>
    public int MinimumUpperCaseChars { get; private set; }
    /// <inheritdoc/>
    public int MinimumNumericChars { get; private set; }
    /// <inheritdoc/>
    public int MinimumSpecialChars { get; private set; }

    /// <inheritdoc/>
    public static string AllLowerCaseChars { get; private set; }
    /// <inheritdoc/>
    public static string AllUpperCaseChars { get; private set; }
    /// <inheritdoc/>
    public static string AllNumericChars { get; private set; }
    /// <inheritdoc/>
    public static string AllSpecialChars { get; private set; }
    private readonly string _allAvailableChars;

    private readonly RandomSecureVersion _randomSecure = new RandomSecureVersion();
    private readonly int _minimumNumberOfChars;

    static PasswordGenerator()
    {
        // Define characters that are valid and reject ambiguous characters such as ilo, IO and 1 or 0
        AllLowerCaseChars = GetCharRange('a', 'z', exclusiveChars: "ilo");
        AllUpperCaseChars = GetCharRange('A', 'Z', exclusiveChars: "IO");
        AllNumericChars = GetCharRange('2', '9');
        AllSpecialChars = "@$%+-()*!^&";

    }

    /// <inheritdoc/>
    public PasswordGenerator(
        int minimumLengthPassword = 10,
        int maximumLengthPassword = 10,
        int minimumLowerCaseChars = 2,
        int minimumUpperCaseChars = 2,
        int minimumNumericChars = 2,
        int minimumSpecialChars = 2)
    {
        if (minimumLengthPassword > maximumLengthPassword)
        {
            throw new ArgumentException("The minimumLength is bigger than the maximum length.",
                nameof(minimumLengthPassword));
        }

        if (minimumLowerCaseChars < 2)
        {
            throw new ArgumentException("The minimumLowerCase is smaller than 2.",
                nameof(minimumLowerCaseChars));
        }

        if (minimumUpperCaseChars < 2)
        {
            throw new ArgumentException("The minimumUpperCase is smaller than 2.",
                nameof(minimumUpperCaseChars));
        }

        if (minimumNumericChars < 2)
        {
            throw new ArgumentException("The minimumNumeric is smaller than 2.",
                nameof(minimumNumericChars));
        }

        if (minimumSpecialChars < 2)
        {
            throw new ArgumentException("The minimumSpecial is smaller than 2.",
                nameof(minimumSpecialChars));
        }

        _minimumNumberOfChars = minimumLowerCaseChars + minimumUpperCaseChars +
                                minimumNumericChars + minimumSpecialChars;

        if (minimumLengthPassword < _minimumNumberOfChars)
        {
            throw new ArgumentException(
                "The minimum length of the password is smaller than the sum " +
                "of the minimum characters of all categories.",
                nameof(maximumLengthPassword));
        }

        MinimumLengthPassword = minimumLengthPassword;
        MaximumLengthPassword = maximumLengthPassword;

        MinimumLowerCaseChars = minimumLowerCaseChars;
        MinimumUpperCaseChars = minimumUpperCaseChars;
        MinimumNumericChars = minimumNumericChars;
        MinimumSpecialChars = minimumSpecialChars;

        _allAvailableChars =
            OnlyIfOneCharIsRequired(minimumLowerCaseChars, AllLowerCaseChars) +
            OnlyIfOneCharIsRequired(minimumUpperCaseChars, AllUpperCaseChars) +
            OnlyIfOneCharIsRequired(minimumNumericChars, AllNumericChars) +
            OnlyIfOneCharIsRequired(minimumSpecialChars, AllSpecialChars);
    }

    private string OnlyIfOneCharIsRequired(int minimum, string allChars)
    {
        return minimum > 0 || _minimumNumberOfChars == 0 ? allChars : string.Empty;
    }

    /// <inheritdoc/>
    public string Generate()
    {
        int lengthOfPassword = _randomSecure.Next(MinimumLengthPassword, MaximumLengthPassword);

        // Get the required number of characters of each category and 
        // add random characters of all categories
        string minimumChars = GetRandomString(AllLowerCaseChars, MinimumLowerCaseChars) +
                        GetRandomString(AllUpperCaseChars, MinimumUpperCaseChars) +
                        GetRandomString(AllNumericChars, MinimumNumericChars) +
                        GetRandomString(AllSpecialChars, MinimumSpecialChars);
        string rest = GetRandomString(_allAvailableChars, lengthOfPassword - minimumChars.Length);
        string unshuffeledResult = minimumChars + rest;

        // Shuffle the result so the order of the characters are unpredictable
        string result = unshuffeledResult.ShuffleTextSecure();
        return result;
    }

    private string GetRandomString(string possibleChars, int lenght)
    {
        string result = string.Empty;
        for (int position = 0; position < lenght; position++)
        {
            int index = _randomSecure.Next(possibleChars.Length);
            result += possibleChars[index];
        }
        return result;
    }

    private static string GetCharRange(char minimum, char maximum, string exclusiveChars = "")
    {
        string result = string.Empty;
        for (char value = minimum; value <= maximum; value++)
        {
            result += value;
        }
        if (!string.IsNullOrEmpty(exclusiveChars))
        {
            char[] inclusiveChars = result.Except(exclusiveChars).ToArray();
            result = new string(inclusiveChars);
        }
        return result;
    }
}

/// <summary>
/// ext
/// </summary>
public static class Extensions
{
    private static readonly Lazy<RandomSecureVersion> RandomSecure =
        new(() => new RandomSecureVersion());
    /// <summary>
    /// ShuffleSecure
    /// </summary>
    public static IEnumerable<T> ShuffleSecure<T>(this IEnumerable<T> source)
    {
        T[] sourceArray = source.ToArray();
        for (int counter = 0; counter < sourceArray.Length; counter++)
        {
            int randomIndex = RandomSecure.Value.Next(counter, sourceArray.Length);
            yield return sourceArray[randomIndex];

            sourceArray[randomIndex] = sourceArray[counter];
        }
    }

    /// <summary>
    /// ShuffleTextSecure
    /// </summary>
    public static string ShuffleTextSecure(this string source)
    {
        char[] shuffeldChars = source.ShuffleSecure().ToArray();
        return new(shuffeldChars);
    }
}

internal class RandomSecureVersion
{
    //Never ever ever never use Random() in the generation of anything that requires true security/randomness
    //and high entropy or I will hunt you down with a pitchfork!! Only RNGCryptoServiceProvider() is safe.
    private readonly RandomNumberGenerator _rngProvider = RandomNumberGenerator.Create();

    public int Next()
    {
        byte[] randomBuffer = new byte[4];
        _rngProvider.GetBytes(randomBuffer);
        int result = BitConverter.ToInt32(randomBuffer, 0);
        return result;
    }

    public int Next(int maximumValue)
    {
        // Do not use Next() % maximumValue because the distribution is not OK
        return Next(0, maximumValue);
    }

    public int Next(int minimumValue, int maximumValue)
    {
        int seed = Next();

        //  Generate uniformly distributed random integers within a given range.
        return new Random(seed).Next(minimumValue, maximumValue);
    }
}