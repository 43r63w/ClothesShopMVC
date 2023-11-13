
using ClothesShop.DAL.Repository.IRepository;
using ClothesShop.Entities;
using ClothesShop.Entities.Clothes;
using ClothesShop.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

      
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryClothesList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),

                ProductClothes = new ProductClothes()
            };

            if (id == 0 || id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.ProductClothes = _unitOfWork.ProductClothes.Get(u => u.Id == id, includeProperties: "ProductImages");

                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.ProductClothes.Id == 0)
                {
                    _unitOfWork.ProductClothes.Add(productVM.ProductClothes);

                }
                else
                {
                    _unitOfWork.ProductClothes.Update(productVM.ProductClothes);
                }
                _unitOfWork.Save();

                var wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var productPath = @"images\products\product" + productVM.ProductClothes.Id;
                        var finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductClothesId = productVM.ProductClothes.Id
                        };

                        if (productVM.ProductClothes.ProductImages == null)
                        {
                            productVM.ProductClothes.ProductImages = new List<ProductImage>();
                        }
                        productVM.ProductClothes.ProductImages.Add(productImage);


                    }
                    _unitOfWork.ProductClothes.Update(productVM.ProductClothes);
                    _unitOfWork.Save();
                }

                TempData["success"] = "Продукт оновленно/доданно!";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryClothesList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });

                return View(productVM);
            }
        }


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int ProductClothesId = imageToBeDeleted.ProductClothesId;


            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.Trim('\\'));


                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }

                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Товар оновленно!";
            }


            return RedirectToAction(nameof(Upsert), new { id = ProductClothesId });
        }




        #region APICALLS
        public IActionResult GetAll()
        {
            List<ProductClothes> productsList = _unitOfWork.ProductClothes.
                GetAll(includeProperties: "CategoryClothes").ToList();

            return Json(new { data = productsList });

        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productToBeDeleted = _unitOfWork.ProductClothes.Get(u => u.Id == id);

            if (productToBeDeleted != null)
            {
                var productPath = @"images\products\product" + id;
                var finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

                if (Directory.Exists(finalPath))
                {
                    string[] imagesPath = Directory.GetFiles(finalPath);
                    foreach (string imagePath in imagesPath)
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    Directory.Delete(finalPath);
                }

                _unitOfWork.ProductClothes.Remove(productToBeDeleted);
                _unitOfWork.Save();
                TempData["success"] = "Товар видаленно!";
            }

            return Json(new { success = true, message = "Товар видаленно!" });

        }


        #endregion

    }



}
