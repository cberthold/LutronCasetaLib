using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LutronCaseta.Core.Connectors
{
    public interface IBridgeConnector
    {
        Task<bool> Connect();
    }
}
