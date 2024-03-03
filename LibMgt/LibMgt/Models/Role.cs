using Microsoft.AspNetCore.Identity;

namespace LibMgt.Models
{
    public class Role:IdentityRole<Guid>
    {
        public int RoleType { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModifiedTime { get; set; } = null;
        public DateTime? DeletionTIme { get; set; } = null;

    }
}
