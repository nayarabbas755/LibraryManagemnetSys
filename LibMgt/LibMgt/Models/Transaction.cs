using LibMgt.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibMgt.Models
{
    public class Transaction:BaseEntity
    {
        public Guid BookID { get; set; }
        [ForeignKey("BookID")]
        public virtual Book Book { get; set; }
        public Guid PatronID { get; set; }
        [ForeignKey("PatronID")]
        public virtual Patron Patron { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? FineAmount { get; set; }
        public string? OtherDetails { get; set; }
    }
}
