using LibMgt.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace LibMgt.Models
{
    public class Book:BaseEntity
    {

        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public string? Genre { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? AvailabilityStatus { get; set; }
        public string? OtherDetails { get; set; }
    }
}
