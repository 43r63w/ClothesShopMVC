using ClothesShop.DAL.Data;
using ClothesShop.DAL.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        public ICategoryRepository Category { get; private set; }

        public IProductClothesRepository ProductClothes { get; private set; }

        public IProductImageRepository ProductImage { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IWishlistRepository Wishlist { get; private set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryClothesRepository(_context);
            ProductClothes = new ProductClothesRepository(_context);
            ProductImage = new ProductImageRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            Wishlist = new WishlistRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
