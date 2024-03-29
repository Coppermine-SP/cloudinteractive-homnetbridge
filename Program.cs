﻿using HomNetBridge.PacketProcessor;

namespace HomNetBridge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CloudInteractive HomNetBridge - 0.2.0");
            Console.WriteLine("Copyright (C) 2024 CloudInteractive Inc.\n\n");

            Logging.Print("Initialization Start...");

            try
            {
                //Loading configurations and set verbose, raw mode.
                Configuration.LoadConfig("config.json");

                if (Configuration.Config.ShowVerbose)
                {
                    Logging.Print("Use verbose mode.");
                    Logging.UseVerbose = true;
                }

                if (Configuration.Config.ShowRaw)
                {
                    Logging.Print("Show all packets in raw.", Logging.LogType.Warn);
                    Logging.UseRaw = true;
                }

                //Init HAService
                Services.HAService.Init(Configuration.Config.HAUrl,
                    Configuration.Config.HAKey);

                //Init ElevatorService
                Services.ElevatorService.Init(Configuration.Config.RefFloor, Configuration.Config.NotifyUnit);

                //Init PacketCapture
                PacketCapture.Init(Configuration.Config.CaptureInterfaceName,
                    Configuration.Config.CaptureFilter, 
                    Configuration.Config.ReadTimeout, 
                    Configuration.Config.ShowRaw);
            }
            catch
            {
                Logging.Print("Initialization Failed!", Logging.LogType.Error);
                return;
            }

            Logging.Print("Type stop in console to exit.");
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "stop") return;
            }
        }
    }
}