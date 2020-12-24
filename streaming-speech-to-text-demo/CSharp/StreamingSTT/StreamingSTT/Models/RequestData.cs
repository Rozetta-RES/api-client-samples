using Newtonsoft.Json;

namespace StreamingSTT.Models
{
    class RequestData
    {
        [JsonProperty("command")]
        public string command { get; set; }

        [JsonProperty("value")]
        public object value { get; set; }
    }
}
