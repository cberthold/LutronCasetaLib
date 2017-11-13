using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands.Write
{
    public class TurnOffZoneCommand : SetZoneLevelCommand
    {
        public TurnOffZoneCommand(int zoneId) : base(zoneId, 0)
        {
        }
    }
}
