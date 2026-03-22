using System;
using System.IO;

namespace FakeAssemblyLine
{
    public class PropShaftSimulator
    {
        private readonly string _outputPath;
        private readonly SerialNumberGenerator _serialGen;
        private readonly Random _rng = new Random();

        public PropShaftSimulator(string outputPath, SerialNumberGenerator serialGen)
        {
            _outputPath = outputPath;
            _serialGen = serialGen;
        }

        public void SimulatePart()
        {
            string csySerial = _serialGen.NextCenterSlipYokeSerial();
            string afSerial = _serialGen.NextAxleFlangeSerial();
            string sySerial = _serialGen.NextSlipYokeSerial();
            string serial = csySerial; // WIP tracked by CSY serial
            string spiderCupBatch = _serialGen.NextBatchCode();
            string wipBatch = _serialGen.NextBatchCode();
            string ujointBatch = _serialGen.NextBatchCode();

            Console.WriteLine($"  [PropShaft] Starting part {serial}");

            SimulateWeldCell(serial, csySerial, afSerial, sySerial, spiderCupBatch);
            SimulateAssembly(serial, wipBatch);
            SimulateBalancer(serial);
            SimulateGP12Inspection(serial);
            SimulateUJointAssembly(serial, ujointBatch, wipBatch);

            Console.WriteLine($"  [PropShaft] Completed part {serial}");
        }

        private void SimulateWeldCell(string serial, string csySerial, string afSerial, string sySerial, string spiderCupBatch)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"PSWeld_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "PropShaft", "WeldCell", fileName);

            var headers = new[] { "SerialNumber", "CenterSlipYokeSN", "AxleFlangeSN", "SlipYokeSN", "SpiderCupBatch", "WeldCellID", "WeldDate", "WeldTime", "WeldCurrent", "WeldVoltage", "WeldResult" };
            var values = new[]
            {
                serial,
                csySerial,
                afSerial,
                sySerial,
                spiderCupBatch,
                "WC-" + _rng.Next(1, 4),
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (200 + _rng.Next(-15, 16)).ToString(),
                (26.0 + _rng.NextDouble() * 2 - 1).ToString("F1"),
                RandomResult(97)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    Weld Cell -> {fileName}");
        }

        private void SimulateAssembly(string serial, string wipBatch)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"PSAssembly_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "PropShaft", "Assembly", fileName);

            var headers = new[] { "SerialNumber", "WIPBatchNumber", "AssemblyStation", "AssemblyDate", "AssemblyTime", "TorqueValue", "Result" };
            var values = new[]
            {
                serial,
                wipBatch,
                "AS-" + _rng.Next(1, 3),
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (75.0 + _rng.NextDouble() * 10 - 5).ToString("F1"),
                RandomResult(98)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    Assembly -> {fileName}");
        }

        private void SimulateBalancer(string serial)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"PSBalance_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "PropShaft", "Balancer", fileName);

            var headers = new[] { "SerialNumber", "BalanceDate", "BalanceTime", "UnbalanceAmount", "CorrectionWeight", "CorrectionAngle", "Result" };
            var values = new[]
            {
                serial,
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (_rng.NextDouble() * 5.0).ToString("F2"),
                (_rng.NextDouble() * 3.0).ToString("F2"),
                _rng.Next(0, 360).ToString(),
                RandomResult(96)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    Balancer -> {fileName}");
        }

        private void SimulateGP12Inspection(string serial)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"PSInspection_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "PropShaft", "GP12", fileName);

            var headers = new[] { "SerialNumber", "InspectionDate", "InspectionTime", "VisionResult", "MeasurementData", "PassFail" };
            bool pass = _rng.Next(100) < 98;
            var values = new[]
            {
                serial,
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                pass ? "OK" : "DEFECT",
                (100.0 + _rng.NextDouble() * 0.5 - 0.25).ToString("F3"),
                pass ? "PASS" : "FAIL"
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    GP12 Inspection -> {fileName}");
        }

        private void SimulateUJointAssembly(string serial, string ujointBatch, string wipBatch)
        {
            var now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"UJoint_{serial}_{timestamp}.csv";
            string filePath = Path.Combine(_outputPath, "PropShaft", "UJoint", fileName);

            var headers = new[] { "SerialNumber", "UJointBatch", "WIPBatchNumber", "AssemblyDate", "AssemblyTime", "PressForce", "Result" };
            var values = new[]
            {
                serial,
                ujointBatch,
                wipBatch,
                now.ToString("yyyy-MM-dd"),
                now.ToString("HH:mm:ss"),
                (15000 + _rng.Next(-500, 501)).ToString(),
                RandomResult(98)
            };

            CsvWriter.Write(filePath, headers, values);
            Console.WriteLine($"    UJoint Assembly -> {fileName}");
        }

        private string RandomResult(int passPercent)
        {
            return _rng.Next(100) < passPercent ? "PASS" : "FAIL";
        }
    }
}
