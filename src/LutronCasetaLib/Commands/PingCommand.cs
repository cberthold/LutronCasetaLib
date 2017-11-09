using LutronCaseta.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands
{
    public class PingCommand : AbstractWriteCommand
    {
        public override void SendCommand(IWriteProcessor processor)
        {
            string ping = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/server/1/status/ping\"}}\n";
            processor.WriteString(ping);
        }
    }
}
