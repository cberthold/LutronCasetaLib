using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses.Ping
{
    public partial class PingResponseBody : IResponseBody
    {
        [JsonProperty("PingResponse")]
        public PingResponse PingResponse { get; set; }
    }
}
