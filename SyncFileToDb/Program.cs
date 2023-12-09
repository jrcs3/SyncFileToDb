// See https://aka.ms/new-console-template for more information
using CsvHelper;
using SyncFileToDb;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Formats.Asn1;
using System.Globalization;

string filePath = "data\\EmployeeIdAndSsn.csv";

string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=InsecureDB;Persist Security Info=False;Integrated Security=true;";

using (SqlConnection con = new SqlConnection(connectionString))
{
    con.Open();

    List<EmployeeIdAndSsn> employeesDb = GetDb(con);

    List<EmployeeIdAndSsn> employeesCsv = GetCsv(filePath);

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

void UpdateInDb(SqlConnection con, EmployeeIdAndSsn kvp)
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

void DeleteFromDb(SqlConnection con, EmployeeIdAndSsn kvp)
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

void WriteToDb(SqlConnection con, EmployeeIdAndSsn kvp)
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

List < EmployeeIdAndSsn > GetDb(SqlConnection con)
{
    List<EmployeeIdAndSsn> rVal = new();
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
            rVal.Add(new EmployeeIdAndSsn(employeeId, ssn));
        }
    }
    return rVal;
}
List<EmployeeIdAndSsn> GetCsv(string filePath)
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