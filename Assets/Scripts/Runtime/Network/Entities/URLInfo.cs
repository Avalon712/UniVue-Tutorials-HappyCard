using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace HappyCard.Network.Entities
{
    public struct URLInfo
    {
        public string url { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HttpMethod method { get; private set; }

        public URLInfo(string url, HttpMethod method)
        {
            this.url = url; this.method = method;
        }
    }
}
