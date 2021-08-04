using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JanumikaPro.Models;

namespace JanumikaPro.Data
{
    public class JanumikaProContext : DbContext
    {
        public JanumikaProContext (DbContextOptions<JanumikaProContext> options)
            : base(options)
        {
        }

        public DbSet<JanumikaPro.Models.User> User { get; set; }

        public DbSet<JanumikaPro.Models.Item> Item { get; set; }

        public DbSet<JanumikaPro.Models.Order> Order { get; set; }

        public DbSet<JanumikaPro.Models.Category> Category { get; set; }

        public DbSet<JanumikaPro.Models.CategoryImage> CategoryImage { get; set; }

        public DbSet<JanumikaPro.Models.Cart> Cart { get; set; }

        public DbSet<JanumikaPro.Models.CartItem> CartItem { get; set; }
    }
}
