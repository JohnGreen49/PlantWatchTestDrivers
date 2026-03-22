using System;

namespace FakeAssemblyLine
{
    public class SerialNumberGenerator
    {
        private int _axleTubeCounter;
        private int _csyCounter;
        private int _afCounter;
        private int _syCounter;
        private int _batchCounter;

        public SerialNumberGenerator()
        {
            var rng = new Random();
            _axleTubeCounter = rng.Next(1, 500);
            _csyCounter = rng.Next(1, 500);
            _afCounter = rng.Next(1, 500);
            _syCounter = rng.Next(1, 500);
            _batchCounter = rng.Next(1, 20);
        }

        public string NextAxleTubeSerial()
        {
            _axleTubeCounter++;
            return string.Format("AT-{0:yyyyMMdd}-{1:D5}", DateTime.Now, _axleTubeCounter);
        }

        public string NextCenterSlipYokeSerial()
        {
            _csyCounter++;
            return string.Format("CSY-{0:D5}", _csyCounter);
        }

        public string NextAxleFlangeSerial()
        {
            _afCounter++;
            return string.Format("AF-{0:D5}", _afCounter);
        }

        public string NextSlipYokeSerial()
        {
            _syCounter++;
            return string.Format("SY-{0:D5}", _syCounter);
        }

        public string NextBatchCode()
        {
            _batchCounter++;
            return string.Format("LOT-{0:yyyyMMdd}-{1:D2}", DateTime.Now, _batchCounter % 100);
        }
    }
}
