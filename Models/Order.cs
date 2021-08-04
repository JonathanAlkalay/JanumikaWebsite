using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JanumikaPro.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public double TotalPrice { get; set; } 

        public DateTime Date { get; set; }

        public Cart Cart { get; set; }

        public int UserId { get; set; }
        
        public User User { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }



    }
}
