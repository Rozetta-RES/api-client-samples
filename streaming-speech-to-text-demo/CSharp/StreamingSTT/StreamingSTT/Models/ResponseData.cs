using Newtonsoft.Json;

namespace StreamingSTT.Models
{
    class ResponseData
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
    }
}
