using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ModelClasses.ViewModels
{
    public class ProductViewModel
    {
      public Product Product { get; set; }
      public IEnumerable<SelectListItem> ? CategoriesList { get; set; } 
      public PImages? PImage { get; set; }
      public  Inventory Inventories { get; set; } 
      public IEnumerable<IFormFile> Images { get; set; } 

    }
}
