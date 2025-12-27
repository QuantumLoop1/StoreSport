using Microsoft.AspNetCore.Mvc;
using StoreSport.Models;
using StoreSport.Models.ViewModels;

namespace StoreSport.Controllers
{
    public class HomeController : Controller
    {
        private IStoreRepository repository;
        public int PageSize = 4;

        public HomeController(IStoreRepository repo)
        {
            repository = repo;
        }

        public ViewResult Index(string? category, int productPage = 1)
        {
            var productsQuery = repository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductId);

            var products = productsQuery
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var totalItems = category == null
                ? repository.Products.Count()
                : repository.Products.Where(e => e.Category == category).Count();

            return View(new ProductsListViewModel
            {
                Products = products,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = totalItems
                },
                CurrentCategory = category
            });
        }
    }
}
