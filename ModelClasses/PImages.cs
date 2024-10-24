using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class PImages
    {
        [Required]
        public int Id { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public string ImageUrl { get; set; }

        public string ProductName { get; set; }
    }
}
