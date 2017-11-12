using LutronCaseta.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Logging
{
    public class ConsoleLogging : ILogging
    {
        public void Debug(object message)
        {
            Console.WriteLine("DEBUG: {0}", message);
        }

        public void Debug(object message, Exception exception)
        {
            Console.WriteLine("DEBUG: {0}", message);
            Console.WriteLine("\tStack Trace: {0}", exception.StackTrace.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            Console.WriteLine("DEBUG: " + format, args);
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            DebugFormat(format, args);
            Console.WriteLine("\tStack Trace: {0}", exception.StackTrace.ToString());
        }
    }
}
