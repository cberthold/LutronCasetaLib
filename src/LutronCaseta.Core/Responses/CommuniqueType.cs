using LutronCaseta.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses
{
    public partial class CommuniqueType<TResponseBody> : CommuniqueTypeWithoutBody, ICommuniqueType
        where TResponseBody : IResponseBody
    {
        [JsonProperty("Body")]
        public TResponseBody Body { get; set; }
    }

    public partial class CommuniqueType<TResponseBody>
    {
        public static CommuniqueType<TResponseBody> FromJson(string json) => JsonConvert.DeserializeObject<CommuniqueType<TResponseBody>>(json, Converter.Settings);
    }

    public partial class CommuniqueTypeWithoutBody
    {   
        [JsonProperty("Header")]
        public Header Header { get; set; }

        [JsonProperty("CommuniqueType")]
        public string CommType { get; set; }
    }

    public partial class CommuniqueTypeWithoutBody
    {
        public static CommuniqueTypeWithoutBody FromJsonWithoutBody(string json) => JsonConvert.DeserializeObject<CommuniqueTypeWithoutBody>(json, Converter.Settings);
    }
}
