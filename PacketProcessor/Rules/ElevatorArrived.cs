using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {
        [Protocol(ProtocolType.Tcp)]
        [StartsWith("SLB&5&lbs&20")]
        [UrlParameter("arrival")]
        public static void ElevatorArrived(IPPacket packet)
        {
            Services.ElevatorService.ElevatorArrived();
        }
    }
}
