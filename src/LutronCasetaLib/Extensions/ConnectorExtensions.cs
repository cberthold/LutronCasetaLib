﻿using LutronCaseta.Commands.Write;
using LutronCaseta.Core.Commands.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta
{
    public static class ConnectorExtensions
    {

        #region Write commands

        public static void Ping(this IWriteProcessor processor)
        {
            var command = new PingCommand();
            processor.ExecuteCommand(command);
        }

        public static void GetDevices(this IWriteProcessor processor)
        {
            var command = new GetDevicesCommand();
            processor.ExecuteCommand(command);
        }

        #endregion
    }
}