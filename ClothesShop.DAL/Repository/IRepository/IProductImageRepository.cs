using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.DAL.Repository.IRepository
{
    public interface IProductImageRepository:IRepository<ProductImage>
    {
        void Update(ProductImage obj);

    }
}
