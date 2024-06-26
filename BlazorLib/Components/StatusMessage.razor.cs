////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components;

/// <inheritdoc/>
public partial class StatusMessage : ComponentBase
{
    /// <summary>
    /// Сообщения для вывода
    /// </summary>
    [Parameter]
    public IEnumerable<ResultMessage>? Messages { get; set; }

    /// <inheritdoc/>
    public static string getCssItem(ResultTypesEnum res_type) => res_type switch
    {
        ResultTypesEnum.Success => "success",
        ResultTypesEnum.Info => "primary",
        ResultTypesEnum.Warning => "warning",
        ResultTypesEnum.Error => "danger",
        ResultTypesEnum.Alert => "info",
        _ => "secondary"
    };
}