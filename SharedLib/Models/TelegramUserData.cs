using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// TelegramUserData
/// </summary>
public class TelegramUserData
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public long Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("first_name")]
    public required string FirstName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("last_name")]
    public string? LastName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("username")]
    public string? UserName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("language_code")]
    public string? LanguageCode { get; set; }
    /// <summary>
    /// Optional. True, if this user allowed the bot to message them.
    /// </summary>
    [JsonProperty("allows_write_to_pm")]
    public string? AllowsWriteToPm { get; set; }
    /// <summary>
    /// Optional. True, if this user is a bot. Returns in the receiver field only.
    /// </summary>
    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }
}