using HADotNet.Core;
using HADotNet.Core.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomNetBridge.Services
{
    public static class NotifyService
    {
        public static void Init(string endpoint, string key)
        {
            Logging.Print($"NotifyService Init... (endpoint={endpoint})");
            ClientFactory.Initialize(endpoint, key);
        }

        public static void Notify(string title, string message)
        {
            var serviceClient = ClientFactory.GetClient<ServiceClient>();

            Logging.Print($"NotifyService.Notify : {title} / {message}", Logging.LogType.Debug);
            serviceClient.CallService("notify.notify", new
            {
                title = title,
                message = message
            });
        }
    }
}
