﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LutronCaseta.Core
{
    public class LutronDeviceInfo
    {
        internal LutronDeviceInfo(IPAddress ipAddress, string codeVersion, string macAddress)
        {
            IPAddress = ipAddress;
            CodeVersion = codeVersion;
            MACAddress = macAddress; 
        }
        public IPAddress IPAddress { get; private set; }
        public string CodeVersion { get; private set; }
        public string MACAddress { get; private set; }

    }
}
