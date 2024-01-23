using System;
using PacketDotNet;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {

        [Protocol(ProtocolType.Udp)]
        [StartsWith("[HBM]")]
        public static void ShellLog(IPPacket packet)
        {
            Logging.Print(packet.PayloadToString() ?? "NULL", Logging.LogType.Debug);
        }
    }
}
