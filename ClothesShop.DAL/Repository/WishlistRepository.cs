using ClothesShop.DAL.Data;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository
{
    public class WishlistRepository : Repository<Wishlist>,IWishlistRepository
    {
       private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

     
        public void Update(Wishlist obj)
        {
            _context.Wishlists.Update(obj);   
        }
    }
}
