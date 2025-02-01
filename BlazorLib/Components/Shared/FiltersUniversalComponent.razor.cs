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

    /// <summary>
    /// Получить все выбранные значения словаря
    /// </summary>
    public List<string> GetSelected()
    {
        List<string> res = [];

        if(_included.All(x=>x) || _included.All(x => !x))
            return res;

        int indexItem = -1;
        foreach(KeyValuePair<string, int> x in FiltersAvailable)
        {
            indexItem++;
            if (_included[indexItem])
                res.Add(x.Key);
        }

        return res;
    }

    /// <summary>
    /// Получить сводку (метаданные) по пространствам хранилища
    /// </summary>
    /// <remarks>
    /// Общий размер и количество группируется по AppName
    /// </remarks>
    private readonly List<bool> _included = [];
    //int _i = -1;


    void OnCheckedChanged(bool e, int _ind)
    {
        _included[_ind] = e;
        //_i = -1;
        StateHasChanged();
        CheckedChangedHandle();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        foreach (KeyValuePair<string, int> item in FiltersAvailable)
        {
            _included.Add(false);
        }
    }
}