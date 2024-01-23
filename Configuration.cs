using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HomNetBridge
{
    public static class Configuration
    {
        public static HomNetBridgeConfig? Config { get; private set; }
        public static void LoadConfig(string path)
        {
            Logging.Print($"Load configurations from {path}...");

            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile(path)
                    .Build();

                Config = config.GetSection("HomNetBridgeConfig").Get<HomNetBridgeConfig>();
            }
            catch(Exception e)
            {
                Logging.Print("Could not load configuration file: " + e.ToString(), Logging.LogType.Error);
                throw new Exception();
            }
        }
    }

    public class HomNetBridgeConfig
    {
        public string CaptureInterfaceName;
        public string CaptureFilter;
        public int ReadTimeout;

        public bool ShowVerbose;
        public bool ShowRaw;

        public string HAUrl;
        public string HAKey;

        public int RefFloor;
        public int NotifyUnit;

    }
}
