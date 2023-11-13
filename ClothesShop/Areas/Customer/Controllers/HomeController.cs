using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using ClothesShop.Entities.ViewModels;
using ClothesShop.Models;
using ClothesShop.Serivices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Claims;

namespace ClothesShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Search(string search)
        {

            var product = await _unitOfWork.ProductClothes.SearchAsync(search);


            IEnumerable<ProductImage> productsImage = _unitOfWork.ProductImage.GetAll();

            return View(product);

        }


        [Authorize]
        public IActionResult AddToWishlist(int productId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            var wishListFromDb = _unitOfWork.Wishlist.Get(u => u.ApplicationUserId == userId && u.ProductId==productId);

            if (wishListFromDb != null)
            {
                TempData["warning"] = "Такий товар вже є в списку!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Wishlist wishlist = new()
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    ProductClothes = _unitOfWork.ProductClothes.Get(u => u.Id == productId, includeProperties: "CategoryClothes,ProductImages"),
                };
                _unitOfWork.Wishlist.Add(wishlist);
                _unitOfWork.Save();
            }

            TempData["success"] = "Товар додано до бажаного";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Desired()
        {
            List<ProductClothes> productsList = _unitOfWork.ProductClothes.GetAll(includeProperties: "ProductImages,CategoryClothes").ToList();
            return View(productsList);
        }

        public IActionResult Index(string category)
        {

            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                HttpContext.Session.SetInt32(SD.ShoppingCartKey, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUser.Id == userId.Value).Count());
                HttpContext.Session.SetInt32(SD.DesiredKey, _unitOfWork.Wishlist.GetAll(u => u.ApplicationUserId == userId.Value).Count());
            }

            List<ProductClothes> productsList = _unitOfWork.ProductClothes.GetAll(includeProperties: "CategoryClothes,ProductImages").ToList();

            switch (category)
            {
                case "jackets":
                    productsList = productsList.Where(u => u.CategoryClothesId == 2).ToList();
                    break;
                case "downJackets":
                    productsList = productsList.Where(u => u.CategoryClothesId == 3).ToList();
                    break;
                case "longsleeves":
                    productsList = productsList.Where(u => u.CategoryClothesId == 4).ToList();
                    break;
                case "sweatshirts":
                    productsList = productsList.Where(u => u.CategoryClothesId == 5).ToList();
                    break;
                case "hoodie":
                    productsList = productsList.Where(u => u.CategoryClothesId == 6).ToList();
                    break;
                case "pants":
                    productsList = productsList.Where(u => u.CategoryClothesId == 7).ToList();
                    break;
                case "tShirt":
                    productsList = productsList.Where(u => u.CategoryClothesId == 8).ToList();
                    break;
                case "shorts":
                    productsList = productsList.Where(u => u.CategoryClothesId == 9).ToList();
                    break;
                case "sportsPants":
                    productsList = productsList.Where(u => u.CategoryClothesId == 10).ToList();
                    break;
                default:
                    break;

            }

            return View(productsList);
        }


        public IActionResult Sales(string category)
        {

            List<ProductClothes> productsForSales = _unitOfWork.ProductClothes.GetAll(includeProperties: "CategoryClothes,ProductImages").Where(u => u.IsDiscount == true).ToList();

            switch (category)
            {
                case "jackets":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 2).ToList();
                    break;
                case "downJackets":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 3).ToList();
                    break;
                case "longsleeves":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 4).ToList();
                    break;
                case "sweatshirts":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 5).ToList();
                    break;
                case "hoodie":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 6).ToList();
                    break;
                case "pants":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 7).ToList();
                    break;
                case "tShirt":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 8).ToList();
                    break;
                case "shorts":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 9).ToList();
                    break;
                case "sportsPants":
                    productsForSales = productsForSales.Where(u => u.CategoryClothesId == 10).ToList();
                    break;
                default:
                    break;

            }

            return View(productsForSales);
        }


        [Authorize]
        public IActionResult AddToCart(int productId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart cart = new()
            {
                ProductClothes = _unitOfWork.ProductClothes.Get(u => u.Id == productId, includeProperties: "CategoryClothes,ProductImages"),
                Count = 1,
                ProductClothesId = productId,
            };

            cart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.
                Get(u => u.ApplicationUserId == userId && u.ProductClothesId == cart.ProductClothesId);


            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.ShoppingCartKey,
               _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }


            TempData["success"] = "Товар додано до кошика";
            return RedirectToAction(nameof(Index));


        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                ProductClothes = _unitOfWork.ProductClothes.Get(u => u.Id == productId, includeProperties: "CategoryClothes,ProductImages"),
                Count = 1,
                ProductClothesId = productId
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
           
                 
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;


            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId
            && u.ProductClothesId == shoppingCart.ProductClothesId);


            if (cartFromDb != null)
            {
                cartFromDb.Count += 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();

            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.ShoppingCartKey,
            _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            }
            TempData["success"] = "Кошик оновленно!";

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}