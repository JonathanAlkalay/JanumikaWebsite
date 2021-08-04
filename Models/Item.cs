using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JanumikaPro.Models
{
    public class Item
    {

        public int ItemId { get; set; }

        public string Name { get; set; }


        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }

        [DataType(DataType.Currency)]
        public float Price { get; set; }

        public string Image { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();


    }
}
