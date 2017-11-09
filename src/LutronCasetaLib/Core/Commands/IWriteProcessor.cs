using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Commands
{
    public interface IWriteProcessor
    {
        void WriteString(string writeString);
        void ExecuteCommand(IWriteCommand command);
    }
}
