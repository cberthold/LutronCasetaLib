using LutronCaseta.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands
{
    public class GetDevicesCommand : AbstractWriteCommand
    {
        public override void SendCommand(IWriteProcessor processor)
        {
            string deviceCommand = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/device\"}}\n";
            processor.WriteString(deviceCommand);
        }
    }
}
