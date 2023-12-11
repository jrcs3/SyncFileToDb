// See https://aka.ms/new-console-template for more information
using CsvCommonLib;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.X86;
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
    //string insertQueryStarter = "INSERT INTO dbo.EmployeeIdAndSsn(EmployeeId,ssn) VALUES ";
    //while (WriteRangeGeneric(startNumber, recordsToWrite,SetValuessWithNumber, SetParametersWithNumberForEmployeeIdAndSsnForCsv, insertQueryStarter, con, employeesCsv))
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

#region Generic methods (just for fun)
bool WriteRangeGeneric<T>(int startNumber, int recordsToWrite, Func<int, string> setValuesWithNumber, Func<int, T, List<SqlParameter>> setParametersWithNumber, string insertQueryStarter, SqlConnection con, List<T> employeesCsv)
{
    bool isThereMore = true;
    StringBuilder sbQuery = new StringBuilder();
    sbQuery.AppendLine(insertQueryStarter);
    int endNumber = startNumber + recordsToWrite;
    // Are we comming to the end?
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
            sbQuery.Append(',');
        }
        sbQuery.AppendLine(setValuesWithNumber(i));
        T record = employeesCsv[i];
        parms.AddRange(setParametersWithNumber(i, record));
    }

    SqlCommand insertCmd = new SqlCommand(sbQuery.ToString(), con);
    insertCmd.Parameters.AddRange(parms.ToArray());
    insertCmd.ExecuteNonQuery();

    return isThereMore;
}

List<SqlParameter> SetParametersWithNumberForEmployeeIdAndSsnForCsv(int number, EmployeeIdAndSsnForCsv employee)
{
    return new List<SqlParameter> {
        new SqlParameter
        {
            ParameterName = $"@EmployeeId{number}",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.Input,
            Value = employee.EmployeeId
        },
        new SqlParameter
        {
            ParameterName = $"@Ssn{number}",
            SqlDbType = SqlDbType.NVarChar,
            Direction = ParameterDirection.Input,
            Value = employee.Ssn
        }
    };
}
#endregion Generic methods (just for fun)