using Newtonsoft.Json.Linq;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HomNetBridge.PacketProcessor
{
    public static partial class Util
    {
        public static string GetUrlParameter(string key, string value)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(value);
            return parameters[key];
        }
    }
}
