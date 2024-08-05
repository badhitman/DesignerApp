////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public class TPaginationStrictResponseModel<T> : PaginationResponseModel
{
    /// <inheritdoc/>
    public TPaginationStrictResponseModel() { }
    /// <inheritdoc/>
    public TPaginationStrictResponseModel(PaginationRequestModel req)
    {
        PageNum = req.PageNum;
        PageSize = req.PageSize;
        SortingDirection = req.SortingDirection;
        SortBy = req.SortBy;
    }

    /// <inheritdoc/>
    public required List<T> Response { get; set; }
}