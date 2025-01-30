////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace DbcLib;

/// <summary>
/// Промежуточный/общий слой контекста базы данных
/// </summary>
public class NLogsContext(DbContextOptions<NLogsContext> options) : NLogsLayerContext(options)
{

}