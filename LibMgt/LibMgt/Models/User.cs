
using Microsoft.AspNetCore.Identity;

namespace LibMgt.Models
{
    public class User : IdentityUser<Guid>
    {

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModifiedTime { get; set; } = null;
        public DateTime? DeletionTIme { get; set; } = null;
        public virtual ICollection<User> Users { get; set;}
    }
}
