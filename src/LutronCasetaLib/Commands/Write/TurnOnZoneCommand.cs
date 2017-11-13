using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.Commands.Write
{
    public class TurnOnZoneCommand : SetZoneLevelCommand
    {
        public TurnOnZoneCommand(int zoneId) : base(zoneId, 100)
        {
        }
    }
}
