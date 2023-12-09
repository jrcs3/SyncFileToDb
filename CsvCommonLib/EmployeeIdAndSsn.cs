using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsvCommonLib
{
    public class EmployeeIdAndSsn
    {
        public EmployeeIdAndSsn()
        {
            
        }
        public EmployeeIdAndSsn(string employeeId, string ssn)
        {
            EmployeeId = employeeId;
            Ssn = FixSsn(ssn);
        }

        private string FixSsn(string ssn)
        {
            return ssn.Replace("-", "");
        }

        [Key]
        public string EmployeeId { get; set; }
        public string Ssn { get; set; }

    }
}
