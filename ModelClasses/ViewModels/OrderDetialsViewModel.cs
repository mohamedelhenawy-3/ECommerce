using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModels
{
    public class OrderDetialsViewModel
    {
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
        public UserOrderHeader? UserOrderHeader { get; set; }
    }
}
