﻿using LibMgt.Models.Base;

namespace LibMgt.Models
{
    public class Patron:BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OtherDetails { get; set; }
    }
}
