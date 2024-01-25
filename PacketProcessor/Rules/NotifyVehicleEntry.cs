using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomNetBridge.Services;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {
        [Protocol(ProtocolType.Tcp)]
        [StartsWith("SLB&5&lbs&17")]
        [UrlParameter("info")]
        public static void NotifyVehicleEntry(IPPacket packet)
        {
            string content = packet.PayloadToString(true);
            string licenseNo = Util.GetUrlParameter("info", content);
            
            Logging.Print($"VehicleEntry: {licenseNo}");
            HAService.Notify("차량 입차", $"{licenseNo} 차량이 입차했습니다.");
        }
    }
}
