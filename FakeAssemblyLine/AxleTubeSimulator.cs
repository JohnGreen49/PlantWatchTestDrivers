using System;
using System.IO;

namespace FakeAssemblyLine
{
    public class AxleTubeSimulator
    {
        private readonly string _outputPath;
        private readonly SerialNumberGenerator _serialGen;
        private readonly Random _rng = new Random();

        public AxleTubeSimulator(string outputPath, SerialNumberGenerator serialGen)
        {
            _outputPath = outputPath;
            _serialGen = serialGen;
        }

        public void SimulatePart()
        {
            string serial = _serialGen.NextAxleTubeSerial();
            Console.WriteLine($"  [AxleTube] Starting part {serial}");

            SimulateWeldStation(serial);
            SimulateMachining(serial);
            SimulateInlineGauge(serial);

            Console.WriteLine($"  [AxleTube] Completed part {serial}");
        }

        private void SimulateWeldStation(string serial)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Weld_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "AxleTube", "WeldStation", fileName);

            var headers = new[] { "SerialNumber", "WeldCellID", "WeldDate", "WeldTime", "WeldCurrent", "WeldVoltage", "WeldSpeed", "WeldResult" };
            var values = new[]
            {
                serial,
                "WC-" + _rng.Next(1, 5),
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (180 + _rng.Next(-10, 11)).ToString(),
                (24.5 + _rng.NextDouble() * 2 - 1).ToString("F1"),
                (12.0 + _rng.NextDouble() * 2 - 1).ToString("F1"),
                RandomResult(98)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    Weld Station -> {fileName}");
        }

        private void SimulateMachining(string serial)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Machine_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "AxleTube", "Machining", fileName);

            var headers = new[] { "SerialNumber", "MachineNumber", "MachineDate", "MachineTime", "SpindleSpeed", "FeedRate", "CycleTime", "Result" };
            var values = new[]
            {
                serial,
                "M-" + _rng.Next(1, 4),
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (2500 + _rng.Next(-100, 101)).ToString(),
                (0.005 + _rng.NextDouble() * 0.002).ToString("F4"),
                (45.0 + _rng.NextDouble() * 10).ToString("F1"),
                RandomResult(97)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    Machining -> {fileName}");
        }

        private void SimulateInlineGauge(string serial)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Gauge_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "AxleTube", "InlineGauge", fileName);

            var headers = new[] { "SerialNumber", "GaugeDate", "GaugeTime", "BoreDiameter", "GoNoGo", "Result" };
            var values = new[]
            {
                serial,
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (50.000 + _rng.NextDouble() * 0.050 - 0.025).ToString("F3"),
                _rng.Next(100) < 98 ? "GO" : "NOGO",
                RandomResult(98)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    Inline Gauge -> {fileName}");
        }

        private string RandomResult(int passPercent)
        {
            return _rng.Next(100) < passPercent ? "PASS" : "FAIL";
        }
    }
}
