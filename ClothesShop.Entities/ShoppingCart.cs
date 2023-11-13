using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClothesShop.Entities.Clothes;

namespace ClothesShop.Entities
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int ProductClothesId { get; set; }
        [ForeignKey("ProductClothesId")]
        [ValidateNever]
        public ProductClothes ProductClothes { get; set; }


        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }



        [Range(1, 1000, ErrorMessage = "Введіть від 1-1000!")]
        public int Count { get; set; }

        [NotMapped]
        public double Price { get; set; }


    }
}
