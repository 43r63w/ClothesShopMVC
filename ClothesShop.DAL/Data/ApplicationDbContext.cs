using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        public DbSet<CategoryClothes> CategoryClothes { get; set; }
        public DbSet<ProductClothes> ProductClothes { get; set; }
  

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set;}

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
