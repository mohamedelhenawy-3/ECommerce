using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModels
{
    public class OrderViewModel
    {
        public IEnumerable<UserCart>? UserCartList { get; set; }
        public UserOrderHeader? UserOrderHeader { get; set; }
        public string? UserCartId { get; set; }
        public IEnumerable<SelectListItem>? PaymentOptiens { get; set; }


        public double? PaymentByCard { get; set; } = 0.0;
    }
}
