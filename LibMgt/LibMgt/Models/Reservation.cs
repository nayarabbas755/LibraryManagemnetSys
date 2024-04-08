using LibMgt.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibMgt.Models
{
    public class Reservation:BaseEntity
    {

        public Guid BookID { get; set; }
        [ForeignKey("BookID")]
        public virtual Book Book { get; set; }
        public Guid PatronID { get; set; }
        [ForeignKey("PatronID")]
        public virtual User Patron { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string? Status { get; set; }
        public string? OtherDetails { get; set; }
   
    }
}
