using PacketDotNet;
using SharpPcap;
using System.Reflection;
using System.Text;

namespace HomNetBridge.PacketProcessor
{
    public static class PacketCapture
    {
        private static ICaptureDevice captureDevice;
        private static bool showRaw;
        private static MethodInfo[] ruleMethods;

        public static void Init(string captureInterfaceName, string captureFilter, int readTimeout, bool showRawPacket)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string interfaces = SharpPcap.CaptureDeviceList.Instance.Count.ToString();
            var instances = SharpPcap.CaptureDeviceList.Instance;
            showRaw = showRawPacket;

            try
            {
                ruleMethods = typeof(Rules).GetMethods(BindingFlags.Public | BindingFlags.Static);
                Logging.Print($"Found {ruleMethods.Length} Rules from HomNetBridge.PacketProcessor.Rules");

                foreach (var rule in ruleMethods)
                {
                    Logging.Print($"Rule: {rule.Name}", Logging.LogType.Debug);
                }
            }
            catch (Exception e)
            {
                Logging.Print("Cannot load rule methods: " + e.ToString(), Logging.LogType.Error);
                throw new Exception();
            }

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

                //Show packet raw info
                if (showRaw)
                {
                    var protocol = packet.Protocol;
                    var src = packet.SourceAddress;
                    var dst = packet.DestinationAddress;
                    var length = packet.TotalPacketLength;
                    var content = packet.PayloadToString() ?? "(not-readable)";

                    Logging.Print($" {src} => {dst} ({protocol}, len={length}) :\n{content}", Logging.LogType.Raw);
                }

                foreach (var rule in ruleMethods)
                {
                    var attributes = rule.GetCustomAttributes();

                    bool isCheckFailed = false;
                    foreach (var attribute in attributes)
                    {
                        if (attribute is not RuleAttribute ruleAttribute)
                        {
                            isCheckFailed = true;
                            break;
                        }

                        if (!ruleAttribute.Check(packet))
                        {
                            isCheckFailed = true;
                            break;
                        }
                    }

                    if (isCheckFailed) continue;
                    rule.Invoke(null, new object[]{ packet });
                }

            }
            catch(Exception ex)
            {
                Logging.Print("Exception: " + ex.ToString(), Logging.LogType.Warn);
            }
        }
    }
}
