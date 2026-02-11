using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSys.Models
{
    public class Salary
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseSalary { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EffectiveDate { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; }
    }
}
