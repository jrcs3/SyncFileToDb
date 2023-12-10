using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsvCommonLib
{
    public class EmployeeIdAndSsnForCsv
    {
        public EmployeeIdAndSsnForCsv(string employeeId, string ssn)
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
