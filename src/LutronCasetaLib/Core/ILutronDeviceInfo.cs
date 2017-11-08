using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LutronCaseta.Core
{
    public interface ILutronDeviceInfo
    {
        IPAddress IPAddress { get; }
        string CodeVersion { get; }
        string MACAddress { get; }
    }
}
