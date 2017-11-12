using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses.Ping
{
    public partial class PingResponse
    {
        [JsonProperty("LEAPVersion")]
        public double LEAPVersion { get; set; }
    }
}
