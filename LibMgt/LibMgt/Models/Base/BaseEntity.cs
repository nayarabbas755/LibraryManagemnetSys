namespace LibMgt.Models.Base
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreationTime {  get; set; } 
        public bool IsDeleted { get; set; }=false;
        public DateTime? LastModifiedTime { get; set; } = null; 
        public DateTime? DeletionTIme { get; set; } = null; 

    }
}
