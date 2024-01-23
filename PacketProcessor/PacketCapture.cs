﻿using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using PacketDotNet;
using SharpPcap;

namespace HomNetBridge.PacketProcessor
{
    public static class PacketCapture
    {
        private static ICaptureDevice captureDevice;
        private static bool showRaw;
        public static void Init(string captureInterfaceName, string captureFilter, int readTimeout, bool showRawPacket)
        {
            string interfaces = SharpPcap.CaptureDeviceList.Instance.Count.ToString();
            var instances = SharpPcap.CaptureDeviceList.Instance;
            showRaw = showRawPacket;

            try
            {
                Logging.Print("LibPcap : " + SharpPcap.Pcap.LibpcapVersion + " / Pcap: " + SharpPcap.Pcap.Version);
                Logging.Print($"Found {interfaces} interfaces.");
                foreach (var dev in instances)
                {
                    Logging.Print($"interface: {dev.Name} ({dev.MacAddress})", Logging.LogType.Debug);
                    if (dev.Name == captureInterfaceName)
                    {
                        Logging.Print($"Open interface {dev.Name} to capture...");
                        captureDevice = dev;
                        captureDevice.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrival);
                        captureDevice.Open(DeviceModes.Promiscuous, readTimeout);
                        captureDevice.Filter = captureFilter;
                        captureDevice.StartCapture();
                        Logging.Print($"interface {dev.Name} capture start.");
                        return;
                    }
                }
                Logging.Print($"Can't find interface {captureInterfaceName}!", Logging.LogType.Warn);
                throw new ArgumentException($"interface {captureInterfaceName} not found.");
            }
            catch (Exception e)
            {
                Logging.Print("Cannot open capture device: " + e.ToString(), Logging.LogType.Error);
                throw new Exception();
            }
        }

        private static void OnPacketArrival(object sender, SharpPcap.PacketCapture e)
        {
            try
            {
                if (e.GetPacket().LinkLayerType != LinkLayers.Ethernet) return;
                var packet = Packet.ParsePacket(LinkLayers.Ethernet, e.GetPacket().Data).Extract<IPPacket>();

                if (packet is null)
                {
                    Logging.Print("invalid ip packet. (packet was null) - ignored.", Logging.LogType.Debug);
                    return;
                }

                if (showRaw)
                {
                    const string pattern = @"[\p{C}]|\t|[ ]{5,}";

                    var protocol = packet.Protocol;
                    var src = packet.SourceAddress;
                    var dst = packet.DestinationAddress;
                    var length = packet.TotalPacketLength;
                    var content = Regex.Replace(packet.PayloadToString() ?? "(not-readable)", pattern, " ").Trim();

                    Logging.Print($"{src} => {dst} ({protocol}, len={length}) :\n{content}", Logging.LogType.Raw);
                }

            }
            catch(Exception ex)
            {
                Logging.Print("Exception: " + ex.ToString(), Logging.LogType.Warn);
            }
        }
    }
}
