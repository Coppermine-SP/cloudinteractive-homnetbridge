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

        public enum NotifyLevel {Active, TimeSensitive, Critical}
        private static readonly string[] NotifyLevelString =
        {
            "active",
            "time-sensitive",
            "critical"
        };

        public static void Notify(string title, string message, NotifyLevel level = NotifyLevel.Active, string? tag = null)
        {
            Logging.Print($"NotifyService.Notify : {title} / {message}", Logging.LogType.Debug);

            Dictionary<string, object> dataFields = new Dictionary<string, object>();
            Dictionary<string, string> pushFields = new Dictionary<string, string>();

            pushFields["interruption-level"] = NotifyLevelString[(int)level];
            dataFields["push"] = pushFields;
            if (tag is not null) dataFields["tag"] = tag;

            serviceClient.CallService("notify.notify", new
            {
                title = title,
                message = message,
                data = dataFields
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
