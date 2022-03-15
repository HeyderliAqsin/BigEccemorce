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
        private readonly IWebHostEnvironment _webHost;
        private readonly PictureManager _pictureManager;

        public ProductController(ProductManager productManager, IWebHostEnvironment webHost, PictureManager pictureManager)
        {
            _productManager = productManager;
            _webHost = webHost;
            _pictureManager = pictureManager;
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
        public async Task<IActionResult> Action(int? id,IFormCollection collection)
        {
            var model = GetProductActionViewModelFromFile(collection);

            try
            {
                var pictureList = collection.Files;
                var upd = "uploads";
                var rootFile = Path.Combine(_webHost.WebRootPath, upd);
                if (!Directory.Exists(upd))
                {
                    Directory.CreateDirectory(upd);
                }
                if (id.HasValue)
                {

                }
                else
                {
                    Product product = new()
                    {
                        CategoryId = model.CategoryID,
                        Price = model.Price,
                        Discount = model.Discount,
                        InStock = (ushort)model.StockQuantity,
                        IsFeatured = model.IsFeatured,
                        IsSlider = model.IsSlider,
                        ModifiedOn = DateTime.Now,
                        PublishDate = DateTime.Now,
                        IsDay = model.DayProduct
                    };
                    if(pictureList != null && pictureList.Count > 0)
                    {
                        product.ProductPictures = new List<ProductPicture>();
                        foreach (var picture in pictureList)
                        {
                            string fileName = Guid.NewGuid() + picture.FileName;
                            string fileRoot=Path.Combine(rootFile,fileName);
                            using FileStream stream = new(fileRoot, FileMode.Create);
                            picture.CopyTo(stream);
                            Picture newpicture=new()
                            {   
                                URL = "/uploads/" + fileName
                            };

                            //Picture add sql code....
                            await _pictureManager.AddPicture(newpicture);
                            product.ProductPictures.Add(
                                new ProductPicture { PictureID = newpicture.Id, ProductID = product.Id }
                              );
                        }

                    }
                    var currentLanguageRecord = new ProductRecord
                    {
                        ProductId = product.Id,
                        LanguageId = 1,
                        Name = model.Name,
                        Summary = model.Summary,
                        Description = model.Description
                    };
                    product.ProductRecords = new List<ProductRecord>();
                    product.ProductRecords.Add(currentLanguageRecord);
                    await _productManager.Add(product);
                }
                return RedirectToAction("index");
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
                CategoryID = !string.IsNullOrEmpty(collection["CategoryID"]) ? int.Parse(collection["CategoryID"]) : 1,
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
