using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities.Clothes;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace ClothesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Upsert(int id)
        {
            if (id == 0)
            {
                CategoryClothes category = new CategoryClothes();

                return View(category);
            }

            CategoryClothes categoryClothes = _unitOfWork.Category.Get(u => u.Id == id);

            return View(categoryClothes);

        }
        [HttpPost]
        public IActionResult Upsert(CategoryClothes categoryClothes)
        {
            if (categoryClothes.Id == null)
            {
                _unitOfWork.Category.Add(categoryClothes);
            }
            else
            {
                _unitOfWork.Category.Update(categoryClothes);
            }

            _unitOfWork.Save();
            TempData["success"] = "Об'єкт успішно оновленно/доданно!";
            return RedirectToAction(nameof(Index));
        }

        #region APICALLS
        public IActionResult GetAll()
        {
            List<CategoryClothes> categoryClothesList = _unitOfWork.Category.GetAll().ToList();

            return Json(new { data = categoryClothesList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _unitOfWork.Category.Get(u=>u.Id == id);

            _unitOfWork.Category.Remove(categoryFromDb);    
            _unitOfWork.Save();

            return Json(new {success=true,message = "Категорію видаленно!"});
        }

        #endregion
    }
}
