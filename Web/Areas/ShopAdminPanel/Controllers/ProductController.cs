using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Web.ViewModels;

namespace Web.Areas.ShopAdminPanel.Controllers
{
    [Area(nameof(ShopAdminPanel))]
    public class ProductController : Controller
    {
        private readonly ProductManager _productManager;

        public ProductController(ProductManager productManager)
        {
            _productManager = productManager;
        }

        // GET: ProductController
        public async Task<IActionResult> Index()
        {
            return View(await _productManager.GetAllAdmin());
        }


        // GET: ProductController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();
            var selectedProduct = await _productManager.GetById(id.Value);
            if (selectedProduct == null) return NotFound();
            return View(selectedProduct);
        }

        // GET: ProductController/Create
        public async Task<IActionResult> Action(int? id)
        {
            ProductActionVM model = new();
            if (id.HasValue)
            {
                var product = await _productManager.GetById(id.Value);
                if (product == null) return NotFound();

                var currentLanguageRecord = new ProductRecord();
                model.ProductID = product.Id;
                model.CategoryID = product.CategoryId;
                model.Price = product.Price;
                model.Discount=product.Discount;
                model.IsFeatured = product.IsFeatured;
                model.IsSlider = product.IsSlider;
                model.DayProduct = product.IsDay;
                model.StockQuantity = product.InStock;
                model.ThumbnailPicture = product.CoverPhotoId;
                model.ProductRecordID = currentLanguageRecord.Id;

                model.Name = currentLanguageRecord.Name;
                model.Summary=currentLanguageRecord.Summary;
                model.Description=currentLanguageRecord.Description;
                //var category = _cateryManager.GetCategoryId(model.CategoryID);
                return View(model);



            }

            return View(model);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Action(int? id,IFormCollection collection,IFormFile[] test)
        {
            ProductActionVM model = GetProductActionViewModelFromFile(collection);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
        public static ProductActionVM GetProductActionViewModelFromFile(IFormCollection collection)
        {
            var model = new ProductActionVM
            {
                ProductID = !string.IsNullOrEmpty(collection["ProductID"]) ? int.Parse(collection["ProductID"]) : 0,
                CategoryID =  int.Parse(collection["CategoryID"]),
                Price = int.Parse(collection["Price"]),
                Discount = !string.IsNullOrEmpty(collection["Discount"]) ? int.Parse(collection["Discount"]) : 0,
                StockQuantity = int.Parse(collection["StockQuantity"]),
                ProductRecordID = !string.IsNullOrEmpty(collection["ProductRecordID"]) ? int.Parse(collection["ProductRecordID"]) : 0,
                IsSlider = collection["IsSlider"].Contains("true"),
                IsFeatured = collection["IsFeatured"].Contains("true"),
                DayProduct= collection["DayProduct"].Contains("true"),
                ProductPictures = collection["ProductPictures"],
                ThumbnailPicture= !string.IsNullOrEmpty(collection["ThumbnailPicture"]) ? int.Parse(collection["ThumbnailPicture"]) : 0,
                Name=collection["name"],
                Summary = collection["Summary"],
                Description= collection["description"],

            };
            return model;
        }



        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
