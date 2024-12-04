////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TreeViewOptionsModel
/// </summary>
public class TreeViewOptionsModel
{
    /// <summary>
    /// SelectedValuesChangedHandler
    /// </summary>
    public required Action<IReadOnlyCollection<UniversalBaseModel?>> SelectedValuesChangedHandler { get; set; }

    /// <summary>
    /// SelectedNodes
    /// </summary>
    public required int[] SelectedNodes { get; set; }
}