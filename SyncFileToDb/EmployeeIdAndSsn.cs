namespace SyncFileToDb
{
    public class EmployeeIdAndSsn
    {
        public EmployeeIdAndSsn(string employeeId, string ssn)
        {
            EmployeeId = employeeId;
            Ssn = FixSsn(ssn);
        }

        private string FixSsn(string ssn)
        {
            return ssn.Replace("-", "");
        }

        public string EmployeeId { get; set; }
        public string Ssn { get; set; }
      
    }
}
