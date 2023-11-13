using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.Entities.Clothes
{
    public class ProductClothes
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public double PriceForSale { get; set; }

        public bool IsDiscount { get; set; }

        public int CategoryClothesId { get; set; }
        [ForeignKey("CategoryClothesId")]
        [ValidateNever]
        public CategoryClothes CategoryClothes { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }

        [NotMapped]
        public string[] Availability { get; set; } = new string[] { "В наявності", "Закінчилось" };

        
    }
}
