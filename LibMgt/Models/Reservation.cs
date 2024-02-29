namespace LibMgt.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int BookID { get; set; }
        public int PatronID { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string? Status { get; set; }
        public string? OtherDetails { get; set; }
    }
}
