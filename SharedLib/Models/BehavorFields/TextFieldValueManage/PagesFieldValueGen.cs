﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Перечень вкладок опроса/анкеты
/// </summary>
public class PagesFieldValueGen : FieldValueGeneratorAbstraction
{
    /// <inheritdoc/>
    public override string Name => "Вкладки анкеты";

    /// <inheritdoc/>
    public override string? About => "Получить перечень имён вклдок опроса/анкеты. Будет генерироваться список из имён вкладок текущего опроса";

    /// <inheritdoc/>
    public override bool AllowCallWithoutParameters => true;

    /// <inheritdoc/>
    public override TResponseModel<string[]> GetListElements(FieldFormConstructorModelDB field, SessionOfDocumentDataModelDB session_Document, TabJoinDocumentSchemeConstructorModelDB? page_join_form = null, uint row_num = 0)
    {
        return new() { Response = session_Document.Owner?.Pages?.OrderBy(x => x.SortIndex).Select(x => x.Name).Distinct().ToArray() };
    }
}