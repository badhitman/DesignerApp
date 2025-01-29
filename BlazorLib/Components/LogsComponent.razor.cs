////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components;

public partial class LogsComponent
{
    [Inject]
    ILogsService LogsRepo { get; set; } = default!;
}
