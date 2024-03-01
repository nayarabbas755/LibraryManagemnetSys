using LibMgt.Models.Base;

namespace LibMgt.Models
{
    public class Genre:BaseEntity
    {
        public string GenreName { get; set; }
        public string Description { get; set; }
        public string OtherDetails { get; set; }
    }
}
