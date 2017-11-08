﻿using LutronCaseta.Core.Responses;
using LutronCaseta.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Extensions
{
    public static class SerializerExtensions
    {
        public static string ToJson(this CommuniqueType self) => JsonConvert.SerializeObject(self, Converter.Settings);

    }
}