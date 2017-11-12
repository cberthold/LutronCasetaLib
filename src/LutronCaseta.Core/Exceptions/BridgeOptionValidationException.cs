using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Exceptions
{

    [Serializable]
    public class BridgeOptionValidationException : Exception
    {
        public BridgeOptionValidationException() { }
        public BridgeOptionValidationException(string message) : base(message) { }
        public BridgeOptionValidationException(string message, Exception inner) : base(message, inner) { }
        protected BridgeOptionValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
