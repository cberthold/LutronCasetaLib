using System;
using System.Collections.Generic;
using System.Text;
using LutronCaseta.Core.Responses;
using LutronCaseta.Core.Responses.Ping;
using LutronCaseta.Core.Responses.MultiDevice;

namespace LutronCaseta.Responses
{
    public class ResponseMapper : IResponseMapper
    {
        public const string RESPONSE_PING = "OnePingResponse";
        public const string RESPONSE_GET_DEVICES = "MultipleDeviceDefinition";


        static readonly Dictionary<string, Func<string, ICommuniqueType>> responseMapper 
            = new Dictionary<string, Func<string, ICommuniqueType>>();

        static ResponseMapper()
        {
            responseMapper.Add(RESPONSE_PING, (json) => MapCommunique<PingResponseBody>(json));
            responseMapper.Add(RESPONSE_GET_DEVICES, (json) => MapCommunique<MultipleDeviceBody>(json));
        }

        private static CommuniqueType<TResponseBody> MapCommunique<TResponseBody>(string jsonString)
            where TResponseBody : IResponseBody
        {
            var output = CommuniqueType<TResponseBody>.FromJson(jsonString);
            return output;
        }

        public ICommuniqueType MapJsonResponse(string jsonString)
        {
            var mapWithoutBody = CommuniqueTypeWithoutBody.FromJsonWithoutBody(jsonString);

            if(!responseMapper.ContainsKey(mapWithoutBody?.Header.MessageBodyType))
            {
                throw new KeyNotFoundException($"{nameof(mapWithoutBody.Header.MessageBodyType)} with value '{mapWithoutBody.Header.MessageBodyType}' cannot be found");
            }

            var mapFunction = responseMapper[mapWithoutBody.Header.MessageBodyType];

            var communique = mapFunction(jsonString);

            return communique;
        }
    }
}
