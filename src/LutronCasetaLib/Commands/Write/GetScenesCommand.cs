using LutronCaseta.Core.Commands.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands.Write
{
    public class GetScenesCommand : AbstractWriteCommand
    {
        public override void SendCommand(IWriteProcessor processor)
        {
            string deviceCommand = "{\"CommuniqueType\":\"ReadRequest\",\"Header\":{\"Url\":\"/virtualbutton\"}}\n";
            processor.WriteString(deviceCommand);
        }
    }
}
