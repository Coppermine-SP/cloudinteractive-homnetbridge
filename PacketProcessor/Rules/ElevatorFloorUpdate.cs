using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomNetBridge.Services;
using PacketDotNet;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {
        [Protocol(ProtocolType.Tcp)]
        [StartsWith("SLB&8&lbs&20&cmd=2")]
        [UrlParameter("floor")]
        public static void ElevatorFloorUpdate(IPPacket packet)
        {
            string content = packet.PayloadToString();
            string floor = Util.GetUrlParameter("floor", content);
            int direction = Convert.ToInt32(Util.GetUrlParameter("direction", content));
            ElevatorService.UpdateFloor(floor, direction);
        }
    }
}
