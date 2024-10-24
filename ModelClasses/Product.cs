using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ModelClasses
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Range between 1 to 99 only")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid decimal number with up to 2 decimal places.")]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.'-]+$", ErrorMessage = "Description contains invalid characters.")]
        public string Description { get; set; }


        public ICollection<PImages>? ImgUrls { get; set; }


        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }

        // Navigation property to related category
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }


        public string? HomeImgUrl { get; set; }
    }
}
