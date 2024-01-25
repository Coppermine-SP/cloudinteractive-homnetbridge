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
        [Contains("CallType=TYPE_LOBBY, MsgType=reqOpenDoor")]
        public static void NotifyLobbyOpen(IPPacket packet)
        {
            Logging.Print($"Lobby reqOpenDoor event detected in HBM REPORT!");
            NotifyService.Notify("공동현관문", $"공동현관문 출입 요청을 승인했습니다.");
        }
    }
}
