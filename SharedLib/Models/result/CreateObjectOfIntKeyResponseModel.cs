using Newtonsoft.Json;

namespace SharedLib
{
    /// <summary>
    /// Результат запроса создания объекта (ключ создаваемого объекта: int)
    /// </summary>
    public class CreateObjectOfIntKeyResponseModel : ResponseBaseModel
    {
        /// <summary>
        /// Идентификатор нового созданного объекта
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}