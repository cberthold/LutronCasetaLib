using LutronCaseta.Commands.Write;
using LutronCaseta.Core.Commands.Write;
using LutronCaseta.Core.Info;
using LutronCaseta.Core.Responses;
using LutronCaseta.Responses;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LutronCaseta
{
    public static class ConnectorExtensions
    {

        #region Write commands

        public static void SendPing(this IWriteProcessor processor)
        {
            var command = new PingCommand();
            processor.ExecuteCommand(command);
        }

        public static Task<ICommuniqueType> Ping(this IWriteProcessor processor, CancellationToken token = default(CancellationToken))
        {
            var task = processor.Responses
                .WaitFor((c) => c.Header?.MessageBodyType == ResponseMapper.RESPONSE_PING, token: token);
            
            processor.SendPing();

            return task;
        }

        public static void SendGetDevices(this IWriteProcessor processor)
        {
            var command = new GetDevicesCommand();
            processor.ExecuteCommand(command);
        }

        public static Task<ICommuniqueType> GetDevices(this IWriteProcessor processor, CancellationToken token = default(CancellationToken))
        {
            var task = processor.Responses
                .WaitFor((c) => c.Header?.MessageBodyType == ResponseMapper.RESPONSE_GET_DEVICES, token: token);

            processor.SendGetDevices();

            return task;
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
