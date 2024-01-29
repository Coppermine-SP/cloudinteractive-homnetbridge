using HomNetBridge.Services;
using PacketDotNet;

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
