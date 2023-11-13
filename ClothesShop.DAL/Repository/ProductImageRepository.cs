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
    public class ProductImageRepository : Repository<ProductImage>,IProductImageRepository
    {
       private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ProductImage obj)
        {
            _context.ProductImages.Update(obj);   
        }
    }
}
