using LutronCaseta.Core.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Responses
{
    public interface IResponseMapper
    {
        ICommuniqueType MapJsonResponse(string jsonString);
    }
}
