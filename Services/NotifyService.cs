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
        public static bool IsInited { get; private set; } = false;
        private static ServiceClient serviceClient;
        public static void Init(string endpoint, string key)
        {
            Logging.Print($"NotifyService Init... (endpoint={endpoint})");
            ClientFactory.Initialize(endpoint, key);
            serviceClient = ClientFactory.GetClient<ServiceClient>();
            IsInited = true;
        }

        public static void Notify(string title, string message)
        {
            Logging.Print($"NotifyService.Notify : {title} / {message}", Logging.LogType.Debug);
            serviceClient.CallService("notify.notify", new
            {
                title = title,
                message = message
            });
        }

        public static void PushLog(string logger, string message, bool isDebug)
        {
            if (!IsInited) return;

            serviceClient.CallService("system_log.write", new
            {
                level = isDebug ? "debug" : "info",
                logger = logger,
                message = message
            });
        }
    }
}
