using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JanumikaPro.Models
{
    public enum UserType
    {
        Client,
        Admin
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public UserType Type { get; set; } = UserType.Client;

        public List<Order> Orders { get; set; }
        public Cart Cart { get; set; } 

    }
}
