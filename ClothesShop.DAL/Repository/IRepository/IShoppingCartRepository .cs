using ClothesShop.Entities.Clothes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository.IRepository
{
    public interface ICategoryRepository:IRepository<CategoryClothes>
    {
        void Update(CategoryClothes obj);
    }
}
