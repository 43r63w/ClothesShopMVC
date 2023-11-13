
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ClothesShop.Entities
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public string City { get; set; }

        public string StreetAdress { get; set; }

        public string PostalCode { get; set; }

    }
}
