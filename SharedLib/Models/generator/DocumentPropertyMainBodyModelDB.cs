////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLib.Models;

/// <summary>
/// Поле/свойство (основное) в теле документа
/// </summary>
[Table("DesignDocumentsMainBodyProperties")]
[Comment("Реквизиты документов (основное тело документа)")]
public class DocumentPropertyMainBodyModelDB : DocumentPropertyMainModelDB
{
    
}