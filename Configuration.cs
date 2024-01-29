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

                Config = config.GetSection("HomeNetBridgeConfig").Get<HomNetBridgeConfig>();
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
        public string? CaptureInterfaceName { get; set; }
        public string? CaptureFilter { get; set; }
        public int ReadTimeout { get; set; }

        public bool ShowVerbose { get; set; }
        public bool ShowRaw { get; set; }

        public string? HAUrl { get; set; }
        public string? HAKey { get; set; }

        public int RefFloor { get; set; }
        public int NotifyUnit { get; set; }

    }
}
