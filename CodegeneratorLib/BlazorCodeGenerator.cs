////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Blazor code-generator
/// </summary>
public abstract class BlazorCodeGenerator
{

}

/* 
    /// <summary>
    /// Записать вступление файла (для Blazor)
    /// </summary>
    async Task WriteHeadBlazor(StreamWriter writer, string title, string? description = null, IEnumerable<string>? using_ns = null)
    {
        await writer.WriteLineAsync($"@* Project: {project.Name} - by  © https://github.com/badhitman - @fakegov *@");
        await writer.WriteLineAsync();
        if (using_ns?.Any() == true)
        {
            foreach (string u in using_ns)
                await writer.WriteLineAsync($"{(u.StartsWith("@using ") ? u : $"@using {u}")}");

            await writer.WriteLineAsync();
        }
        await writer.WriteLineAsync("@inherits BlazorBusyComponentBaseModel");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("<div class=\"card\">");
        await writer.WriteLineAsync("\t<div class=\"card-body\">");
        await writer.WriteLineAsync($"\t\t<h5 class=\"card-title\">{title}</h5>");
        if (!string.IsNullOrWhiteSpace(description))
            await writer.WriteLineAsync($"\t\t<h6 class=\"card-subtitle mb-4 text-body-secondary\">{DescriptionHtmlToLinesRemark(description ?? "").FirstOrDefault()}</h6>");
    }

    static async Task WriteSegmentBlazor(StreamWriter writer)//, ParameterComponent[] parameters
    {
        await writer.WriteAsync("\t</div>");
        await writer.WriteAsync("</div>");
        await writer.WriteAsync("@code {");
    }

    /// <summary>
    /// Запись финальной части файла и закрытие потока записи (для Blazor)
    /// </summary>
    /// <param name="writer">Поток записи ZIP архива</param>
    static async Task WriteEndBlazor(StreamWriter writer)
    {
        await writer.WriteAsync("}");
        await writer.FlushAsync();
        writer.Close();
        await writer.DisposeAsync();
    }
 */