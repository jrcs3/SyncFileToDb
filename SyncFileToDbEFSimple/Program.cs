// See https://aka.ms/new-console-template for more information
using CsvCommonLib;
using EFCommonLib;

string directory = Tools.GetDirectoryInParent("data");

string filePath = Path.Combine(directory, "EmployeeIdAndSsn.csv");

List<EmployeeIdAndSsnForCsv> employeesCsv = Tools.GetCsv(filePath);

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

    foreach (var employee in employeesCsv)
    {
        employeesDb.Add(new EmployeeIdAndSsn { EmployeeId = employee.EmployeeId, Ssn = employee.Ssn });
    }

    context.SaveChanges();
}
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine($"elapsedMs: {elapsedMs}");
