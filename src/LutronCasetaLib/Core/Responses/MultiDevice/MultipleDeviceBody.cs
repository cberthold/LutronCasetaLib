using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses.MultiDevice
{
    public partial class MultipleDeviceBody : IResponseBody
    {
        [JsonProperty("Devices")]
        public Device[] Devices { get; set; }
    }
}
