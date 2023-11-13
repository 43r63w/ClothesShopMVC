using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.ViewModels;
using ClothesShop.Serivices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace ClothesShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ProductClothes"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach (var cart in shoppingCartVM.ShoppingCartsList)
            {
                cart.Price = GetPrice(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ProductClothes"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var cart in ShoppingCartVM.ShoppingCartsList)
            {
                cart.Price = GetPrice(cart);

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ShoppingCartsList = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == userId, includeProperties: "ProductClothes");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ShoppingCartVM.OrderHeader.TrackingNumber = Guid.NewGuid().ToString();

            ShoppingCartVM.OrderHeader.Carrier = "Нова пошта";

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartVM.ShoppingCartsList)
            {
                cart.Price = GetPrice(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentPending;


            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            foreach (var item in ShoppingCartVM.ShoppingCartsList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductClothesId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.Price,
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            var domain = "https://localhost:7028/";
            if (User.IsInRole(SD.Role_Customer))
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConformation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",

                };


                foreach (var item in ShoppingCartVM.ShoppingCartsList)
                {
                    var sessionItemInCart = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "uah",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.ProductClothes.Brand + " " + item.ProductClothes.Name,

                            }
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionItemInCart);
                }


                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdatePaymentStatus(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);



            }
            return RedirectToAction(nameof(OrderConformation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConformation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != SD.PaymentCancel)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdatePaymentStatus(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.OrderApproved, SD.PaymentApproved);
                }

                _unitOfWork.Save();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(SD.ShoppingCartKey, _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).Count());


            return View(id);
        }

        #region ManagmentCart
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, includeProperties: "ProductClothes");


            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.ShoppingCartKey, _unitOfWork.ShoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count());


            TempData["success"] = "Кошик оновленно!";
            return RedirectToAction(nameof(Index), new { id = cartFromDb.Id });
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, includeProperties: "ProductClothes");

            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
                HttpContext.Session.SetInt32(SD.ShoppingCartKey, _unitOfWork.ShoppingCart
                  .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);

            }
            _unitOfWork.Save();
            ; TempData["success"] = "Кошик оновленно!";
            return RedirectToAction(nameof(Index), new { id = cartFromDb.Id });
        }
        public IActionResult Delete(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, includeProperties: "ProductClothes");
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            HttpContext.Session.SetInt32(SD.ShoppingCartKey, _unitOfWork.ShoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.Save();
            TempData["success"] = "Товар видалленно";
            return RedirectToAction(nameof(Index), new { id = cartFromDb.Id });
        }


        #endregion


        private double GetPrice(ShoppingCart cart)
        {
            if (cart.ProductClothes.IsDiscount == true)
            {
                return cart.ProductClothes.PriceForSale;
            }
            else
            {
                return cart.ProductClothes.Price;
            }

        }



    }
}
