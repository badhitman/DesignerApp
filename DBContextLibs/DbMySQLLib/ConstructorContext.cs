////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbLayerLib;
using Microsoft.EntityFrameworkCore;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public partial class ConstructorContext(DbContextOptions<ConstructorContext> options) : ConstructorLayerContext(options)
{

}