﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Генератор значений для выбора клиентом в специализированном контроле
/// </summary>
public abstract class FieldValueGeneratorAbstraction : DeclarationAbstraction
{
    /// <summary>
    /// Элементы, полученные от генератора
    /// </summary>
    public abstract TResponseModel<string[]> GetListElements(FieldFormConstructorModelDB field, SessionOfDocumentDataModelDB session_Document, FormToTabJoinConstructorModelDB? page_join_form = null, uint row_num = 0);
}