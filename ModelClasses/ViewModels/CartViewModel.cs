using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<UserCart> ProductList { get; set; }
    }
}
