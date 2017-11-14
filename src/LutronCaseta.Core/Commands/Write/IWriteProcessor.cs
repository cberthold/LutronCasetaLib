using LutronCaseta.Core.Responses;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace LutronCaseta.Core.Commands.Write
{
    public interface IWriteProcessor
    {
        Subject<ICommuniqueType> Responses { get; }
        void WriteString(string writeString);
        void ExecuteCommand(IWriteCommand command);
    }
}
