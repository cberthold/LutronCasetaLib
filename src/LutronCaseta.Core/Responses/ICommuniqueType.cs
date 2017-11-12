using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Responses
{
    public interface ICommuniqueType<TResponseBody> : ICommuniqueType
        where TResponseBody : IResponseBody
    {
        TResponseBody Body { get; }
    }
    public interface ICommuniqueType
    {
        Header Header { get; }
        string CommType { get; }
    }
}
