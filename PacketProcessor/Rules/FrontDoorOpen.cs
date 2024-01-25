using HomNetBridge.Services;
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
        [Protocol(ProtocolType.Udp)]
        [Contains("F3 09 82 02 00 01 88 F4")]
        public static void FrontDoorOpen(IPPacket packet)
        {
            Logging.Print("Security sensor open event detected in HBM REPORT!");
            HAService.ChangeDoorBinarySensorState(true);
        }
    }
}
