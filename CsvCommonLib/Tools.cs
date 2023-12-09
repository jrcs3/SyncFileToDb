using CsvHelper;
using System.Globalization;
using System.Reflection;

namespace CsvCommonLib
{
    public class Tools
    {
        public static string GetDirectoryInParent(string directory)
        {
            string? currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (currentDir is null)
            {
                throw new Exception("Weird situation, invalid location");
            }
            do
            {
                int lastSlash = currentDir.LastIndexOf("\\");
                string proposedDir = currentDir.Substring(0, lastSlash);
                string parent = Path.Combine(proposedDir, directory);
                if (Directory.Exists(parent))
                {
                    return parent;
                }
                currentDir = proposedDir;
            } while (currentDir != null);

            return string.Empty;
        }
        public static List<EmployeeIdAndSsn> GetCsv(string filePath)
        {
            List<EmployeeIdAndSsn> rVal = new();
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var anonymousTypeDefinition = new
                {
                    EmployeeId = string.Empty,
                    SSN = string.Empty
                };
                var records = csv.GetRecords(anonymousTypeDefinition);

                foreach (var record in records)
                {
                    rVal.Add(new EmployeeIdAndSsn(record.EmployeeId, record.SSN));
                }
            }
            return rVal;
        }
    }
}