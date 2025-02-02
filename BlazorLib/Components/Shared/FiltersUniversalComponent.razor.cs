////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;

namespace BlazorLib.Components.Shared;

/// <summary>
/// FiltersUniversalComponent
/// </summary>
public partial class FiltersUniversalComponent
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Dictionary<string, int> FiltersAvailable { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required string Title { get; set; }

    /// <summary>
    /// CheckedChangedHandle
    /// </summary>
    [Parameter, EditorRequired]
    public required Action CheckedChangedHandle { get; set; }

    IEnumerable<string>? options = [];
    IEnumerable<string>? _options
    {
        get => options;
        set
        {
            options = value;
            CheckedChangedHandle();
        }
    }

    string _value = "Не выбрано";
    string[] States => FiltersAvailable.Select(x => x.Key).Distinct().ToArray();

    /// <summary>
    /// Получить все выбранные значения словаря
    /// </summary>
    public List<string> GetSelected()
    {
        return _options?.ToList() ?? [];
    }

}