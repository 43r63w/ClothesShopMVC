using ClothesShop.Entities.Clothes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public ProductClothes ProductClothes { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }

    }
}
