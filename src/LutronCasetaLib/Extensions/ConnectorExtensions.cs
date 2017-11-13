using LutronCaseta.Commands.Write;
using LutronCaseta.Core.Commands.Write;
using LutronCaseta.Core.Info;
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

        public static void SendGetDevices(this IWriteProcessor processor)
        {
            var command = new GetDevicesCommand();
            processor.ExecuteCommand(command);
        }

        public static void SendGetScenes(this IWriteProcessor processor)
        {
            var command = new GetScenesCommand();
            processor.ExecuteCommand(command);
        }

        public static void SendGetZoneStatus(this IWriteProcessor processor, IZoneInfo zone)
        {
            var command = new GetZoneStatusCommand(zone.Id);
            processor.ExecuteCommand(command);
        }

        public static void SetZoneLevel(this IWriteProcessor processor, IZoneInfo zone, int level)
        {
            var command = new SetZoneLevelCommand(zone.Id, level);
            processor.ExecuteCommand(command);
        }

        public static void TurnOnZone(this IWriteProcessor processor, IZoneInfo zone)
        {
            var command = new TurnOnZoneCommand(zone.Id);
            processor.ExecuteCommand(command);
        }

        public static void TurnOffZone(this IWriteProcessor processor, IZoneInfo zone)
        {
            var command = new TurnOffZoneCommand(zone.Id);
            processor.ExecuteCommand(command);
        }
        #endregion
    }
}
