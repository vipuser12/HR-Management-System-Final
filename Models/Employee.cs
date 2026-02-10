using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSys.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Department { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string JobTitle { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime HireDate { get; set; }

        [Column(TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        public virtual User User { get; set; }
        public virtual Salary Salary { get; set; }
    }
}
