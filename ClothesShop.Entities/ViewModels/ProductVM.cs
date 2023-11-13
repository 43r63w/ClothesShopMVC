﻿using ClothesShop.Entities.Clothes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothesShop.Entities.ViewModels
{
    public class ProductVM
    {
        public ProductClothes ProductClothes { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryClothesList { get; set; }


    }
}
