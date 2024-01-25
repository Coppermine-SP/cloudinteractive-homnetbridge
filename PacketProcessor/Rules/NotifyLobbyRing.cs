using HomNetBridge.Services;
using PacketDotNet;


namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {
        [Protocol(ProtocolType.Udp)]
        [Contains("CallType=TYPE_LOBBY, MsgType=evtRing")]
        public static void NotifyLobbyRing(IPPacket packet)
        {
            Logging.Print($"Lobby evtRing event detected in HBM REPORT!");
            NotifyService.Notify("공동현관문", $"공동현관문 출입 요청이 있습니다.");
        }
    }
}
