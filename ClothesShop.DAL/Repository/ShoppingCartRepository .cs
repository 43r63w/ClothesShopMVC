using ClothesShop.DAL.Data;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository
    {
       private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ShoppingCart obj)
        {
            _context.ShoppingCarts.Update(obj);   
        }
    }
}
