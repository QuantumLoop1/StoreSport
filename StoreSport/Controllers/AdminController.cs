using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StoreSport.Models;

namespace StoreSport.Controllers
{
    public class AdminController : Controller
    {
        private IStoreRepository repository;
        private readonly ILogger<AdminController> logger;

        public AdminController(IStoreRepository repo, ILogger<AdminController> log)
        {
            repository = repo;
            logger = log;
        }

        public ViewResult Index()
        {
            var products = repository.Products.ToList();
            logger.LogInformation($"Admin Index: Loading {products.Count} products");
            return View(products);
        }

        [HttpGet]
        public ViewResult Edit(int productId)
        {
            var product = repository.Products.FirstOrDefault(p => p.ProductId == productId);
            return View(product ?? new Product());
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                repository.SaveProduct(product);
                TempData["message"] = $"{product.Name} saved successfully";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpGet]
        public ViewResult Create() => View("Edit", new Product());

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                repository.CreateProduct(product);
                TempData["message"] = $"{product.Name} created successfully";
                return RedirectToAction("Index");
            }
            return View("Edit", product);
        }

        [HttpPost]
        public IActionResult Delete(int productId)
        {
            Product? deletedProduct = repository.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (deletedProduct != null)
            {
                repository.DeleteProduct(deletedProduct);
                TempData["message"] = $"{deletedProduct.Name} deleted successfully";
            }

            return RedirectToAction("Index");
        }
    }
}
