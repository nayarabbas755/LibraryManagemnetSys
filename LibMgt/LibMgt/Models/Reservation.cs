using LibMgt.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibMgt.Models
{
    public class Reservation:BaseEntity
    {
     
        public Guid BookID { get; set; }
        [ForeignKey("BookID")]
        public Book Book { get; set; }
        public Guid PatronID { get; set; }
        [ForeignKey("PartronID")]
        public Patron Patron { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string? Status { get; set; }
        public string? OtherDetails { get; set; }
    }
}
