using ClothesShop.Entities.Clothes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.Entities.ViewModels
{
    public class DesiredVM
    { 
        public ProductClothes ProductClothes { get; set; }


        public ApplicationUser ApplicationUser { get; set; }
    }
}
