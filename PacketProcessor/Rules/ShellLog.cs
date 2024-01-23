using System;
using System.Text.RegularExpressions;
using PacketDotNet;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Rules
    {
        [Protocol(ProtocolType.Udp)]
        [StartsWith("[HBM]")]
        public static void ShellLog(IPPacket packet)
        {
            const string pattern = @"[\p{C}]|\t|[ ]{5,}";

            Logging.Print(Regex.Replace(packet.PayloadToString(), pattern, " ").Trim(), Logging.LogType.Debug);
        }
    }
}
