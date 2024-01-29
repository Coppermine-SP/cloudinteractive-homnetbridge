using PacketDotNet;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {
        [Protocol(ProtocolType.Tcp)]
        [StartsWith("SLB&5&lbs&120")]
        [UrlParameter("res")]
        public static void ElevatorCalled(IPPacket packet)
        {
            Services.ElevatorService.ElevatorCalled();
        }
    }
}
