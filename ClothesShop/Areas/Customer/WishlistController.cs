using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using ClothesShop.Entities.ViewModels;
using ClothesShop.Serivices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Security.Claims;

namespace ClothesShop.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistVM WishlistVM { get; set; }

        public WishlistController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                HttpContext.Session.SetInt32(SD.DesiredKey, _unitOfWork.Wishlist.GetAll(u => u.ApplicationUserId == userId).Count());
            }


            WishlistVM = new()
            {
                Wishlists = _unitOfWork.Wishlist.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ProductClothes")
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            return View(WishlistVM);
        }


        public IActionResult AddToCart(int productId)
        {

            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ProductClothesId == productId && u.ApplicationUserId == userId, includeProperties: "ProductClothes");

            ShoppingCart cart = new()
            {
                ProductClothesId = productId,
                ApplicationUserId = userId,           
                Count = 1,
            };


            if (cartFromDB != null)
            {
                TempData["warning"] = "Товар є в кошику";
                return RedirectToAction(nameof(Index), new { id = productId });
            }
            else
            {

                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();

            }
            HttpContext.Session.SetInt32(SD.ShoppingCartKey, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            TempData["success"] = "Товар додано до кошика";
            return RedirectToAction(nameof(Index), new { id = productId });


        }


        public IActionResult Delete(int wishId)
        {
            var deleteFromDb = _unitOfWork.Wishlist.Get(u => u.Id == wishId);

            _unitOfWork.Wishlist.Remove(deleteFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Товар видаленно зі списку бажанного";
            return RedirectToAction("Index");

        }


    }
}
