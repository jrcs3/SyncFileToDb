// See https://aka.ms/new-console-template for more information
using CsvCommonLib;
using System.Data;
using System.Data.SqlClient;

string directory = Tools.GetDirectoryInParent("data");

string filePath = Path.Combine(directory, "EmployeeIdAndSsn.csv");

List<EmployeeIdAndSsnForCsv> employeesCsv = Tools.GetCsv(filePath);

string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=InsecureDB;Persist Security Info=False;Integrated Security=true;";
var watch = System.Diagnostics.Stopwatch.StartNew();
using (SqlConnection con = new SqlConnection(connectionString))
{
    con.Open();

    List<EmployeeIdAndSsnForCsv> employeesDb = GetDb(con);

    var toAddQuery = employeesCsv.Where(p => employeesDb.All(p2 => p2.EmployeeId != p.EmployeeId)).ToList();

    var toSkipQuery = (from csv in employeesCsv
                       from db in employeesDb
                       where csv.EmployeeId == db.EmployeeId && csv.Ssn == db.Ssn
                       select csv
                       ).ToList();

    var toChangeSsnQuery = (from csv in employeesCsv
                            from db in employeesDb
                            where csv.EmployeeId == db.EmployeeId && csv.Ssn != db.Ssn
                            select csv
                       ).ToList();

    var toDeleteQuery = employeesDb.Where(p => employeesCsv.All(p2 => p2.EmployeeId != p.EmployeeId)).ToList();


    if (toAddQuery.Count + toSkipQuery.Count + toChangeSsnQuery.Count != employeesCsv.Count)
    {
        string errorMessage = $"Invalid counts {toAddQuery.Count} + {toSkipQuery.Count} + {toChangeSsnQuery.Count} = {toAddQuery.Count + toSkipQuery.Count + toChangeSsnQuery.Count}, should be {employeesCsv.Count}";
        Console.WriteLine(errorMessage);
        throw new Exception(errorMessage);
    }

    Console.WriteLine($"{toAddQuery.Count}, {toSkipQuery.Count}, {toChangeSsnQuery.Count}; {toDeleteQuery.Count}");

    foreach (var kvp in toAddQuery)
    {
        WriteToDb(con, kvp);
    }
    foreach (var kvp in toDeleteQuery)
    {
        DeleteFromDb(con, kvp);
    }
    foreach (var kvp in toChangeSsnQuery)
    {
        UpdateInDb(con, kvp);
    }
}
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine($"elapsedMs: {elapsedMs}");

void UpdateInDb(SqlConnection con, EmployeeIdAndSsnForCsv kvp)
{
    string insertQuery = "UPDATE dbo.EmployeeIdAndSsn  SET ssn = @ssn WHERE EmployeeId = @EmployeeId";

    SqlCommand topiccmd = new SqlCommand(insertQuery, con);

    SqlParameter empIdPar = new()
    {
        ParameterName = "@EmployeeId",
        SqlDbType = SqlDbType.NVarChar,
        Direction = ParameterDirection.Input,
        Value = kvp.EmployeeId
    };
    topiccmd.Parameters.Add(empIdPar);
    SqlParameter ssnPar = new()
    {
        ParameterName = "@ssn",
        SqlDbType = SqlDbType.NVarChar,
        Direction = ParameterDirection.Input,
        Value = kvp.Ssn
    };
    topiccmd.Parameters.Add(ssnPar);

    topiccmd.ExecuteNonQuery();
}

void DeleteFromDb(SqlConnection con, EmployeeIdAndSsnForCsv kvp)
{
    string insertQuery = "DELETE dbo.EmployeeIdAndSsn WHERE EmployeeId = @EmployeeId";

    SqlCommand topiccmd = new SqlCommand(insertQuery, con);

    SqlParameter empIdPar = new()
    {
        ParameterName = "@EmployeeId",
        SqlDbType = SqlDbType.NVarChar,
        Direction = ParameterDirection.Input,
        Value = kvp.EmployeeId
    };
    topiccmd.Parameters.Add(empIdPar);

    topiccmd.ExecuteNonQuery();
}

void WriteToDb(SqlConnection con, EmployeeIdAndSsnForCsv kvp)
{
    string insertQuery = "INSERT INTO dbo.EmployeeIdAndSsn(EmployeeId,ssn) VALUES (@EmployeeId,@ssn)";

    SqlCommand topiccmd = new SqlCommand(insertQuery, con);

    SqlParameter empIdPar = new()
    {
        ParameterName = "@EmployeeId",
        SqlDbType = SqlDbType.NVarChar,
        Direction = ParameterDirection.Input,
        Value = kvp.EmployeeId
    };
    topiccmd.Parameters.Add(empIdPar);
    SqlParameter ssnPar = new()
    {
        ParameterName = "@ssn",
        SqlDbType = SqlDbType.NVarChar,
        Direction = ParameterDirection.Input,
        Value = kvp.Ssn
    };
    topiccmd.Parameters.Add(ssnPar);

    topiccmd.ExecuteNonQuery();
}

List < EmployeeIdAndSsnForCsv > GetDb(SqlConnection con)
{
    List<EmployeeIdAndSsnForCsv> rVal = new();
    string selectTopics = "SELECT EmployeeId,ssn FROM dbo.EmployeeIdAndSsn";
    SqlCommand topiccmd = new SqlCommand(selectTopics, con);
    using (var reader = topiccmd.ExecuteReader())
    {
        int employeeIdOrdnal = reader.GetOrdinal("EmployeeId");
        int ssnOrdnal = reader.GetOrdinal("SSN");
        while (reader.Read())
        {
            string employeeId = (string)reader[employeeIdOrdnal];
            string ssn = (string)reader[ssnOrdnal];
            rVal.Add(new EmployeeIdAndSsnForCsv(employeeId, ssn));
        }
    }
    return rVal;
}
