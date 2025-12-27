using Microsoft.AspNetCore.Mvc;
using StoreSport.Models;

namespace StoreSport.Controllers
{
    public class AdminController : Controller
    {
        private IStoreRepository repository;

        public AdminController(IStoreRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index() => View(repository.Products);

        public ViewResult Edit(int productId) =>
            View(repository.Products.FirstOrDefault(p => p.ProductId == productId));

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                repository.SaveProduct(product);
                TempData["message"] = $"{product.Name} был сохранен";
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        public ViewResult Create() => View("Edit", new Product());

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                repository.CreateProduct(product);
                TempData["message"] = $"{product.Name} был создан";
                return RedirectToAction("Index");
            }
            else
            {
                return View("Edit", product);
            }
        }

        [HttpPost]
        public IActionResult Delete(int productId)
        {
            Product? deletedProduct = repository.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (deletedProduct != null)
            {
                repository.DeleteProduct(deletedProduct);
                TempData["message"] = $"{deletedProduct.Name} был удален";
            }

            return RedirectToAction("Index");
        }
    }
}
