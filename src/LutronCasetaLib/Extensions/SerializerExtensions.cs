using LutronCaseta.Core.Responses;
using LutronCaseta.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Extensions
{
    public static class SerializerExtensions
    {
        public static string ToJson(this ICommuniqueType self) => JsonConvert.SerializeObject(self, Converter.Settings);

    }
}
