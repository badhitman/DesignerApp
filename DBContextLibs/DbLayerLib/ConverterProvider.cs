using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections;

namespace DbLayerLib;

/// <summary>
/// ConverterProvider
/// </summary>
public static class ConverterProvider
{
    /// <summary>
    /// GetBoolToBitArrayConverter
    /// </summary>
    public static ValueConverter<bool, BitArray> GetBoolToBitArrayConverter()
    {
        return new ValueConverter<bool, BitArray>(
            value => new BitArray(new[] { value }),
            value => value.Get(0));
    }
}


