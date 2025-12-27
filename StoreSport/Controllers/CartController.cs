using Microsoft.AspNetCore.Mvc;
using StoreSport.Models;
using StoreSport.Infrastructure;

namespace StoreSport.Controllers
{
    public class CartController : Controller
    {
        private IStoreRepository repository;
        private Cart cart;

        public CartController(IStoreRepository repo, Cart cartService)
        {
            repository = repo;
            cart = cartService;
        }

        public ViewResult Index(string returnUrl)
        {
            return View(new Models.ViewModels.CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl ?? "/"
            });
        }

        public RedirectToActionResult AddToCart(int productId, string returnUrl)
        {
            Product? product = repository.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                cart.AddItem(product, 1);
                if (cart is SessionCart sessionCart)
                {
                    sessionCart.Session?.SetJson("Cart", sessionCart);
                }
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToActionResult RemoveFromCart(int productId, string returnUrl)
        {
            Product? product = repository.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                cart.RemoveLine(product);
                if (cart is SessionCart sessionCart)
                {
                    sessionCart.Session?.SetJson("Cart", sessionCart);
                }
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        // API дл€ получени€ количества товаров в корзине (дл€ AJAX)
        [HttpGet]
        public JsonResult GetCartCount()
        {
            return Json(new { count = cart.Lines.Sum(l => l.Quantity) });
        }
    }
}
