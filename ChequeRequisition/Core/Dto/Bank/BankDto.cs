﻿using ChequeRequisiontService.Core.Dto.Vendor;
using System.ComponentModel.DataAnnotations;

namespace ChequeRequisiontService.Core.Dto.Bank
{
    public class BankDto
    {
        public int Id { get; set; }
        [Required]
        public int VendorId { get; set; }
        public required string BankName { get; set; } 
        public required string BankCode { get; set; }
        public required string RoutingNumber { get; set; }
        public required string BankEmail { get; set; }
        public required string BankPhone { get; set; }
        public required string BankAddress { get; set; }
        public Boolean IsActive { get; set; } = true;
        public VendorDto? Vendor { get; set; }
        public string VendorName => Vendor?.VendorName ?? "";
    }
}
