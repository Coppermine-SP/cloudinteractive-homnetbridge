using System;
using System.Text;
using PacketDotNet;
using SharpPcap;

namespace HomNetBridge.PacketProcessor
{
    public static class IPPacketExtension
    {
        public static string? PayloadToString(this IPPacket packet)
        {
            Packet extractedPacket;
            if (packet.Protocol == ProtocolType.Tcp) 
                extractedPacket = packet.Extract<TcpPacket>();
            else if (packet.Protocol == ProtocolType.Udp)
                extractedPacket = packet.Extract<UdpPacket>();
            else return null;

            var payload = extractedPacket.PayloadData;
            if (payload is not null)
            {
                return Encoding.UTF8.GetString(payload).Trim();
            }
            return null;
        }
    }
}
