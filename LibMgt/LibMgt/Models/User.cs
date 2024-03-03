
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace LibMgt.Models
{
    public class User : IdentityUser<Guid>
    {

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModifiedTime { get; set; } = null;
        public DateTime? DeletionTIme { get; set; } = null;
        public virtual ICollection<User> Users { get; set;}
        [JsonIgnore]
        public override string PasswordHash { get; set; }
        [JsonIgnore]
        public override string NormalizedEmail { get; set; }
        [JsonIgnore]
        public override string NormalizedUserName { get; set; }
        [JsonIgnore]
        public override string SecurityStamp { get; set; }
        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }
        [JsonIgnore]
        public override string PhoneNumber { get; set; }
        [JsonIgnore]
        public override bool PhoneNumberConfirmed { get; set; }
        [JsonIgnore]
        public override bool TwoFactorEnabled { get; set; }
        [JsonIgnore]
        public override int AccessFailedCount { get; set; }
    }
}
