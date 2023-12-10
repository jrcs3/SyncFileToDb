// See https://aka.ms/new-console-template for more information
using CsvCommonLib;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

string directory = Tools.GetDirectoryInParent("data");

string filePath = Path.Combine(directory, "EmployeeIdAndSsn.csv");

List<EmployeeIdAndSsnForCsv> employeesCsv = Tools.GetCsv(filePath);

string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=InsecureDB;Persist Security Info=False;Integrated Security=true;";
var watch = System.Diagnostics.Stopwatch.StartNew();
using (SqlConnection con = new SqlConnection(connectionString))
{
    con.Open();
    TruncateTable(con);

    int startNumber = 0;
    int recordsToWrite = 1;
    while (WriteRange(con, startNumber, recordsToWrite, employeesCsv))
    {
        startNumber += recordsToWrite;
    }
}
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine($"elapsedMs: {elapsedMs}");

void TruncateTable(SqlConnection con)
{
    SqlCommand deleteCmd = new SqlCommand("DELETE FROM dbo.EmployeeIdAndSsn", con);
    deleteCmd.ExecuteNonQuery();
}

bool WriteRange(SqlConnection con, int startNumber, int recordsToWrite, List<EmployeeIdAndSsnForCsv> employeesCsv)
{
    bool isThereMore = true;
    string insertQueryStarter = "INSERT INTO dbo.EmployeeIdAndSsn(EmployeeId,ssn) VALUES ";
    StringBuilder sb = new StringBuilder();
    sb.AppendLine(insertQueryStarter);
    int endNumber = startNumber + recordsToWrite;
    if (endNumber >= employeesCsv.Count)
    {
        endNumber = employeesCsv.Count;
        isThereMore = false;
    }
    List<SqlParameter> parms = new List<SqlParameter>();
    for (int i = startNumber; i < endNumber; ++i)
    {
        if (i > startNumber)
        {
            sb.Append(',');
        }
        sb.AppendLine(SetValuessWithNumber(i));
        EmployeeIdAndSsnForCsv record = employeesCsv[i];
        parms.AddRange(SetParametersWithNumber(i, record.EmployeeId, record.Ssn));
    }

    SqlCommand insertCmd = new SqlCommand(sb.ToString(), con);
    insertCmd.Parameters.AddRange(parms.ToArray());
    insertCmd.ExecuteNonQuery();

    return isThereMore;
}

string SetValuessWithNumber(int number)
{
    return $"(@EmployeeId{number},@Ssn{number})";
}

List<SqlParameter> SetParametersWithNumber(int number, string employeeId, string ssn)
{
    return new List<SqlParameter> {
        new SqlParameter
        {
            ParameterName = $"@EmployeeId{number}",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.Input,
            Value = employeeId
        },
        new SqlParameter
        {
            ParameterName = $"@Ssn{number}",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.Input,
            Value = ssn
        }
    };
}