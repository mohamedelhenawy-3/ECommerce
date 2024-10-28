using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
   public  class UserOrderHeader
    {
        [Key]
        public int Id { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser AppUser { get; set; }
        [Required]
        public DateTime DateOfOrder { get; set; }

        public DateTime? DateOfShipping { get; set; }
        [Required]
        public double TotalAmount { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public string? OrderState { get; set; }
        public string? StripeSessionId { get; set; }

        public string? StripePaymentIntendId { get; set; }

        public string? PaymentState { get; set; }

        public DateTime? PaymentProcessDate { get; set; }

        public string? TransactId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string DeliveryStreetAddress { get; set; }
        [Required]
        public string CIty { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
