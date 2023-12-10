using System.ComponentModel.DataAnnotations;

namespace EFCommonLib
{
    public class EmployeeIdAndSsn
    {
        [Key]
        public string EmployeeId { get; set; }
        public string? Ssn { get; set; }

    }
}
