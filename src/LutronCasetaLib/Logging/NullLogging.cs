using LutronCaseta.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Logging
{
    public class NullLogging : ILogging
    {
        public static readonly ILogging Instance = new NullLogging();

        private NullLogging() { }

        public void Debug(object message)
        {
        }

        public void Debug(object message, Exception exception)
        {
        }

        public void DebugFormat(string format, params object[] args)
        {
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
        }
    }
}
