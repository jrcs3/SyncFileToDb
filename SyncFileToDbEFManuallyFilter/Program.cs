// See https://aka.ms/new-console-template for more information
using CsvCommonLib;
using EFCommonLib;
using System.Data;

Console.WriteLine("Get CSV Data");
string directory = Tools.GetDirectoryInParent("data");
string filePath = Path.Combine(directory, "EmployeeIdAndSsn.csv");
List<EmployeeIdAndSsnForCsv> employeesCsv = Tools.GetCsv(filePath);

string connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=InsecureDB;Trusted_Connection=True;";

var watch = System.Diagnostics.Stopwatch.StartNew();
using (EmployeeAndSsnContext context = new EmployeeAndSsnContext(connectionString))
{
    Console.WriteLine("Get DB Query");
    var employeesDb = context.EmployeeIdAndSsn;
    if (employeesDb == null)
    {
        throw new Exception("No records found");
    }
    List<EmployeeIdAndSsn> employeesDb2 = employeesDb.ToList();

    //employeesDb.RemoveRange(employeesDb);

    Console.WriteLine("Get DB Data");
    List<EmployeeIdAndSsnForCsv> employeesFromDbAsList = MoveToCsvList(employeesDb2);

    List<EmployeeIdAndSsnForCsv> toAddQuery = employeesCsv.Where(p => employeesFromDbAsList.All(p2 => p2.EmployeeId != p.EmployeeId)).ToList();

    List<EmployeeIdAndSsnForCsv> toSkipQuery = (from csv in employeesCsv
                       from db in employeesFromDbAsList
                                                where csv.EmployeeId == db.EmployeeId && csv.Ssn == db.Ssn
                       select csv
                       ).ToList();

    List<EmployeeIdAndSsnForCsv> toChangeSsnQuery = (from csv in employeesCsv
                            from db in employeesFromDbAsList
                                                     where csv.EmployeeId == db.EmployeeId && csv.Ssn != db.Ssn
                            select csv
                       ).ToList();

    var toDeleteQuery = employeesFromDbAsList.Where(p => employeesCsv.All(p2 => p2.EmployeeId != p.EmployeeId)).ToList();

    Console.WriteLine("Check Validity");
    if (toAddQuery.Count + toSkipQuery.Count + toChangeSsnQuery.Count != employeesCsv.Count)
    {
        string errorMessage = $"Invalid counts {toAddQuery.Count} + {toSkipQuery.Count} + {toChangeSsnQuery.Count} = {toAddQuery.Count + toSkipQuery.Count + toChangeSsnQuery.Count}, should be {employeesCsv.Count}";
        Console.WriteLine(errorMessage);
        throw new Exception(errorMessage);
    }

    Console.WriteLine($"{toAddQuery.Count}, {toSkipQuery.Count}, {toChangeSsnQuery.Count}; {toDeleteQuery.Count}");

    Console.WriteLine("Add loop");
    foreach (var employee in toAddQuery)
    {
        employeesDb.Add(new EmployeeIdAndSsn { EmployeeId = employee.EmployeeId, Ssn = employee.Ssn });
    }

    Console.WriteLine("Change loop");
    foreach (var employee in toChangeSsnQuery)
    {
        var result = employeesDb2.SingleOrDefault(b => b.EmployeeId == employee.EmployeeId);
        if (result != null)
        {
            result.Ssn = employee.Ssn;
        }
    }

    Console.WriteLine("Delete loop");
    List<string> toBeDelete = toDeleteQuery.Select(p => p.EmployeeId).ToList();
    employeesDb.RemoveRange(employeesDb2.Where(x => toBeDelete.Contains(x.EmployeeId)));

    context.SaveChanges();
}
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine($"elapsedMs: {elapsedMs}");

List<EmployeeIdAndSsnForCsv> MoveToCsvList(List<EmployeeIdAndSsn> employeesDb)
{
    List<EmployeeIdAndSsnForCsv> rVal = new List<EmployeeIdAndSsnForCsv>();
    foreach (var employee in employeesDb)
    {
        rVal.Add(new EmployeeIdAndSsnForCsv(employee.EmployeeId, employee.Ssn));
    }
    return rVal;
}
