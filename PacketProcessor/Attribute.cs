using System.Collections.Specialized;
using System.Web;
using PacketDotNet;

namespace HomNetBridge.PacketProcessor
{
    public abstract class RuleAttribute : Attribute
    {
        public abstract bool Check(IPPacket packet);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolAttribute : RuleAttribute
    {
        public ProtocolType Value { get; }

        public ProtocolAttribute(ProtocolType type)
        {
            Value = type;
        }

        public override bool Check(IPPacket packet)
        {
            return packet.Protocol == Value;
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class UrlParameterAttribute : RuleAttribute
    {
        public string Value { get; }

        public UrlParameterAttribute(string value)
        {
            Value = value;
        }

        public override bool Check(IPPacket packet)
        {
            if (packet.Protocol is ProtocolType.Udp or ProtocolType.Tcp)
            {
                NameValueCollection parameters = HttpUtility.ParseQueryString(packet.PayloadToString());
                return !String.IsNullOrWhiteSpace(parameters[Value]);
            }
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPacketAttribute : RuleAttribute
    {
        public override bool Check(IPPacket packet)
        {
            return true;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class StartsWithAttribute : RuleAttribute
    {
        public string Value { get; }

        public StartsWithAttribute(string value)
        {
            Value = value;
        }

        public override bool Check(IPPacket packet)
        {
            return packet.PayloadToString().StartsWith(Value);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ContainsAttribute : RuleAttribute
    {
        public string Value { get; }

        public ContainsAttribute(string value)
        {
            Value = value;
        }

        public override bool Check(IPPacket packet)
        {
            if (packet.Protocol is ProtocolType.Udp or ProtocolType.Tcp)
            {
                return packet.PayloadToString().Contains(Value);
            }
            return false;
        }
    }


}
