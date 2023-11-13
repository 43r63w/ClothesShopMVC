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
    public class CategoryClothesRepository : Repository<CategoryClothes>,ICategoryRepository
    {
       private readonly ApplicationDbContext _context;

        public CategoryClothesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(CategoryClothes obj)
        {
            _context.CategoryClothes.Update(obj);   
        }
    }
}
