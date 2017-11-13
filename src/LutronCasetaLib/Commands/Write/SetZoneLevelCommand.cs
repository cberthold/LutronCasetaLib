using LutronCaseta.Core.Commands.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands.Write
{
    public class SetZoneLevelCommand : AbstractWriteCommand
    {
        readonly int zoneId;
        readonly int level;

        public SetZoneLevelCommand(int zoneId, int level)
        {
            this.zoneId = zoneId;
            this.level = level;
        }

        public override void SendCommand(IWriteProcessor processor)
        {
            string deviceCommand = $@"{{
                ""CommuniqueType"": ""CreateRequest"",
                ""Header"": {{ ""Url"": ""/zone/{zoneId}/commandprocessor""}},
                ""Body"": {{
                ""Command"": {{
                    ""CommandType"": ""GoToLevel"",
                        ""Parameter"": [{{""Type"": ""Level"", ""Value"": {level}}}]}}}}}}" + "\n";
            deviceCommand = deviceCommand.Replace("\r\n", string.Empty);
            processor.WriteString(deviceCommand);
        }
    }
}
