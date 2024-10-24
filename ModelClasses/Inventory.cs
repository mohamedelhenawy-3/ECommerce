using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }



        [Required]
        public string Name { get; set; }


        [Range(1, 99, ErrorMessage = "Range between 1 to 99 only")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid decimal number with up to 2 decimal places.")]
        public double PurchasePrice { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }


        public string Category { get; set; }
    }
}
