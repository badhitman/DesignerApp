////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public class TPaginationResponseModel<T> : PaginationResponseModel
{
    /// <inheritdoc/>
    public TPaginationResponseModel() { }
    /// <inheritdoc/>
    public TPaginationResponseModel(PaginationRequestModel req)
    {
        PageNum = req.PageNum;
        PageSize = req.PageSize;
        SortingDirection = req.SortingDirection;
        SortBy = req.SortBy;
    }

    /// <inheritdoc/>
    public required List<T> Response { get; set; }
}