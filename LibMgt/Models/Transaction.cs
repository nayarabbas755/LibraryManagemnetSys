namespace LibMgt.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public int BookID { get; set; }
        public int PatronID { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? FineAmount { get; set; }
        public string? OtherDetails { get; set; }
    }
}
