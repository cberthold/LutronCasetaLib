using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses
{
    public partial class Header
    {
        [JsonProperty("MessageBodyType")]
        public string MessageBodyType { get; set; }

        [JsonProperty("StatusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }
    }
}
