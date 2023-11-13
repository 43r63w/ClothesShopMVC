using ClothesShop.DAL.Data;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities.Clothes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository
{
    public class ProductClothesRepository : Repository<ProductClothes>,IProductClothesRepository
    {
       private readonly ApplicationDbContext _context;

        public ProductClothesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductClothes>> SearchAsync(string search)
        {
           return  await _context.ProductClothes.Where(u=>u.Name.Contains(search)||u.Brand.Contains(search)).ToListAsync();
        }

        public void Update(ProductClothes obj)
        {
            _context.ProductClothes.Update(obj);   
        }
    }
}
