using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace JanumikaPro.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public Cart Cart { get; set; }

        [DefaultValue(1)]
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public Item Item { get; set; }
    }
}
