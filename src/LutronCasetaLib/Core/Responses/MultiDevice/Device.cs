using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses.MultiDevice
{
    public partial class Device
    {
        [JsonProperty("ButtonGroups")]
        public ButtonGroups[] ButtonGroups { get; set; }

        [JsonProperty("DeviceType")]
        public string DeviceType { get; set; }

        [JsonProperty("FullyQualifiedName")]
        public string[] FullyQualifiedName { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("LocalZones")]
        public ButtonGroups[] LocalZones { get; set; }

        [JsonProperty("ModelNumber")]
        public string ModelNumber { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Parent")]
        public ButtonGroups Parent { get; set; }

        [JsonProperty("RepeaterProperties")]
        public RepeaterProperties RepeaterProperties { get; set; }

        [JsonProperty("SerialNumber")]
        public long SerialNumber { get; set; }
    }
}
