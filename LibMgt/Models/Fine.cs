namespace LibMgt.Models
{
    public class Fine
    {
        public int FineID { get; set; }
        public int PatronID { get; set; }
        public decimal? FineAmount { get; set; }
        public DateTime? FineDate { get; set; }
        public string? Status { get; set; }
        public string? OtherDetails { get; set; }
    }

}
