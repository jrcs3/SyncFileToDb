// See https://aka.ms/new-console-template for more information
using CsvCommonLib;
using SyncFileToDbEFSimple;
using System.Formats.Asn1;
using System.Globalization;
using System.Reflection;

string directory = Tools.GetDirectoryInParent("data");

string filePath = Path.Combine(directory, "EmployeeIdAndSsn.csv");

List<EmployeeIdAndSsn> employeesCsv = Tools.GetCsv(filePath);

string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=InsecureDB;Trusted_Connection=True;";

var watch = System.Diagnostics.Stopwatch.StartNew();
using (EmployeeAndSsnContext context = new EmployeeAndSsnContext(connectionString))
{
    var employeesDb = context.EmployeeIdAndSsn;
    if (employeesDb == null)
    {
        throw new Exception("No records found");
    }

    employeesDb.RemoveRange(employeesDb);

    employeesDb.AddRange(employeesCsv);

    //Console.WriteLine(employeesDb?.Sql);

    context.SaveChanges();

    //var theList = employeesDb?.ToList();
}
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine($"elapsedMs: {elapsedMs}");
