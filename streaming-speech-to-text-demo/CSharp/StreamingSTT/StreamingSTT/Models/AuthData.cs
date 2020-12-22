using Newtonsoft.Json;

namespace StreamingSTT.Models
{
    class AuthData
    {
        [JsonProperty("accessKey")]
        public string accessKey { get; set; }

        [JsonProperty("nonce")]
        public string nonce { get; set; }

        [JsonProperty("signature")]
        public string signature { get; set; }

        [JsonProperty("remoteurl")]
        public string remoteUrl { get; set; }

        [JsonProperty("contractId")]
        public string contractId { get; set; }
    }
}
