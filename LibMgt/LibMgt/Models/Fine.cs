using LibMgt.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibMgt.Models
{
    public class Fine : BaseEntity
    {

        public Guid PatronID { get; set; }
        [ForeignKey("PatronID")]
        public virtual Patron Patron { get; set; }
        public decimal? FineAmount { get; set; }
        public DateTime? FineDate { get; set; }
        public string? Status { get; set; }
        public string? OtherDetails { get; set; }
    }

}
