using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomNetBridge.PacketProcessor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketProtocolAttribute : Attribute
    {
        public string Value { get; }

        public PacketProtocolAttribute(string value)
        {
            Value = value;
        }
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
