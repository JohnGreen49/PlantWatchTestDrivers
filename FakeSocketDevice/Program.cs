using System;

namespace FakeSocketDevice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port = 2001;
            int intervalSeconds = 10;

            // Parse command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "-p" || args[i] == "--port") && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out port);
                }
                else if ((args[i] == "-i" || args[i] == "--interval") && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out intervalSeconds);
                }
                else if (args[i] == "-h" || args[i] == "-help" || args[i] == "--help" || args[i] == "/?")
                {
                    ShowHelp();
                    return;
                }
            }

            Console.WriteLine("========================================");
            Console.WriteLine("  FakeSocketDevice - TCP Test Server");
            Console.WriteLine("========================================");
            Console.WriteLine();

            var server = new FakeTcpServer(port, intervalSeconds * 1000);

            Console.WriteLine($"Starting server on port {port}...");
            Console.WriteLine($"Will broadcast a GUID every {intervalSeconds} seconds");
            Console.WriteLine();

            server.Start();

            Console.WriteLine("Server is running. Press Ctrl+C or Enter to stop.");
            Console.WriteLine();

            // Handle Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                server.Stop();
            };

            // Wait for Enter key
            Console.ReadLine();

            server.Stop();
            Console.WriteLine("Server stopped.");
        }

        private static void ShowHelp()
        {
            Console.WriteLine("FakeSocketDevice - A fake TCP device for testing socket connections");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("  This utility creates a TCP server that accepts client connections and");
            Console.WriteLine("  periodically broadcasts a GUID string to all connected clients.");
            Console.WriteLine("  It is designed for testing socket-based device connections in the");
            Console.WriteLine("  PlantWatch Configurator application.");
            Console.WriteLine();
            Console.WriteLine("USAGE:");
            Console.WriteLine("  FakeSocketDevice.exe [options]");
            Console.WriteLine();
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("  -p, --port <port>        TCP port to listen on (default: 2001)");
            Console.WriteLine("  -i, --interval <seconds> Seconds between GUID broadcasts (default: 10)");
            Console.WriteLine("  -h, -help, --help, /?    Show this help message");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("  FakeSocketDevice.exe");
            Console.WriteLine("      Start server on port 2001, broadcast every 10 seconds");
            Console.WriteLine();
            Console.WriteLine("  FakeSocketDevice.exe -p 3000");
            Console.WriteLine("      Start server on port 3000");
            Console.WriteLine();
            Console.WriteLine("  FakeSocketDevice.exe -p 8080 -i 5");
            Console.WriteLine("      Start server on port 8080, broadcast every 5 seconds");
            Console.WriteLine();
            Console.WriteLine("TESTING WITH PLANTWATCHCONFIGURATOR:");
            Console.WriteLine("  1. Run FakeSocketDevice.exe");
            Console.WriteLine("  2. In PlantWatchConfigurator, create a new Socket Device");
            Console.WriteLine("  3. Set IP Address to: 127.0.0.1 (or localhost)");
            Console.WriteLine("  4. Set Port to: 2001 (or your custom port)");
            Console.WriteLine("  5. Click 'Connect' to test the connection");
            Console.WriteLine("  6. You should see GUIDs appearing in the output window");
        }
    }
}
