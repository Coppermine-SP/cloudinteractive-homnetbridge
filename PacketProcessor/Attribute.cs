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
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPacketAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class StartsWithAttribute : Attribute
    {
        public string Value { get; }

        public StartsWithAttribute(string value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ContainsAttribute : Attribute
    {
        public string Value { get; }

        public ContainsAttribute(string value)
        {
            Value = value;
        }
    }


}
