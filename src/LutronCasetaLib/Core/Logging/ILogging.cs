using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Core.Logging
{
    public interface ILogging
    {
        /// <summary>
		/// Log a message object with the Debug level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Debug(object message);

        /// <summary>
        /// Log a message object with the Debug level including
        /// the stack trace of the <see cref="Exception"/> passed
        /// as a parameter.
        /// </summary>
        /// <param name="message">The message object to log.</param>
        /// <param name="exception">The exception to log, including its stack trace.</param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// Log a message with the Debug level.
        /// </summary>
        /// <param name="format">The format of the message object to log.<see cref="string.Format(string,object[])"/> </param>
        /// <param name="args">the list of format arguments</param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Log a message with the Debug level.
        /// </summary>
        /// <param name="format">The format of the message object to log.<see cref="string.Format(string,object[])"/> </param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="args">the list of format arguments</param>
        void DebugFormat(string format, Exception exception, params object[] args);
    }
}
