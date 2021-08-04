using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JanumikaPro.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double TotalPrice { get; set; } = 0;

        public List<CartItem> Items { get; set; } 

    }
}
