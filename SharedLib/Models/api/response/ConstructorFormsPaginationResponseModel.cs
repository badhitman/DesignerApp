﻿namespace SharedLib;

/// <summary>
/// Формы (с пагинацией)
/// </summary>
public class ConstructorFormsPaginationResponseModel : PaginationResponseModel
{
    /// <summary>
    /// Формы (с пагинацией)
    /// </summary>
    public ConstructorFormsPaginationResponseModel() { }

    /// <summary>
    /// Формы (с пагинацией)
    /// </summary>
    public ConstructorFormsPaginationResponseModel(PaginationRequestModel req)  { }

    /// <summary>
    /// Формы
    /// </summary>
    public IEnumerable<ConstructorFormModelDB>? Elements { get; set; }
}