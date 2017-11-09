using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Commands
{
    /// <summary>
    /// Represents an individual command that can be sent to the bridge
    /// </summary>
    public interface IWriteCommand
    {
        void SendCommand(IWriteProcessor processor);
    }
}
