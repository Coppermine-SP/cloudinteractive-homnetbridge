﻿using HADotNet.Core;
using HADotNet.Core.Clients;

namespace HomNetBridge.Services
{
    public static class HAService
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
            Logging.Print($"NotifyService.Notify : {title} / {message} (tag={tag ?? "null"}, level={NotifyLevelString[(int)level]})", Logging.LogType.Debug);

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

        public static void ChangeDoorBinarySensorState(bool state)
        {
            Logging.Print($"Change doorBinarySensor state = {state}.");
            serviceClient.CallService("python_script.set_state", new
            {
                entity_id = "binary_sensor.homnet_front_door",
                state = state ? "on" : "off"
            });
        }
    }
}
