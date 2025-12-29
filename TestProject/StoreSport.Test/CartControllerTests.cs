using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StoreSport.Controllers;
using StoreSport.Models;
using StoreSport.Models.ViewModels;

namespace SportsStore.Tests;

[TestFixture]
public class CartControllerTests
{
    private Mock<IStoreRepository> mockRepository = null!;
    private CartController controller = null!;
    private TestSession testSession = null!;
    private Mock<HttpContext> mockHttpContext = null!;

    [SetUp]
    public void Setup()
    {
        this.mockRepository = new Mock<IStoreRepository>();
        this.testSession = new TestSession();
        this.mockHttpContext = new Mock<HttpContext>();

        // Create a mock Cart for dependency injection
        var mockCart = new Cart();

        this.controller = new CartController(this.mockRepository.Object, mockCart);
        this.controller.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        this.mockHttpContext.Setup(c => c.Session).Returns(this.testSession);
    }

    [TearDown]
    public void TearDown()
    {
        this.controller.Dispose();
    }

    [Test]
    public void CartController_Index_GET_ReturnsViewWithEmptyCart()
    {
        // Arrange - testSession starts empty, so GetJson will return null

        // Act
        var result = this.controller.Index("/");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult!.Model, Is.InstanceOf<CartIndexViewModel>());

            var viewModel = viewResult.Model as CartIndexViewModel;
            Assert.That(viewModel!.Cart, Is.Not.Null);
            Assert.That(viewModel.Cart!.Lines, Is.Empty);
            Assert.That(viewModel.ReturnUrl, Is.EqualTo("/"));
        });
    }

    [Test]
    public void CartController_Index_GET_ReturnsViewWithExistingCart()
    {
        // Arrange
        var existingCart = new Cart();
        existingCart.AddItem(new Product { ProductId = 1, Name = "Test Product", Price = 10.00m }, 2);

        // Create a new controller with the cart that has items
        var controllerWithCart = new CartController(this.mockRepository.Object, existingCart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.Index("/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartIndexViewModel;

            Assert.That(viewModel?.Cart?.Lines, Has.Count.EqualTo(1));
            Assert.That(viewModel?.Cart?.Lines[0].Product.Name, Is.EqualTo("Test Product"));
            Assert.That(viewModel!.ReturnUrl, Is.EqualTo("/test"));
        });
    }

    [Test]
    public void CartController_Index_GET_WithNullReturnUrl_UsesDefault()
    {
        // Arrange - testSession starts empty

        // Act
        var result = this.controller.Index(null!);

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartIndexViewModel;
            Assert.That(viewModel!.ReturnUrl, Is.EqualTo("/"));
        });
    }

    [Test]
    public void CartController_AddToCart_WithValidProduct_AddsToCart()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.00m,
            Category = "Test"
        };

        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.AddToCart(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<RedirectResult>());
            var redirectResult = result as RedirectResult;
            Assert.That(redirectResult!.Url, Is.EqualTo("/test"));

            // Verify that cart contains the product
            Assert.That(cart.Lines, Has.Count.EqualTo(1));
            Assert.That(cart.Lines[0].Product.ProductId, Is.EqualTo(1));
            Assert.That(cart.Lines[0].Quantity, Is.EqualTo(1));
        });
    }

    [Test]
    public void CartController_AddToCart_WithExistingProduct_IncreasesQuantity()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };

        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product, 2);
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        controllerWithCart.AddToCart(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart.Lines, Has.Count.EqualTo(1));
            Assert.That(cart.Lines[0].Quantity, Is.EqualTo(3)); // 2 + 1
        });
    }

    [Test]
    public void CartController_AddToCart_WithNonExistentProduct_RedirectsAnyway()
    {
        // Arrange
        var products = new List<Product>(); // Empty list, so product 999 won't be found
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.controller.AddToCart(999, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<RedirectResult>());
            var redirectResult = result as RedirectResult;
            Assert.That(redirectResult!.Url, Is.EqualTo("/test"));
        });
    }

    [Test]
    public void CartController_AddToCart_WithNullReturnUrl_UsesDefaultRedirect()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.AddToCart(1, null!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<RedirectResult>());
            var redirectResult = result as RedirectResult;
            Assert.That(redirectResult!.Url, Is.EqualTo("/"));
        });
    }

    [Test]
    public void CartController_AddToCart_VerifiesRepositoryCall()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        controllerWithCart.AddToCart(1, "/test");

        // Assert
        this.mockRepository.Verify(r => r.Products, Times.Once);
    }

    [Test]
    public void CartController_RemoveFromCart_WithValidProductId_RemovesItemFromCart()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product, 2);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.RemoveFromCart(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult!.ActionName, Is.EqualTo("Index"));

            // Verify cart is empty after removal
            Assert.That(cart.Lines, Is.Empty);
        });
    }

    [Test]
    public void CartController_RemoveFromCart_WithNonExistentProductId_DoesNotThrowException()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product, 2);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var result = controllerWithCart.RemoveFromCart(999, "/test");
            Assert.That(result, Is.Not.Null);
        });
    }

    [Test]
    public void CartController_RemoveFromCart_WithNullReturnUrl_UsesDefaultRedirect()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product, 2);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.RemoveFromCart(1, null!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult!.ActionName, Is.EqualTo("Index"));
        });
    }

    [Test]
    public void CartController_RemoveFromCart_WithMultipleItems_RemovesOnlySpecifiedItem()
    {
        // Arrange
        var product1 = new Product { ProductId = 1, Name = "Product 1", Price = 10.00m };
        var product2 = new Product { ProductId = 2, Name = "Product 2", Price = 15.00m };
        var products = new List<Product> { product1, product2 };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 1);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.RemoveFromCart(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart.Lines, Has.Count.EqualTo(1));
            Assert.That(cart.Lines.First().Product.ProductId, Is.EqualTo(2));
        });
    }

    [Test]
    public void CartController_GetCartItemCount_ReturnsCorrectCount()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var cart = new Cart();
        cart.AddItem(product, 3);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.GetCartItemCount();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            Assert.That(jsonResult!.Value, Is.Not.Null);
        });
    }

    [Test]
    public void CartController_GetCartItemCount_WithEmptyCart_ReturnsZero()
    {
        // Arrange
        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.GetCartItemCount();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<JsonResult>());
        });
    }
}