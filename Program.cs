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
                Configuration.LoadConfig("config.json");
            }
            catch
            {
                Logging.Print("Initialization Failed!", Logging.LogType.Error);
            }
        }
    }
}