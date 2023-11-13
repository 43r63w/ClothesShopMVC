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
    public class ApplicationUserRepository : Repository<ApplicationUser>,IApplicationUserRepository
    {
       private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ApplicationUser obj)
        {
            _context.ApplicationUsers.Update(obj);   
        }
    }
}
