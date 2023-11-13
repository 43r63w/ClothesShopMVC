using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository.IRepository
{
    public interface IUnitOfWork
    {

        public ICategoryRepository Category { get; }
        public IProductClothesRepository ProductClothes { get; }
        public IProductImageRepository ProductImage { get; }

        public IShoppingCartRepository ShoppingCart { get; }

        public IOrderHeaderRepository OrderHeader { get; }

        public IOrderDetailRepository OrderDetail { get; }

        public IApplicationUserRepository ApplicationUser { get; }

        public IWishlistRepository Wishlist { get; }

        void Save();


    }
}
