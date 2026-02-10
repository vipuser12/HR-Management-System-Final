using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSys.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string PasswordHash { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Column(TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
