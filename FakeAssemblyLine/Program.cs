using System;
using System.Threading;

namespace FakeAssemblyLine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string outputPath = @"C:\PlantWatchData\";
            int count = -1; // -1 means continuous
            int intervalSeconds = 10;
            string product = "both";

            for (int i = 0; i < args.Length; i++)
            {
                if ((args[i] == "-o" || args[i] == "--output") && i + 1 < args.Length)
                {
                    outputPath = args[i + 1];
                    i++;
                }
                else if ((args[i] == "-n" || args[i] == "--count") && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out count);
                    i++;
                }
                else if (args[i] == "-c" || args[i] == "--continuous")
                {
                    count = -1;
                }
                else if ((args[i] == "-i" || args[i] == "--interval") && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out intervalSeconds);
                    i++;
                }
                else if ((args[i] == "-p" || args[i] == "--product") && i + 1 < args.Length)
                {
                    product = args[i + 1].ToLower();
                    i++;
                }
                else if (args[i] == "-h" || args[i] == "-help" || args[i] == "--help" || args[i] == "/?")
                {
                    ShowHelp();
                    return;
                }
            }

            Console.WriteLine("========================================");
            Console.WriteLine("  FakeAssemblyLine - CSV Simulator");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine($"Output path:  {outputPath}");
            Console.WriteLine($"Product:      {product}");
            Console.WriteLine($"Mode:         {(count > 0 ? $"Batch ({count} parts)" : "Continuous")}");
            Console.WriteLine($"Interval:     {intervalSeconds} seconds");
            Console.WriteLine();

            var serialGen = new SerialNumberGenerator();
            var axleSim = new AxleTubeSimulator(outputPath, serialGen);
            var propSim = new PropShaftSimulator(outputPath, serialGen);

            bool runAxle = product == "both" || product == "axle";
            bool runProp = product == "both" || product == "propshaft";

            bool running = true;
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                running = false;
                Console.WriteLine("\nStopping...");
            };

            if (count > 0)
            {
                // Single batch mode
                for (int i = 0; i < count && running; i++)
                {
                    Console.WriteLine($"--- Part {i + 1} of {count} ---");
                    if (runAxle) axleSim.SimulatePart();
                    if (runProp) propSim.SimulatePart();
                    Console.WriteLine();
                }
            }
            else
            {
                // Continuous mode
                Console.WriteLine("Running continuously. Press Ctrl+C to stop.");
                Console.WriteLine();
                int partNumber = 0;
                while (running)
                {
                    partNumber++;
                    Console.WriteLine($"--- Part {partNumber} ---");
                    if (runAxle) axleSim.SimulatePart();
                    if (runProp) propSim.SimulatePart();
                    Console.WriteLine();

                    for (int s = 0; s < intervalSeconds && running; s++)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            Console.WriteLine("Done.");
        }

        private static void ShowHelp()
        {
            Console.WriteLine("FakeAssemblyLine - Simulates assembly line stations by writing CSV files");
            Console.WriteLine();
            Console.WriteLine("USAGE:");
            Console.WriteLine("  FakeAssemblyLine.exe [options]");
            Console.WriteLine();
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("  -o, --output <path>      Output folder (default: C:\\PlantWatchData\\)");
            Console.WriteLine("  -n, --count <N>          Generate N parts then exit (single batch mode)");
            Console.WriteLine("  -c, --continuous         Run continuously until stopped (default)");
            Console.WriteLine("  -i, --interval <seconds> Seconds between parts in continuous mode (default: 10)");
            Console.WriteLine("  -p, --product <name>     Product to simulate: axle, propshaft, both (default: both)");
            Console.WriteLine("  -h, --help               Show this help message");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("  FakeAssemblyLine.exe -n 5 -o C:\\temp\\PWTest");
            Console.WriteLine("      Generate 5 parts of each product line to C:\\temp\\PWTest");
            Console.WriteLine();
            Console.WriteLine("  FakeAssemblyLine.exe -p axle -i 5");
            Console.WriteLine("      Continuously generate axle tube parts every 5 seconds");
            Console.WriteLine();
            Console.WriteLine("  FakeAssemblyLine.exe -p propshaft -n 10");
            Console.WriteLine("      Generate 10 prop shaft parts and exit");
            Console.WriteLine();
            Console.WriteLine("OUTPUT STRUCTURE:");
            Console.WriteLine("  <output>/AxleTube/WeldStation/     Weld station CSVs");
            Console.WriteLine("  <output>/AxleTube/Machining/       Machining CSVs");
            Console.WriteLine("  <output>/AxleTube/InlineGauge/     Inline gauge CSVs");
            Console.WriteLine("  <output>/PropShaft/WeldCell/       Weld cell CSVs");
            Console.WriteLine("  <output>/PropShaft/Assembly/       Assembly CSVs");
            Console.WriteLine("  <output>/PropShaft/Balancer/       Balancer CSVs");
            Console.WriteLine("  <output>/PropShaft/GP12/           GP12 inspection CSVs");
            Console.WriteLine("  <output>/PropShaft/UJoint/         UJoint assembly CSVs");
        }
    }
}
