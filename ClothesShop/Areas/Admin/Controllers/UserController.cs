using ClothesShop.DAL.Data;
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APICALLS
        public IActionResult GetAll() 
        {
            List<ApplicationUser> applicationUsersList = _unitOfWork.ApplicationUser.GetAll().ToList();
            return Json(new { data = applicationUsersList });
        }
        #endregion
    }
}
