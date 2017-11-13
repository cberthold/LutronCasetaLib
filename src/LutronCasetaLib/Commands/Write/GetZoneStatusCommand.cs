using LutronCaseta.Core.Commands.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands.Write
{
    public class GetZoneStatusCommand : AbstractWriteCommand
    {
        readonly int zoneId;
        public GetZoneStatusCommand(int zoneId)
        {
            this.zoneId = zoneId;
        }
        public override void SendCommand(IWriteProcessor processor)
        {
            string deviceCommand = $"{{\"CommuniqueType\":\"ReadRequest\",\"Header\":{{\"Url\":\"/zone/{zoneId}/status\"}}}}\n";
            processor.WriteString(deviceCommand);
        }
    }
}
