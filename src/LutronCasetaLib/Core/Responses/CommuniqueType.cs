using LutronCaseta.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses
{
    public partial class CommuniqueType
    {
        [JsonProperty("Body")]
        public IResponseBody Body { get; set; }

        [JsonProperty("Header")]
        public Header Header { get; set; }

        [JsonProperty("CommuniqueType")]
        public string PurpleCommuniqueType { get; set; }
    }

    public partial class CommuniqueType
    {
        public static CommuniqueType FromJson(string json) => JsonConvert.DeserializeObject<CommuniqueType>(json, Converter.Settings);
    }
}
