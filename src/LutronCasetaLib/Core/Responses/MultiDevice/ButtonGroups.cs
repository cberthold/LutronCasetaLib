using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses.MultiDevice
{
    public partial class ButtonGroups
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}
