using ClothesShop.Areas.Customer.Controllers;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.ViewModels;
using ClothesShop.Serivices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ClothesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "ProductClothes")
            };


            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult Update()
        {
            var updateOrder = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");

            updateOrder.Name = OrderVM.OrderHeader.Name;
            updateOrder.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            updateOrder.City = OrderVM.OrderHeader.City;
            updateOrder.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            updateOrder.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                updateOrder.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                updateOrder.Carrier = OrderVM.OrderHeader.Carrier;
            }
            _unitOfWork.OrderHeader.Update(updateOrder);
            _unitOfWork.Save();

            TempData["success"] = "Данні оновленні";

            return RedirectToAction(nameof(Details), new { orderId = updateOrder.Id });

        }

        #region  MANAGMENTORDER
        [HttpPost]
        public IActionResult StartProccesing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.OrderInProccess);
            _unitOfWork.Save();
            TempData["success"] = "Замовлення оновленно";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult Shipping()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.OrderDelivered);
            _unitOfWork.Save();
            TempData["success"] = "Замовлення оновленно";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };


                var service = new RefundService();
                Refund refund = service.Create(options);


                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderCancelled, SD.PaymentRefund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id,SD.OrderCancelled, SD.PaymentCancel);
            }

            _unitOfWork.Save();
            TempData["warning"] = "Замовлення скасованно";

            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });

        }



        #endregion


        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> objOrderHeader;

            if (User.IsInRole(SD.Role_Admin))
            {
                objOrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claim = (ClaimsIdentity)User.Identity;
                var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeader = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }


            return Json(new { data = objOrderHeader });
        }


        #endregion
    }
}
