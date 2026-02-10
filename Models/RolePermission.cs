using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagementSys.Models
{
    public class RolePermission
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        [Required]
        [ForeignKey("Permission")]
        public int PermissionId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
