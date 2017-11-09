using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Commands.Write
{
    public abstract class AbstractWriteCommand : IWriteCommand
    {
        public abstract void SendCommand(IWriteProcessor processor);
    }
}
