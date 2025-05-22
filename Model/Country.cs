using Newtonsoft.Json;

namespace AppSalas.Model
{
    public class Country
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dial_code")]
        public string DialCode { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("emoji")]
        public string FlagEmoji { get; set; }

        [JsonProperty("cities")]

        public List<string> Cities { get; set; }

        // Propiedad calculada para el Picker
        [JsonIgnore]
        public string DisplayName => $"{FlagEmoji} {Name} ({DialCode})";
    }
}
