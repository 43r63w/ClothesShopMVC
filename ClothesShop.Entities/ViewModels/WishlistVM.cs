using ClothesShop.Entities.Clothes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.Entities.ViewModels
{
    public class WishlistVM
    {
        public IEnumerable<Wishlist> Wishlists { get; set; } 
    }
}
