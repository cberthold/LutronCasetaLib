using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses.MultiDevice
{
    public partial class RepeaterProperties
    {
        [JsonProperty("IsRepeater")]
        public bool IsRepeater { get; set; }
    }
}
