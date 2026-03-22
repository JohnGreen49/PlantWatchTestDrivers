using System.IO;

namespace FakeAssemblyLine
{
    public static class CsvWriter
    {
        public static void Write(string filePath, string[] headers, string[] values)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(string.Join(",", headers));
                writer.WriteLine(string.Join(",", values));
            }
        }
    }
}
