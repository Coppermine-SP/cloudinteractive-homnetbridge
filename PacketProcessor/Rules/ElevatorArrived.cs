using PacketDotNet;

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
